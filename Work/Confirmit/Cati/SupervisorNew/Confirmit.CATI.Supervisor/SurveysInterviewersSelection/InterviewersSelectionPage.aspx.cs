using System;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;

namespace Confirmit.CATI.Supervisor.SurveysInterviewersSelection
{
    public partial class InterviewersSelectionPage : BaseForm
    {
        [StoreInViewState]
        public SourceList SourceList;

        [StoreInViewState]
        public int? SelectedInterviewerId;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                SourceList = (SourceList)Int32.Parse(Request.Params["SourceList"]);

                if (String.IsNullOrEmpty(Request.Params["SelectedId"]) == false)
                    SelectedInterviewerId = Int32.Parse(Request.Params["SelectedId"]);

                InitSelectedInterviewersIds();
            }              
        }

        private void InitSelectedInterviewersIds()
        {
            int[] ids;

            switch (SourceList)
            {
                case SourceList.InterviewerList:
                    ids = SessionVariables.TaskListSelectedInterviewersIds;                    
                    break;
                case SourceList.PerformanceList:
                    ids = SessionVariables.PerformanceListSelectedInterviewersIds;                    
                    break;                
                case SourceList.SurveyOverviewReport:
                    ids =  ReportsSessionVariables.SurveyOverviewReportSelectedInterviewersIds;
                    break;
                case SourceList.CatiProductivityReport:
                    ids = ReportsSessionVariables.CatiProductivityReportSelectedInterviewersIds;
                    break;
                case SourceList.ProductivityReport:
                    ids = ReportsSessionVariables.ProductivityReportSelectedInterviewersIds;
                    break;
                case SourceList.SampleStatusSummary:
                    ids = ReportsSessionVariables.SampleStatusSummaryReportSelectedInterviewersIds;
                    break;
                case SourceList.InterviewerBreakReport:
                    ids = ReportsSessionVariables.InterviewerSessionsReportSelectedInterviewersIds;
                    break;
                case SourceList.AlertsHistoryReport:
                    ids = ReportsSessionVariables.AlertsHistoryReportSelectedInterviewersIds;
                    break;
                case SourceList.AlertsHistoryAggregatedReport:
                    ids = ReportsSessionVariables.AlertsHistoryAggregatedReportSelectedInterviewersIds;
                    break;

                default:
                    throw new NotSupportedException("Not supported list");
            }

            doubleGrid.SelectedIds = ids;            
        }

        protected void Save(object sender, EventArgs e)
        {            
            int[] ids = doubleGrid.SelectedIds;

            switch (SourceList)
            {
                case SourceList.InterviewerList:
                    SessionVariables.TaskListSelectedInterviewersIds = ids;
                    break;         
                case SourceList.PerformanceList:
                    SessionVariables.PerformanceListSelectedInterviewersIds = ids;
                    break;
                case SourceList.SurveyOverviewReport:
                    ReportsSessionVariables.SurveyOverviewReportSelectedInterviewersIds = ids;
                    break;
                case SourceList.CatiProductivityReport:
                    ReportsSessionVariables.CatiProductivityReportSelectedInterviewersIds = ids;
                    break;
                case SourceList.ProductivityReport:
                    ReportsSessionVariables.ProductivityReportSelectedInterviewersIds = ids;
                    break;
                case SourceList.SampleStatusSummary:
                    ReportsSessionVariables.SampleStatusSummaryReportSelectedInterviewersIds = ids;
                    break;
                case SourceList.InterviewerBreakReport:
                    ReportsSessionVariables.InterviewerSessionsReportSelectedInterviewersIds = ids;
                    break;
                case SourceList.AlertsHistoryReport:
                    ReportsSessionVariables.AlertsHistoryReportSelectedInterviewersIds = ids;
                    break;
                case SourceList.AlertsHistoryAggregatedReport:
                    ReportsSessionVariables.AlertsHistoryAggregatedReportSelectedInterviewersIds = ids;
                    break; 
                default:
                    throw new NotSupportedException("Not supported list");
            }       

            CloseOverlay(true);
        }
    }
}