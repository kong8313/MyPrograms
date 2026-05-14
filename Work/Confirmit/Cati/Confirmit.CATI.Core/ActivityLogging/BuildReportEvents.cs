using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class BuildSurveyProductivityReportEventParameters : ManagementActivityEventDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int[] SurveyIds { get; set; }
        public string[] StateIds { get; set; }
        public int[] PersonIds { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.BuildSurveyProductivityReport)]
    public class BuildSurveyProductivityReportEvent : ManagementActivityEvent<BuildSurveyProductivityReportEventParameters>
    {
        public BuildSurveyProductivityReportEvent(
            DateTime startDate,
            DateTime endDate,
            IEnumerable<int> surveyIds,
            IEnumerable<string> stateIds,
            IEnumerable<int> personIds
            ):
            base(ManagementEventCategory.Report, ManagementEvent.BuildSurveyProductivityReport)
        {
            if (surveyIds.Count() == 1)
            {
                ObjectId = surveyIds.First();
                ObjectName = SurveyRepository.GetById(ObjectId).Name;
            }

            Details = new BuildSurveyProductivityReportEventParameters
            {
                StartDate = startDate,
                EndDate = endDate,
                SurveyIds = surveyIds != null ? surveyIds.ToArray() : new int[0],
                StateIds = stateIds != null ? stateIds.ToArray() : new string[0],
                PersonIds = personIds != null ? personIds.ToArray() : new int[0]
            };
        }
    }

    [Serializable]
    public class BuildSurveySummaryReportEventParameters : ManagementActivityEventDetails
    {
        public string StateIds { get; set; }
        public string PersonIds { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.BuildSurveySummaryReport)]
    public class BuildSurveySummaryReportEvent : ManagementActivityEvent<BuildSurveySummaryReportEventParameters>
    {
        public BuildSurveySummaryReportEvent(
            int surveyId,
            string personIds,
            string stateIds):
            base(ManagementEventCategory.Report, ManagementEvent.BuildSurveySummaryReport)
        {
            ObjectId = surveyId;
            ObjectName = SurveyRepository.GetById(ObjectId).Name;

            Details = new BuildSurveySummaryReportEventParameters
            {
                StateIds = stateIds,
                PersonIds = personIds
            };
        }
    }

    [Serializable]
    public class BuildInterviewerProductivityReportEventParameters : ManagementActivityEventDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int[] SurveyIds { get; set; }
        public int[] StateIds { get; set; }
        public int[] PersonIds { get; set; }
        public bool ShowDialerAttempts { get; set; }
        public bool HideEmpty { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.BuildInterviewerProductivityReport)]
    public class BuildInterviewerProductivityReportEvent : ManagementActivityEvent<BuildInterviewerProductivityReportEventParameters>
    {
        public BuildInterviewerProductivityReportEvent(
            IEnumerable<int> surveyIds,
            DateTime startDate,
            DateTime endDate,
            IEnumerable<int> stateIds,
            IEnumerable<int> personIds,
            bool showDialerAttempts,
            bool hideEmpty):
            base(ManagementEventCategory.Report, ManagementEvent.BuildInterviewerProductivityReport)
        {
            if (surveyIds.Count() == 1)
            {
                ObjectId = surveyIds.First();
                ObjectName = SurveyRepository.GetById(ObjectId).Name;
            }

            Details = new BuildInterviewerProductivityReportEventParameters
            {
                StartDate = startDate,
                EndDate = endDate,
                SurveyIds = surveyIds != null ? surveyIds.ToArray() : new int[0],
                StateIds = stateIds != null ? stateIds.ToArray() : new int[0],
                PersonIds = personIds != null ? personIds.ToArray() : new int[0],
                ShowDialerAttempts = showDialerAttempts,
                HideEmpty = hideEmpty
            };
        }
    }

    [Serializable]
    public class BuildSurveyOverviewReportEventParameters : ManagementActivityEventDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int[] SurveyIds { get; set; }
        public int[] StateIds { get; set; }
        public int[] PersonIds { get; set; }
        public bool ShowDialerAttempts { get; set; }
        public bool HideEmpty { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.BuildSurveyOverviewReport)]
    public class BuildSurveyOverviewReportEvent : ManagementActivityEvent<BuildSurveyOverviewReportEventParameters>
    {
        public BuildSurveyOverviewReportEvent(
            IEnumerable<int> surveyIds,
            DateTime startDate,
            DateTime endDate,
            IEnumerable<int> stateIds,
            IEnumerable<int> personIds,
            bool showDialerAttempts,
            bool hideEmpty):
            base(ManagementEventCategory.Report, ManagementEvent.BuildSurveyOverviewReport)
        {
            if (surveyIds.Count() == 1)
            {
                ObjectId = surveyIds.First();
                ObjectName = SurveyRepository.GetById(ObjectId).Name;
            }

            Details = new BuildSurveyOverviewReportEventParameters
            {
                StartDate = startDate,
                EndDate = endDate,
                SurveyIds = surveyIds != null ? surveyIds.ToArray() : new int[0],
                StateIds = stateIds != null ? stateIds.ToArray() : new int[0],
                PersonIds = personIds != null ? personIds.ToArray() : new int[0],
                ShowDialerAttempts = showDialerAttempts,
                HideEmpty = hideEmpty
            };
        }
    }

    [Serializable]
    public class BuildAttemptsByDispositionReportEventParameters : ManagementActivityEventDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int[] StateIds { get; set; }
        public bool HideEmpty { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.BuildAttemptsByDispositionReport)]
    public class BuildAttemptsByDispositionReportEvent : ManagementActivityEvent<BuildAttemptsByDispositionReportEventParameters>
    {
        public BuildAttemptsByDispositionReportEvent(
            int surveyId,
            DateTime startDate,
            DateTime endDate,
            IEnumerable<int> stateIds,
            bool hideEmpty):
            base(ManagementEventCategory.Report, ManagementEvent.BuildAttemptsByDispositionReport)
        {
            ObjectId = surveyId;
            ObjectName = SurveyRepository.GetById(ObjectId).Name;

            Details = new BuildAttemptsByDispositionReportEventParameters
            {
                StartDate = startDate,
                EndDate = endDate,
                StateIds = stateIds != null ? stateIds.ToArray() : null,
                HideEmpty = hideEmpty
            };
        }
    }

    [Serializable]
    public class BuildNumberOfAttemptsReportEventParameters : ManagementActivityEventDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.BuildNumberOfAttemptsReport)]
    public class BuildNumberOfAttemptsReportEvent : ManagementActivityEvent<BuildNumberOfAttemptsReportEventParameters>
    {
        public BuildNumberOfAttemptsReportEvent(
            int surveyId,
            DateTime startDate,
            DateTime endDate):
            base(ManagementEventCategory.Report, ManagementEvent.BuildNumberOfAttemptsReport)
        {
            ObjectId = surveyId;
            ObjectName = SurveyRepository.GetById(ObjectId).Name;

            Details = new BuildNumberOfAttemptsReportEventParameters
            {
                StartDate = startDate,
                EndDate = endDate,
            };
        }
    }

    [ManagementEventAttribute(ManagementEvent.BuildAggregatedAlertsHistoryReport)]
    public class BuildAggregatedAlertsHistoryReportEvent : ManagementActivityEvent<BuildAggregatedAlertsHistoryReportEventParameters>
    {
        public BuildAggregatedAlertsHistoryReportEvent(
            int[] personIds,
            int[] surveyIds,
            DateTime startDate,
            DateTime endDate,
            byte? interviewState):
            base(ManagementEventCategory.Report, ManagementEvent.BuildAggregatedAlertsHistoryReport)
        {
            if (surveyIds != null && surveyIds.Any())
            {
                ObjectId = surveyIds.First();
                ObjectName = SurveyRepository.GetById(ObjectId).Name;
            }

            Details = new BuildAggregatedAlertsHistoryReportEventParameters
            {
                PersonIds = personIds,
                SurveyIds = surveyIds,
                InterviewState = interviewState,
                StartDate = startDate,
                EndDate = endDate,
            };
        }
    }

    [Serializable]
    public class BuildAggregatedAlertsHistoryReportEventParameters : ManagementActivityEventDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int[] PersonIds { get; set; }
        public int[] SurveyIds { get; set; }
        public byte? InterviewState { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.BuildAlertsHistoryReport)]
    public class BuildAlertsHistoryReportEvent : ManagementActivityEvent<BuildAlertsHistoryReportEventParameters>
    {
        public BuildAlertsHistoryReportEvent(
            int[] personIds,
            int[] surveyIds,
            PagingArgs pagingArgs):
            base(ManagementEventCategory.Report, ManagementEvent.BuildAlertsHistoryReport)
        {
            if (surveyIds != null && surveyIds.Any())
            {
                ObjectId = surveyIds.First();
                ObjectName = SurveyRepository.GetById(ObjectId).Name;
            }

            Details = new BuildAlertsHistoryReportEventParameters
            {
                PersonIds = personIds,
                SurveyIds = surveyIds,
                PagingArgs = pagingArgs,
            };
        }
    }

    [Serializable]
    public class BuildAlertsHistoryReportEventParameters : ManagementActivityEventDetails
    {
        public int[] PersonIds { get; set; }
        public int[] SurveyIds { get; set; }
        public PagingArgs PagingArgs { get; set; }
    }

    [Serializable]
    public class BuildInterviewerSessionsReportEventParameters : ManagementActivityEventDetails
    {
        public int[] PersonIds { get; set; }
        public PagingArgs PagingArgs { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.BuildInterviewerSessionsReport)]
    public class BuildInterviewerSessionsReportEvent : ManagementActivityEvent<BuildInterviewerSessionsReportEventParameters>
    {
        public BuildInterviewerSessionsReportEvent(
            int[] personIds,
            PagingArgs pagingArgs):
            base(ManagementEventCategory.Report, ManagementEvent.BuildInterviewerSessionsReport)
        {
            Details = new BuildInterviewerSessionsReportEventParameters
            {
                PersonIds = personIds,
                PagingArgs = pagingArgs
            };
        }
    }

    [Serializable]
    public class BuildCallAttemptLogEventParameters : ManagementActivityEventDetails
    {
        public string SupervisorName { get; set; }
        public int TimezoneId { get; set; }
        public PagingArgs PagingArgs { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.BuildCallAttemptLog)]
    public class BuildCallAttemptLogEvent : ManagementActivityEvent<BuildCallAttemptLogEventParameters>
    {
        public BuildCallAttemptLogEvent(
            string supervisorName,
            int timezoneId,
            PagingArgs pagingArgs
            ):
            base(ManagementEventCategory.Report, ManagementEvent.BuildCallAttemptLog)
        {
            Details = new BuildCallAttemptLogEventParameters
            {
                SupervisorName = supervisorName,
                TimezoneId = timezoneId,
                PagingArgs = pagingArgs
            };
        }
    }

    [Serializable]
    public class BuildInboundCallsReportEventParameters : ManagementActivityEventDetails
    {
        public string SupervisorName { get; set; }
        public int TimezoneId { get; set; }
        public PagingArgs PagingArgs { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.BuildInboundCallsReport)]
    public class BuildInboundCallsReportEvent : ManagementActivityEvent<BuildInboundCallsReportEventParameters>
    {
        public BuildInboundCallsReportEvent(
            string supervisorName,
            int timezoneId,
            PagingArgs pagingArgs
            ):
            base(ManagementEventCategory.Report, ManagementEvent.BuildInboundCallsReport)
        {
            Details = new BuildInboundCallsReportEventParameters
            {
                SupervisorName = supervisorName,
                TimezoneId = timezoneId,
                PagingArgs = pagingArgs
            };
        }
    }

    [Serializable]
    public class BuildTelerikReportEventParameters : ManagementActivityEventDetails
    {
        public string QueryString { get; set; }
        public string Referrer { get; set; }
    }

    [ManagementEvent(ManagementEvent.BuildTelerikReport)]
    public class BuildTelerikReportEvent : ManagementActivityEvent<BuildTelerikReportEventParameters>
    {
        public BuildTelerikReportEvent(string queryString, string referrer):
            base(ManagementEventCategory.Report, ManagementEvent.BuildTelerikReport)
        {
            Details = new BuildTelerikReportEventParameters
                      {
                          QueryString = queryString,
                          Referrer = referrer
                      };
        }
    }
}