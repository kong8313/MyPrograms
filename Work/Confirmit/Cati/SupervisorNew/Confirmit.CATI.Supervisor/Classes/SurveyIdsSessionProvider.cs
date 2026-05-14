using System;
using Confirmit.CATI.Supervisor.Classes.Activity;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class SurveyIdsSessionProvider
    {
        public void SetSelectedSurveyIds(SourceList sourceList, int[] ids)
        {
            switch (sourceList)
            {
                case SourceList.InterviewerList:
                    SessionVariables.TaskListSelectedSurveysIds = ids;
                    break;
                case SourceList.SurveysList:
                    SessionVariables.SurveysActivityViewSelectedSurveysIds = ids;
                    break;
                case SourceList.AppointmentsList:
                    SessionVariables.AppointmentsListSelectedSurveysIds = ids;
                    break;
                case SourceList.PerformanceList:
                    SessionVariables.PerformanceListSelectedSurveysIds = ids;
                    break;
                case SourceList.SurveyOverviewReport:
                    ReportsSessionVariables.SurveyOverviewReportSelectedSurveysIds = ids;
                    break;
                case SourceList.CatiProductivityReport:
                    ReportsSessionVariables.CatiProductivityReportSelectedSurveysIds = ids;
                    break;
                case SourceList.SampleStatusSummary:
                    ReportsSessionVariables.SampleStatusSummaryReportSelectedSurveysIds = ids;
                    break;
                case SourceList.AttemptsByDespositionReport:
                    ReportsSessionVariables.AttemptsByDispositionReportSelectedSurveysIds = ids;
                    break;
                case SourceList.NumberOfAttemptsReport:
                    ReportsSessionVariables.NumberOfAttemptsReportSelectedSurveysIds = ids;
                    break;
                case SourceList.ProductivityReport:
                    ReportsSessionVariables.ProductivityReportSelectedSurveysIds = ids;
                    break;
                case SourceList.AlertsHistoryReport:
                    ReportsSessionVariables.AlertsHistoryReportSelectedSurveysIds = ids;
                    break;
                case SourceList.AlertsHistoryAggregatedReport:
                    ReportsSessionVariables.AlertsHistoryAggregatedReportSelectedSurveysIds = ids;
                    break;
                case SourceList.SampleUtilisationReport:
                    ReportsSessionVariables.SampleUtilisationReportSelectedSurveysIds = ids;
                    break;
                case SourceList.SampleStatusSummaryByQuestionReport:
                    ReportsSessionVariables.SampleStatusSummaryByQuestionReportSelectedSurveysIds = ids;
                    break;
                case SourceList.InboundCallsReport:
                    ReportsSessionVariables.InboundCallsReportSelectedSurveysIds = ids;
                    break;
                case SourceList.QuotaProgressReport:
                    ReportsSessionVariables.QuotaProgressReportSelectedSurveysIds = ids;
                    break;
                default:
                    throw new NotSupportedException("Not supported list");
            }
        }

        public int[] GetSelectedSurveys(SourceList sourceList, int? selectedSurveyId)
        {
            int[] ids;
            switch (sourceList)
            {
                case SourceList.InterviewerList:
                    ids = SessionVariables.TaskListSelectedSurveysIds;
                    break;
                case SourceList.SurveysList:
                    ids = SessionVariables.SurveysActivityViewSelectedSurveysIds;
                    break;
                case SourceList.AppointmentsList:
                    ids = SessionVariables.AppointmentsListSelectedSurveysIds;
                    break;
                case SourceList.PerformanceList:
                    ids = SessionVariables.PerformanceListSelectedSurveysIds;
                    break;
                case SourceList.SurveyOverviewReport:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.SurveyOverviewReportSelectedSurveysIds;
                    break;
                case SourceList.InboundCallsReport:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.InboundCallsReportSelectedSurveysIds;
                    break;
                case SourceList.CatiProductivityReport:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.CatiProductivityReportSelectedSurveysIds;
                    break;
                case SourceList.SampleStatusSummary:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.SampleStatusSummaryReportSelectedSurveysIds;
                    break;
                case SourceList.AttemptsByDespositionReport:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.AttemptsByDispositionReportSelectedSurveysIds;
                    break;
                case SourceList.NumberOfAttemptsReport:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.NumberOfAttemptsReportSelectedSurveysIds;
                    break;
                case SourceList.ProductivityReport:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.ProductivityReportSelectedSurveysIds;
                    break;
                case SourceList.AlertsHistoryReport:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.AlertsHistoryReportSelectedSurveysIds;
                    break;
                case SourceList.AlertsHistoryAggregatedReport:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.AlertsHistoryAggregatedReportSelectedSurveysIds;
                    break;
                case SourceList.SampleUtilisationReport:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.SampleUtilisationReportSelectedSurveysIds;
                    break;
                case SourceList.SampleStatusSummaryByQuestionReport:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.SampleStatusSummaryByQuestionReportSelectedSurveysIds;
                    break;
                case SourceList.QuotaProgressReport:
                    ids = selectedSurveyId.HasValue ? new[] { selectedSurveyId.Value } : ReportsSessionVariables.QuotaProgressReportSelectedSurveysIds;
                    break;

                default:
                    throw new NotSupportedException("Not supported list");
            }

            return ids;
        }
    }
}