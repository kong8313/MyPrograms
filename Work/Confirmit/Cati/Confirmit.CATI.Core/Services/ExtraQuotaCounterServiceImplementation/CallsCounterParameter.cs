using System;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    [Serializable]
    public class CallsCounterParameter : IExtraQuotaCounterParameters
    {
        public  int SurveyId { get; private set; }
        
        public int QuotaId { get; private set; }

        public string[] QuotaFields { get; private set; }

        public bool IncludeDisabledCalls { get; private set; }

        public int[] Its { get; private set; }

        public CallsCounterParameter(int surveyId, int quotaId, bool includeDisabledCalls, int[] its, string[] quotaFields)
        {
            this.SurveyId = surveyId;
            QuotaId = quotaId;
            this.QuotaFields = quotaFields;
            this.IncludeDisabledCalls = includeDisabledCalls;
            this.Its = its;
        }
    }
}
