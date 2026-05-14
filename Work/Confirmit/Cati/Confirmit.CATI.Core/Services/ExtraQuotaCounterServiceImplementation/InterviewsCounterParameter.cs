using System;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    [Serializable]
    public class InterviewsCounterParameter : IExtraQuotaCounterParameters
    {
        public  int SurveyId { get; private set; }

        public int QuotaId { get; private set; }

        public string[] QuotaFields { get; private set; }

        public int[] Its { get; private set; }

        public InterviewsCounterParameter(int surveyId, int quotaId, int[] its, string[] quotaFields)
        {
            this.SurveyId = surveyId;
            this.QuotaId = quotaId;
            this.QuotaFields = quotaFields;
            this.Its = its;
        }
    }
}
