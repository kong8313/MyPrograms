using System;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;

namespace Confirmit.CATI.Core.Services
{
    public class QuotaBalancingConfigurationValidator : IQuotaBalancingConfigurationValidator
    {
        private readonly IQuotaInfoService _quotaInfoService;

        public QuotaBalancingConfigurationValidator(IQuotaInfoService quotaInfoService)
        {
            _quotaInfoService = quotaInfoService;
        }

        public void CheckConfiguration(int surveyId, QuotaBalancingConfiguration configuration)
        {
            if (configuration.PromotionPriority < 1)
            {
                throw new UserMessageException("The priority can't be less than 1.");
            }

            if (configuration.PromotionThreshold < 1)
            {
                throw new UserMessageException("The threshold can't be less than 1.");
            }

            QuotaInfo[] quotas;

            try
            {
                quotas = _quotaInfoService.GetQuotaInfos(surveyId);
            }
            catch (Exception ex)
            {
                Trace.TraceError("CheckQuotaBalancingParameters: {0}", ex);
                throw new UserMessageException("Survey was deleted.");
            }

            var balancedFields = configuration.Fields.Where(x => x.IsEnabled).ToArray();
            
            foreach (var balancedQuota in configuration.Quotas.Where(x => x.IsEnabled))
            {
                if(!balancedQuota.QuotaFieldIds.Any(quotaFieldId => balancedFields.Any(f => f.FieldId == quotaFieldId)))
                {
                    throw new UserMessageException($"Quota balancing configuration doesn't contain balanced fields for quota '{balancedQuota.QuotaName}'.");
                }
            }

            var availableFieldIds = configuration.Quotas.Where(x => x.IsEnabled)
                .SelectMany(quota => quota.QuotaFieldIds).Distinct().ToArray();

            foreach (var field in balancedFields)
            {
                if (!availableFieldIds.Contains(field.FieldId))
                {
                    throw new UserMessageException($"Quota balancing configuration doesn't contain balanced quotas for field '{field.FieldName}'.");
                }
            }
        }
    }
}