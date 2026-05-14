using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data
{
    public class QuotaBalancingConfiguration
    {
        public class Quota
        {
            public int QuotaId { get; set; }
            public string QuotaName { get; set; }
            public bool IsEnabled { get; set; }
            public string[] QuotaFieldIds { get; set; }
        }

        public class Field
        {
            public string FieldId { get; set; }
            public string FieldName { get; set; }
            public bool IsEnabled { get; set; }
        }

        public Quota[] Quotas { get; set; }
        public Field[] Fields { get; set; }

        public int PromotionPriority { get; set; }
        public int PromotionThreshold { get; set; }
    }
}
