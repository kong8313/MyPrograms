using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Misc;
using Newtonsoft.Json.Linq;

namespace Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport
{
    public class JsonProductivityReportTemplateColumnConverter : JsonCreationConverter<ProductivityReportTemplateColumn>
    {
        protected override ProductivityReportTemplateColumn Create(Type objectType, JObject jsonObject)
        {
            if (jsonObject["$type"] == null)
            {
                return new ProductivityReportTemplateColumn();
            }

            return new ProductivityReportTemplateColumnWithStatuses();
        }
    }
}
