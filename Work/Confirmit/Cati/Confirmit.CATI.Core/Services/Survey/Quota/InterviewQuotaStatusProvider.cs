using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.SurveyEngine.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.Services.Survey.Quota
{
    public class InterviewQuotaStatusProvider
    {
        private readonly ISeQuotaRepository _quotaRepository;
        private readonly IQuotaCellRepository _quotaCellRepository;
        private readonly IInterviewQuotaCellRepository _interviewQuotaCellRepository;
        private readonly IReplicatedDataRepository _replicatedDataRepository;

        public InterviewQuotaStatusProvider(
            ISeQuotaRepository quotaRepository,
            IQuotaCellRepository quotaCellRepository,
            IInterviewQuotaCellRepository interviewQuotaCellRepository,
            IReplicatedDataRepository replicatedDataRepository)
        {
            _quotaRepository = quotaRepository;
            _quotaCellRepository = quotaCellRepository;
            _interviewQuotaCellRepository = interviewQuotaCellRepository;
            _replicatedDataRepository = replicatedDataRepository;
        }

        public IReadOnlyCollection<InterviewQuotaStatusItem> GetQuotaStatus(int surveyId, int interviewId)
        {
            var result = new List<InterviewQuotaStatusItem>();

            var interviewCells = _interviewQuotaCellRepository.GetByInterviewId(surveyId, interviewId);
            
            // get replicated data row
            var responses = _replicatedDataRepository.GetReplicationValues(surveyId, interviewId);

            // get all quota cells in survey
            var quotas = _quotaRepository.GetAll(surveyId);

            foreach (var quota in quotas)
            {
                var isFcdQuota = quota.IsFCD == 1;

                if (isFcdQuota)
                {
                    var interviewQuotaCell = interviewCells.First(x => x.QuotaID == quota.QuotaID);

                    var cell = _quotaCellRepository.TryGetById(surveyId, quota.QuotaID, interviewQuotaCell.CellID);
                    
                    var fields = new Dictionary<string, string>();
                    foreach (var name in quota.Data.FieldNames)
                    {
                        var value = responses.ContainsKey(name) ? responses[name] : null;

                        var fieldValue = cell.Data.FieldValues.Single(x => x.Field == name);
                        if (value != null && fieldValue.Value != value)
                        {
                            value = $"({value})";
                        }

                        fields.Add(name, value);
                    }

                    result.Add(new InterviewQuotaStatusItem
                    {
                        QuotaId = quota.QuotaID,
                        QuotaName = quota.Name,
                        IsFcdQuota = true,
                        IsZeroLimit = cell.CellID >= 0 && cell.Limit == 0,
                        Fields = fields,
                        IsNormalCell = cell.CellID >= 0,
                        HasEmptyAnswers = fields.ContainsValue(null),
                        IsOpen = cell.IsOpen
                    });
                }
                else
                {
                    result.Add(new InterviewQuotaStatusItem
                    {
                        QuotaId = quota.QuotaID,
                        QuotaName = quota.Name,
                        IsFcdQuota = false,
                        IsNormalCell = true,
                    });
                }
            }

            return result;
        }
    }
}