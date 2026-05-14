using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport
{
    public class StandardColumnsProvider
    {
        public const string PersonColumnName = "PersonName";

        public bool IsStandardColumn(string standardColumnName)
        {
            var columns = GetStandardColumns();

            return columns.Any(x => x.FieldName == standardColumnName);
        }

        public List<ReportColumnInfo> GetStandardColumns()
        {
            return new List<ReportColumnInfo>
            {
                new ReportColumnInfo("PersonId", "User ID", "Total:") { ValueFormat = string.Empty },
                new ReportColumnInfo(PersonColumnName, "User name", "= Count(Fields.PersonId)") { ValueFormat = string.Empty },
                new ReportColumnInfo("DisplayName", "Display name", ""),
                new ReportColumnInfo("Attribute1", "Attribute1", ""),
                new ReportColumnInfo("Attribute2", "Attribute2", ""),
                new ReportColumnInfo("Attribute3", "Attribute3", ""),
                new ReportColumnInfo("Attribute4", "Attribute4", ""),
                new ReportColumnInfo("Attribute5", "Attribute5", ""),
                new ReportColumnInfo("LogOnHours", "Log on time  (hours)"),
                new ReportColumnInfo("WaitingHours", "Waiting time (hours)"),
                new ReportColumnInfo("BreakHoursPaid", "Paid break time (hours)"),
                new ReportColumnInfo("BreakHoursUnpaid", "Unpaid break time (hours)"),
                new ReportColumnInfo("OpenEndReviewHours", "Review Time (hours)"),
                new ReportColumnInfo("PreviewTime", "Preview time (hours)"),
                new ReportColumnInfo("WrapTime", "Wrap time (hours)"),
                new ReportColumnInfo("InterviewTime", "Interview time (hours)"),
                new ReportColumnInfo("ConnectedTime", "Connected time (hours)"),
                new ReportColumnInfo("DialingsCount", "Interviews") { ValueFormat = string.Empty },
                new ReportColumnInfo("DialingsPerLogOnHours", "Interviews per log on hour", "= IIf(Sum(Fields.LogOnHoursWithoutBreaks) <> 0, CDbl(Sum(Fields.DialingsCount)) / Sum(Fields.LogOnHoursWithoutBreaks), 0)"),
                new ReportColumnInfo("Completes", "Completes") { ValueFormat = string.Empty },
                new ReportColumnInfo("CompletesPerLogOnHours", "Completes per log on hour", "= IIf(Sum(Fields.LogOnHoursWithoutBreaks) <> 0, CDbl(Sum(Fields.Completes)) / Sum(Fields.LogOnHoursWithoutBreaks), 0)"),
                new ReportColumnInfo("DialingsPerComplete", "Interviews per complete", "= IIf(Sum(Fields.Completes) <> 0, CDbl(Sum(Fields.DialingsCount)) / Sum(Fields.Completes), 0)"),
                new ReportColumnInfo("AverageDuration", "Average completed interview length (min)", "= Confirmit.CATI.Core.Reports.TelerikReportsCustomFunctions.DivideWithZeroCheck(CDbl(IsNull(Sum(Fields.AverageCompletedInterviewDuration * Fields.Completes), 0)), CDbl(IsNull(Sum(Fields.Completes), 0))) / 60")
            };
        }
    }
}
