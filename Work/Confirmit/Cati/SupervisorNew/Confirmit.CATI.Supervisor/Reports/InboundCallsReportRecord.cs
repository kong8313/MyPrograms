using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Timezones;

namespace Confirmit.CATI.Supervisor.Reports
{
    public class InboundCallsReportRecord : BvSpGetInboundCallsReport_ListPageEntity
    {
        public string OperationTitle { get; }

        public InboundCallsReportRecord(BvSpGetInboundCallsReport_ListPageEntity entity, bool hidePii, string operationTitle, int? localTzId) : base()
        {
            ID = entity.ID;              
            SurveySID = entity.SurveySID;
            ProjectID = entity.ProjectID;
            ProjectName = entity.ProjectName;
            InboundNumber = entity.InboundNumber;
            RespondentNumber = hidePii ? "***" : entity.RespondentNumber;
            InterviewId = entity.InterviewId;
            OperationTitle = operationTitle;

            if (localTzId.HasValue)
            {
                EventDate = TimezoneManager.ConvertToTzLocalTime(localTzId.Value, entity.EventDate.Value);
            }
            else
            {
                EventDate = entity.EventDate;
            }
        }
    }
}
