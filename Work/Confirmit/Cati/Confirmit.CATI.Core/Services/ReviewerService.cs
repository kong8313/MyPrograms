using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Resources;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.Configuration;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Core.Services
{
    public class ReviewerService : IReviewerService
    {
        private const string ReadScope = "reviewer api.reviewerservice.read";
        private const string WriteScope = "reviewer api.reviewerservice.write";

        private readonly IReviewerSettings _settings;
        private readonly IBatchFactory _batchFactory;
        private readonly IResponseReviewerApiClient _responseReviewerApiClient;
        private readonly ICompanyInfo _companyInfo;

        public ReviewerService(IBatchFactory batchFactory, IReviewerSettings settings, IResponseReviewerApiClient responseReviewerApiClient, ICompanyInfo companyInfo)
        {
            _batchFactory = batchFactory;
            _responseReviewerApiClient = responseReviewerApiClient;
            _settings = settings;
            _companyInfo = companyInfo;
        }

        public string CreateSessionForReview(string sessionName, int surveyId, string userName, BatchParameters batchParameters)
        {
            using (var batch = _batchFactory.CreateDatabaseBatch(batchParameters))
            using (var scope = new DatabaseTransactionScope("Create session"))
            {
                if (batch.Size > 100)
                    throw new UserMessageException(Strings.SendToReviewLimitMessage);

                var interviewsTransferArray =
                    BvTransferArraysAdapter.GetByCondition("[BatchID] = @BatchID\r\n",
                    new SqlParameter("@BatchID", batch.Id));
                var interviewIds = interviewsTransferArray.Select(x => x.ItemID).ToArray();

                BvSpInterviews_UpdateIsSentToReview_BatchAdapter.ExecuteNonQuery(surveyId, batch.Id, (int)ReviewStatus.SentToReview);
                
                var projectId = SurveyRepository.GetById(surveyId).Name;
                var session = _responseReviewerApiClient.AddSession(new SessionModel
                {
                    CompanyId = _companyInfo.CompanyId,
                    Name = sessionName,
                    ProjectId = projectId,
                    InterviewIds = interviewIds,
                    CreatedDate = DateTime.Now,
                    CreatedByUser = userName,
                    CreatedByCompanyId = _companyInfo.CompanyId
                }).ConfigureAwait(false).GetAwaiter().GetResult();
                
                scope.Commit();

                return string.Format(GetReviewerUrlTemplate(), session.SessionId);
            }
        }

        public string GetReviewerUrlTemplate()
        {
            var urlTemplate = BootstrapConfig.IsContainerEnvironment
                ? $"https://{new UriBuilder(ConfirmitConfiguration.ConfirmitUrl).Host}/reviewer/{{0}}"
                : _settings.SessionUrlTemplate;
            return urlTemplate;
        }
    }
}
