using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Microsoft.Practices.ObjectBuilder2;

namespace Confirmit.CATI.Core.Services.Survey.Quota
{
    public class QuotaBalancingService : IQuotaBalancingService
    {
        public const float DefaultPromotionCoefficient = (float)1.0;

        private readonly IQuotaBalancingSettings _quotaBalancingSettings;
        private readonly IQuotaBalancingConfigurationValidator _quotaBalancingConfigurationValidator;
        private readonly IQuotaBalancingRepository _quotaBalancingRepository;
        private readonly IQuotaInfoService _quotaInfoService;
        private readonly IContextInfoService _contextInfoService;
        private readonly IReplicationIndexService _replicationIndexService;
        private readonly ISurveyRepository _surveyRepository;

        public QuotaBalancingService(
            IQuotaBalancingSettings settings,
            IQuotaBalancingConfigurationValidator quotaBalancingConfigurationValidator,
            IQuotaBalancingRepository quotaBalancingRepository,
            IQuotaInfoService quotaInfoService,
            IContextInfoService contextInfoService,
            IReplicationIndexService replicationIndexService,
            ISurveyRepository surveyRepository)
        {
            _quotaBalancingSettings = settings;
            _quotaBalancingConfigurationValidator = quotaBalancingConfigurationValidator;
            _quotaBalancingRepository = quotaBalancingRepository;
            _quotaInfoService = quotaInfoService;
            _contextInfoService = contextInfoService;
            _replicationIndexService = replicationIndexService;
            _surveyRepository = surveyRepository;
        }

        internal int MaxCellsCount()
        {
            return _quotaBalancingSettings.MaxCellsCount;
        }

        internal int PromoteCell(int surveyId, int quotaId, int cellId, int promotedCallsCount, TimeSpan totalRunPeriod, int promotionPriority)
        {
            int result;

            using (new ConnectionScope())
            {
                _contextInfoService.WriteContextInfo(0, OperationType.PromoteCall, 0);
                BvSpPromoteCallsAdapter.ExecuteNonQuery(surveyId, quotaId, cellId, promotionPriority, promotedCallsCount, DateTime.UtcNow.Add(totalRunPeriod), out result);
            }
            return result;
        }

        public QuotaBalancingConfiguration GetQuotaBalancingConfiguration(int surveyId)
        {
            var quotas = _quotaInfoService.GetQuotaInfos(surveyId);
            var fields = quotas.SelectMany(quota => quota.Fields).Distinct().ToArray();
            var balancedQuotas = _quotaBalancingRepository.GetBalancedQuotasForSurvey(surveyId);
            var balancedFieldIds = _quotaBalancingRepository.GetBalancedFieldsForSurvey(surveyId).Select(f => f.ToLower()).ToArray();
            var firstQuota = balancedQuotas.FirstOrDefault();

            return new QuotaBalancingConfiguration()
            {
                Quotas = quotas.Select(quota => new QuotaBalancingConfiguration.Quota()
                {
                    QuotaId = quota.Id,
                    QuotaName = quota.Name,
                    QuotaFieldIds = quota.Fields.Select(x => x.ToLower()).ToArray(),
                    IsEnabled = balancedQuotas.Any(x => x.quotaId == quota.Id)
                }).ToArray(),
                Fields = fields.Select(fieldName => new QuotaBalancingConfiguration.Field()
                {
                    FieldId = fieldName.ToLower(),
                    FieldName = fieldName,
                    IsEnabled = balancedFieldIds.Contains(fieldName.ToLower())
                }).ToArray(),
                PromotionPriority = firstQuota?.priority ?? 500,
                PromotionThreshold = firstQuota?.promotionThreshold ?? 10,
                //PromotionCoefficient = balancedQuotas.Select(x => x.promotionCoefficient).FirstOrDefault(),
            };
        }

        public void SetQuotaBalancingConfiguration(int surveyId, QuotaBalancingConfiguration configuration)
        {
            var evt = new SetQuotaBalancingEvent(surveyId, _surveyRepository.GetById(surveyId).Name, configuration);

            _quotaBalancingConfigurationValidator.CheckConfiguration(surveyId, configuration);

            var quotas = configuration.Quotas.Where(x => x.IsEnabled).Select(q => new BvQuotaBalancingEntity()
            {
                surveyId = surveyId,
                quotaId = q.QuotaId,
                quotaName = q.QuotaName,
                priority = configuration.PromotionPriority,
                promotionCoefficient = DefaultPromotionCoefficient,
                promotionThreshold = configuration.PromotionThreshold
            });

            var selectedFields = configuration.Fields.Where(f => f.IsEnabled).ToArray();

            _quotaBalancingRepository.SetBalancedQuotasForSurvey(surveyId, quotas, selectedFields.Select(x => x.FieldName));

            foreach (var quota in configuration.Quotas.Where(x => x.IsEnabled))
            {
                var filterFields = selectedFields.Where(field => quota.QuotaFieldIds.Contains(field.FieldId)).ToArray();
                _replicationIndexService.ChangeOrderOfIndexColumns(surveyId, quota.QuotaId, filterFields.Select(x => x.FieldName).ToArray());
            }

            evt.Finish();
        }


        public void ResetQuotaBalancingConfiguration(int surveyId)
        {
            var evt = new ResetQuotaBalancingEvent(surveyId, _surveyRepository.GetById(surveyId).Name);

            _quotaBalancingRepository.SetBalancedQuotasForSurvey(surveyId, null, null);

            evt.Finish();
        }

        public void AdjustQuotaBalancingConfiguration(int surveyId, IEnumerable<TableInfo> tables)
        {
            var actualReplicationColumns = tables.SelectMany(x => x.ReplicationColumns);
            var actualQuotas = _quotaInfoService.GetQuotaInfos(surveyId);
            var actualReplicationFields = actualReplicationColumns.Select(x => x.Name.ToLower()).ToArray();

            var currentBalancedQuotas = _quotaBalancingRepository.GetBalancedQuotasForSurvey(surveyId);
            var currentBalancedFields = _quotaBalancingRepository.GetBalancedFieldsForSurvey(surveyId).Select(x => x.ToLower()).ToArray();

            currentBalancedFields = currentBalancedFields.Intersect(actualReplicationFields).ToArray();

            var newBalancedQuotas = new List<BvQuotaBalancingEntity>();
            var newBalancedFields = new HashSet<string>();

            foreach (var balancedQuota in currentBalancedQuotas)
            {
                var actualQuotaInfo = actualQuotas.SingleOrDefault(x => x.Name == balancedQuota.quotaName);
                if (actualQuotaInfo == null)
                {
                    continue;
                }

                var actualBalancedQuotaField = currentBalancedFields.Intersect(actualQuotaInfo.Fields).ToArray();
                if (!actualBalancedQuotaField.Any())
                {
                    continue;
                }

                //we need tp update quota id because it can be changes during launch survey operation

                balancedQuota.quotaId = actualQuotaInfo.Id;

                newBalancedQuotas.Add(balancedQuota);

                actualBalancedQuotaField.ForEach(field => newBalancedFields.Add(field));
            }

            _quotaBalancingRepository.SetBalancedQuotasForSurvey(surveyId, newBalancedQuotas, newBalancedFields);
        }
    }
}
