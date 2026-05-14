using System.Collections.Generic;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;

namespace Confirmit.CATI.Supervisor.Classes
{
    public static class ReportsSessionVariables
    {                

        private static readonly SessionVariable<int[]> _surveyOverviewReportSelectedSurveysIds =
                                new SessionVariable<int[]>("SurveyOverviewReportSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _catiProductivityReportSelectedSurveysIds =
                                new SessionVariable<int[]>("CatiProductivityReportSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _sampleStatusSummaryReportSelectedSurveysIds =
                                new SessionVariable<int[]>("SampleStatusSummaryReportSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _attemptsByDispositionReportSelectedSurveysIds =
                                new SessionVariable<int[]>("AttemptsByDispositionReportSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _numberOfAttemptsReportSelectedSurveysIds =
                                new SessionVariable<int[]>("NumberOfAttemptsReportSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _productivityReportSelectedSurveysIds =
                                new SessionVariable<int[]>("ProductivityReportSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _surveyOverviewReportSelectedInterviewersIds =
                                new SessionVariable<int[]>("SurveyOverviewReportSelectedInterviewersIds");

        private static readonly SessionVariable<int[]> _catiProductivityReportSelectedInterviewersIds =
                        new SessionVariable<int[]>("CatiProductivityReportSelectedInterviewersIds");

        private static readonly SessionVariable<int[]> _sampleStatusSummaryReportSelectedInterviewersIds =
                        new SessionVariable<int[]>("SampleStatusSummaryReportSelectedInterviewersIds");

        private static readonly SessionVariable<int[]> _productivityReportSelectedInterviewersIds =
                        new SessionVariable<int[]>("ProductivityReportSelectedInterviewersIds");

        private static readonly SessionVariable<int[]> _interviewerSessionsReportSelectedInterviewersIds =
                       new SessionVariable<int[]>("InterviewerSessionsReportSelectedInterviewersIds");

        private static readonly SessionVariable<int[]> _alertsHistoryReportSelectedSurveysIds =
                     new SessionVariable<int[]>("AlertsHistoryReportSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _alertsHistoryReportSelectedInterviewersIds =
                     new SessionVariable<int[]>("AlertsHistoryReportSelectedInterviewersIds");

        private static readonly SessionVariable<int[]> _alertsHistoryAggregatedReportSelectedSurveysIds =
                     new SessionVariable<int[]>("AlertsHistoryAggregatedReportSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _alertsHistoryAggregatedReportSelectedInterviewersIds =
                     new SessionVariable<int[]>("AlertsHistoryAggregatedReportSelectedInterviewersIds");

        private static readonly SessionVariable<ShiftForReport> _shiftForInterviewerProductivityReportItem =
                     new SessionVariable<ShiftForReport>("ShiftForInterviewerProductivityReport");

        private static readonly SessionVariable<ShiftForReport> _shiftForSurveyOverviewReportItem =
             new SessionVariable<ShiftForReport>("ShiftForSurveyOverviewReport");

        private static readonly SessionVariable<ShiftForReport> _shiftForSurveyProductivityReportItem =
             new SessionVariable<ShiftForReport>("ShiftForSurveyProductivityReport");

        private static readonly SessionVariable<int[]> _sampleUtilisationReportSelectedSurveysIds =
                              new SessionVariable<int[]>("SampleUtilisationReportSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _sampleStatusSummaryByQuestionReportSelectedSurveysIds =
                      new SessionVariable<int[]>("_sampleStatusSummaryByQuestionReportSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _quotaProgressReportSelectedSurveysIds =
              new SessionVariable<int[]>("_quotaProgressReportSelectedSurveysIds");

        private static readonly SessionVariable<int[]> _inboundCallsReportSelectedSurveysIds =
              new SessionVariable<int[]>("_inboundCallsReportSelectedSurveysIds");

        public static int[] SurveyOverviewReportSelectedSurveysIds
        {
            get
            {
                return _surveyOverviewReportSelectedSurveysIds.Value;
            }
            set
            {
                _surveyOverviewReportSelectedSurveysIds.Value = value;
            }
        }

        public static int[] CatiProductivityReportSelectedSurveysIds
        {
            get
            {
                return _catiProductivityReportSelectedSurveysIds.Value;
            }
            set
            {
                _catiProductivityReportSelectedSurveysIds.Value = value;
            }
        }

        public static int[] SampleStatusSummaryReportSelectedSurveysIds
        {
            get
            {
                return _sampleStatusSummaryReportSelectedSurveysIds.Value;
            }
            set
            {
                _sampleStatusSummaryReportSelectedSurveysIds.Value = value;
            }
        }

        public static int[] AttemptsByDispositionReportSelectedSurveysIds
        {
            get
            {
                return _attemptsByDispositionReportSelectedSurveysIds.Value;
            }
            set
            {
                _attemptsByDispositionReportSelectedSurveysIds.Value = value;
            }
        }

        public static int[] NumberOfAttemptsReportSelectedSurveysIds
        {
            get
            {
                return _numberOfAttemptsReportSelectedSurveysIds.Value;
            }
            set
            {
                _numberOfAttemptsReportSelectedSurveysIds.Value = value;
            }
        }        

        public static int[] ProductivityReportSelectedSurveysIds
        {
            get
            {
                return _productivityReportSelectedSurveysIds.Value;
            }
            set
            {
                _productivityReportSelectedSurveysIds.Value = value;
            }
        }

        public static int[] SurveyOverviewReportSelectedInterviewersIds
        {
            get
            {
                return _surveyOverviewReportSelectedInterviewersIds.Value;
            }
            set
            {
                _surveyOverviewReportSelectedInterviewersIds.Value = value;
            }
        }

        public static int[] CatiProductivityReportSelectedInterviewersIds
        {
            get
            {
                return _catiProductivityReportSelectedInterviewersIds.Value;
            }
            set
            {
                _catiProductivityReportSelectedInterviewersIds.Value = value;
            }
        }

        public static int[] SampleStatusSummaryReportSelectedInterviewersIds
        {
            get
            {
                return _sampleStatusSummaryReportSelectedInterviewersIds.Value;
            }
            set
            {
                _sampleStatusSummaryReportSelectedInterviewersIds.Value = value;
            }
        }

        public static int[] InboundCallsReportSelectedSurveysIds
        {
            get
            {
                return _inboundCallsReportSelectedSurveysIds.Value;
            }
            set
            {
                _inboundCallsReportSelectedSurveysIds.Value = value;
            }
        }

        public static int[] ProductivityReportSelectedInterviewersIds
        {
            get
            {
                return _productivityReportSelectedInterviewersIds.Value;
            }
            set
            {
                _productivityReportSelectedInterviewersIds.Value = value;
            }
        }

        public static int[] InterviewerSessionsReportSelectedInterviewersIds
        {
            get
            {
                return _interviewerSessionsReportSelectedInterviewersIds.Value;
            }
            set
            {
                _interviewerSessionsReportSelectedInterviewersIds.Value = value;
            }
        }

        public static int[] AlertsHistoryReportSelectedSurveysIds
        {
            get
            {
                return _alertsHistoryReportSelectedSurveysIds.Value;
            }
            set
            {
                _alertsHistoryReportSelectedSurveysIds.Value = value;
            }
        }

        public static int[] AlertsHistoryReportSelectedInterviewersIds
        {
            get
            {
                return _alertsHistoryReportSelectedInterviewersIds.Value;
            }
            set
            {
                _alertsHistoryReportSelectedInterviewersIds.Value = value;
            }
        }

        public static int[] AlertsHistoryAggregatedReportSelectedSurveysIds
        {
            get
            {
                return _alertsHistoryAggregatedReportSelectedSurveysIds.Value;
            }
            set
            {
                _alertsHistoryAggregatedReportSelectedSurveysIds.Value = value;
            }
        }

        public static int[] AlertsHistoryAggregatedReportSelectedInterviewersIds
        {
            get
            {
                return _alertsHistoryAggregatedReportSelectedInterviewersIds.Value;
            }
            set
            {
                _alertsHistoryAggregatedReportSelectedInterviewersIds.Value = value;
            }
        }

        public static ShiftForReport ShiftForInterviewerProductivityReport
        {
            get
            {
                return _shiftForInterviewerProductivityReportItem.Value;
            }
            set
            {
                _shiftForInterviewerProductivityReportItem.Value = value;
            }
        }

        public static ShiftForReport ShiftForSurveyOverviewReport
        {
            get
            {
                return _shiftForSurveyOverviewReportItem.Value;
            }
            set
            {
                _shiftForSurveyOverviewReportItem.Value = value;
            }
        }

        public static ShiftForReport ShiftForSurveyProductivityReport
        {
            get
            {
                return _shiftForSurveyProductivityReportItem.Value;
            }
            set
            {
                _shiftForSurveyProductivityReportItem.Value = value;
            }
        }

        public static int[] SampleUtilisationReportSelectedSurveysIds
        {
            get
            {
                return _sampleUtilisationReportSelectedSurveysIds.Value;
            }
            set
            {
                _sampleUtilisationReportSelectedSurveysIds.Value = value;
            }
        }

        public static int[] SampleStatusSummaryByQuestionReportSelectedSurveysIds
        {
            get
            {
                return _sampleStatusSummaryByQuestionReportSelectedSurveysIds.Value;
            }
            set
            {
                _sampleStatusSummaryByQuestionReportSelectedSurveysIds.Value = value;
            }
        }

        public static int[] QuotaProgressReportSelectedSurveysIds
        {
            get
            {
                return _quotaProgressReportSelectedSurveysIds.Value;
            }
            set
            {
                _quotaProgressReportSelectedSurveysIds.Value = value;
            }
        }


    }
}
