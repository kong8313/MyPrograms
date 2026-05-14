using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    [Serializable]
    public class DailyCounterParameter : IExtraQuotaCounterParameters
    {
        public int SurveyId { get; private set; }

        public string[] QuotaFields { get; private set; }

        public int QuotaId { get; private set; }

        public int[] Its { get; private set; }

        public (DateTime startDate, DateTime endDate)? Period { get; private set; }
        public DailyCounterParameter(int surveyId, int quotaId, int[] its, string[] quotaFields, (DateTime,DateTime)? period = null)
        {
            SurveyId = surveyId;
            Its = its;
            QuotaFields = quotaFields;
            QuotaId = quotaId;
            Period = period;
        }
    }
}
