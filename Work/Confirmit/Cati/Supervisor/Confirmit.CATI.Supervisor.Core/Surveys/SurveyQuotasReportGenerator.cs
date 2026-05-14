using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
    public class SurveyQuotasReportGenerator
    {
        private readonly ISurveyQuotasExportInfoProvider _infoProvider;
        private readonly Func<string> _getReportTime;
        private readonly Func<int, string> _getSurveyName;

        public SurveyQuotasReportGenerator(ISurveyQuotasExportInfoProvider infoProvider,
                                           Func<string> getReportTime,
                                           Func<int, string> getSurveyName)
        {
            _infoProvider = infoProvider;
            _getReportTime = getReportTime;
            _getSurveyName = getSurveyName;
        }

        public string Generate()
        {
            var builder = new StringBuilder();

            AddHeader(builder);

            foreach (var quotaName in _infoProvider.GetQuotaNames())
            {
                AddEmptyLine(builder);
                AddQuotaName(builder, quotaName);
                AddQuotaBody(builder, quotaName); 
            }

            return builder.ToString();
        }

        private void AddEmptyLine(StringBuilder builder)
        {
            builder.Append(Environment.NewLine);
        }

        private void AddHeader(StringBuilder builder)
        {
            builder.AppendLine(Strings.QuotaStatusReport);
            builder.AppendLine(String.Format(Strings.QuotaStatusReport_Survey, _getSurveyName(_infoProvider.SurveyId)));
            builder.AppendLine(String.Format(Strings.QuotaStatusReport_ReportDate, _getReportTime()));
        }

        private void AddQuotaName(StringBuilder builder, string quotaName)
        {
            builder.AppendLine(String.Format(Strings.QuotaStatusReport_QuotaName, quotaName));
        }

        private void AddQuotaBody(StringBuilder builder, string quotaName)
        {            
            var details = _infoProvider.GetQuotaInfo(quotaName);

            var columns = details.Columns.Cast<DataColumn>().ToArray();

            var data = new List<String[]> { columns.Select(x => ResourceWrapper.Instance.GetString(x.Caption)).ToArray() };

            data.AddRange(details.Rows.Cast<DataRow>().Select(r => columns.Select(c => (r[c] is double) ? 
                                                                                        ((double)r[c]).ToString("N1") : 
                                                                                        r[c].ToString()).ToArray()));

            builder.AppendLine(DsvManager.ExportToDsv(data, "\t", x => x));
        }        
    }
}
