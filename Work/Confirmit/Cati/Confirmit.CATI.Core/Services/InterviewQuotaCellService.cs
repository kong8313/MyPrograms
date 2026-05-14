using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Collections.Generic;
using System.Data;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Diagnostics;
using System.IO;
using Confirmit.CATI.Core.Services.Survey.Quota;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.DAL.Framework;
using System;
using System.Threading;

namespace Confirmit.CATI.Core.Services
{
    public class InterviewQuotaCellService : IInterviewQuotaCellService
    {
        private const int BatchSize = 10000;

        private readonly IInterviewQuotaCellRepository _interviewQuotaCellRepository;
        private readonly QuotaMatcherBuilder _quotaMatcherBuilder;
        private readonly IReplicatedDataRepository _replicatedDataRepository;

        public InterviewQuotaCellService(
           IInterviewQuotaCellRepository interviewQuotaCellRepository,
           QuotaMatcherBuilder quotaMatcherBuilder,
           IReplicatedDataRepository replicatedDataRepository)
        {
            _interviewQuotaCellRepository = interviewQuotaCellRepository;
            _quotaMatcherBuilder = quotaMatcherBuilder;
            _replicatedDataRepository = replicatedDataRepository;
        }

        public void PopulateBatch(QuotaMatcher quotaMatcher, DataTable interviewsData)
        {
            var newEntities = new List<BvInterviewQuotaCellEntity>();

            foreach (var interview in interviewsData.Rows)
            {
                var interviewQuotaCells = quotaMatcher.GetInterviewQuotaCells((DataRow)interview);
                newEntities.AddRange(interviewQuotaCells);
            }

            _interviewQuotaCellRepository.Insert(newEntities);
        }

        public void Delete(int surveyId, List<int> interviewIds)
        {
            _interviewQuotaCellRepository.Delete(surveyId, interviewIds);
        }

        public void Populate(int surveyId, int quotaId)
        {
            var quotaMatcher = _quotaMatcherBuilder.Build(surveyId, quotaId);

            Populate(surveyId, quotaMatcher, CancellationToken.None);
        }

        public void Populate(int surveyId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _interviewQuotaCellRepository.Delete(surveyId);

            cancellationToken.ThrowIfCancellationRequested();
            var quotaMatcher = _quotaMatcherBuilder.Build(surveyId);

            cancellationToken.ThrowIfCancellationRequested();
            Populate(surveyId, quotaMatcher, cancellationToken);
        }

        private void Populate(int surveyId, QuotaMatcher quotaMatcher, CancellationToken cancellationToken)
        {
            using (var reader = _replicatedDataRepository.ExecuteReplicatedDataReader(surveyId))
            {
                var batch = DatabaseEngine.ReadBatch(reader, BatchSize);
                while (batch.Rows.Count > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    PopulateBatch(quotaMatcher, batch);

                    batch = DatabaseEngine.ReadBatch(reader, BatchSize);
                }
            }
        }
    }
}
