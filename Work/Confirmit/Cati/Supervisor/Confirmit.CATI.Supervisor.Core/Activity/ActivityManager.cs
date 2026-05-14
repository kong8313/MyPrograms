using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Tasks;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Supervisor.Core.Resources;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
    /// <summary>
    /// Class is responsible for operations with activity views .
    /// </summary>
    public class ActivityManager : IActivityManager
    {
        private readonly ISupervisorServiceClient _supervisorServiceClient;
        private readonly IPersonRepository _personRepository;
        private readonly ITaskRepository _taskRepository;

        public ActivityManager(ISupervisorServiceClient supervisorServiceClient, IPersonRepository personRepository, ITaskRepository taskRepository)
        {
            _supervisorServiceClient = supervisorServiceClient;
            _personRepository = personRepository;
            _taskRepository = taskRepository;
        }

        /// <summary>
        /// Dictionary contains modificators on alerts' red and amber values. 
        /// They should be divided on the dictionary values after receiving from Fusion
        /// and multiplied before sending back.
        /// </summary>
        private static readonly Dictionary<BvThresholdType, int> AlertsModificators = new Dictionary<BvThresholdType, int>
        {
            {BvThresholdType.MinutesSpentWorkingOnSurveyAlert, 60}
        };

        /// <summary>
        /// List contains threshold types used in survey list.
        /// </summary>
        public static readonly List<BvThresholdType> SurveyListThresholdTypes = new List<BvThresholdType>
        {
            BvThresholdType.InterviewersLoggedCountAlert,
            BvThresholdType.NextAppointmentTimeAlert,
            BvThresholdType.ScheduledCallsCountAlert,
            BvThresholdType.SuspendedCallsCountAlert,
            BvThresholdType.MinutesSpentWorkingOnSurveyAlert,
            BvThresholdType.AssignedInterviewersCountAlert,
            BvThresholdType.StrikeRateAlert,
            BvThresholdType.CountCallsAlert
        };

        /// <summary>
        /// List contains threshold types used in task list.
        /// </summary>
        public static readonly List<BvThresholdType> TaskListThresholdTypes = new List<BvThresholdType>
        {
            BvThresholdType.LastSubmissionAlert,
            BvThresholdType.LastKeepAliveTimeAlert,
            BvThresholdType.QuickAnswerSubmissionAlert,
            BvThresholdType.NoActivityAlert,
            BvThresholdType.InterviewDurationAlert,
            BvThresholdType.BreakDurationAlert,
        };

        /// <summary>
        /// Gets general survey-specific CATI activity data from Fusion.
        /// </summary>
        /// <param name="sortExpression">Database name of column the list is currently sorted on.</param>
        /// <param name="sortOrderAsc">Sorting direction (ascending or descending).</param>
        /// <param name="showOnlyActiveSurveys">If true only surveys that have people working on them appear.</param>
        /// <param name="surveys">The surveys.</param>
        /// <param name="its">Array of its (up to 5) to include in query</param>
        /// <returns>List of survey activity data objects.</returns>
        public List<SurveyActivityInfo> GetSurveyActivityData(
             string sortExpression,
             bool sortOrderAsc,
             bool showOnlyActiveSurveys,
             IEnumerable<int> surveys,
             bool onlyCatiInterviews,
             params int[] its
        )
        {
            var source = new List<SurveyActivityInfo>();

            var its1 = its.Length > 0 ? its[0] : 0;
            var its2 = its.Length > 1 ? its[1] : 0;
            var its3 = its.Length > 2 ? its[2] : 0;
            var its4 = its.Length > 3 ? its[3] : 0;
            var its5 = its.Length > 4 ? its[4] : 0;

            foreach (BvSpGetSurveyActivityWithAlertsEntity entity in
                GetActivityData(surveys,
                                batchId => BvSpGetSurveyActivityWithAlertsAdapter.ExecuteEntityList(batchId, showOnlyActiveSurveys, its1, its2, its3, its4, its5, onlyCatiInterviews)))
            {
                var info = new SurveyActivityInfo
                {
                    SID = entity.SurveySID.Value,
                    Id = entity.ProjectID,
                    Name = entity.SurveyName,
                    LoggedCount = entity.InterviewersLoggedCount.Value,
                    AssignedCount = entity.AssignedInterviewersCount.Value,
                    SampleSize = entity.TotalSampleSize.Value,
                    TotalTime = TimeSpan.FromSeconds(entity.MinutesSpentWorkingOnSurvey.Value),
                    TotalTimeToday = TimeSpan.FromSeconds(entity.MinutesSpentWorkingOnSurveyInDay.Value),
                    NextAppointment = entity.NextAppointmentTime as DateTime?,
                    ScheduledCallsCount = entity.ScheduledCallsCount.Value,
                    SuspendedCallsCount = entity.SuspendedCallsCount.Value,
                    StrikeRate = entity.StrikeRate.Value,
                    StrikeRate1h = entity.StrikeRate1h.Value,
                    CountCalls = entity.CountCalls.Value,
                    CountCalls1h = entity.CountCalls1h.Value,
                    InterviewDuration = TimeSpan.FromSeconds(entity.AvgDuration.Value),
                    InterviewDuration1h = TimeSpan.FromSeconds(entity.AvgDuration1h.Value),
                    Target = entity.Target,
                    CustomIts1 = entity.CustomITS1_Cnt,
                    CustomIts2 = entity.CustomITS2_Cnt,
                    CustomIts3 = entity.CustomITS3_Cnt,
                    CustomIts4 = entity.CustomITS4_Cnt,
                    CustomIts5 = entity.CustomITS5_Cnt
                };

                info.AlertStatuses.Add("LoggedCount", (AlertStatus)entity.AlertStatusOfInterviewersLoggedCount.Value);
                info.AlertStatuses.Add("NextAppointment", (AlertStatus)entity.AlertStatusOfNextAppointmentTime.Value);
                info.AlertStatuses.Add("ScheduledCallsCount", (AlertStatus)entity.AlertStatusOfScheduledCallsCount.Value);
                info.AlertStatuses.Add("SuspendedCallsCount", (AlertStatus)entity.AlertStatusOfSuspendedCallsCount.Value);
                info.AlertStatuses.Add("TimeSpent", (AlertStatus)entity.AlertStatusOfMinutesSpentWorkingOnSurvey.Value);
                info.AlertStatuses.Add("AssignedCount", (AlertStatus)entity.AlertStatusOfAssignedInterviewersCount.Value);
                info.AlertStatuses.Add("StrikeRate", (AlertStatus)entity.AlertStatusOfStrikeRate.Value);
                info.AlertStatuses.Add("StrikeRate1h", (AlertStatus)entity.AlertStatusOfStrikeRate1h.Value);
                info.AlertStatuses.Add("CountCalls", (AlertStatus)entity.AlertStatusOfCountCalls.Value);
                info.AlertStatuses.Add("CountCalls1h", (AlertStatus)entity.AlertStatusOfCountCalls1h.Value);
                info.AlertStatuses.Add("MaxStatusOfITSAlerts", (AlertStatus)entity.MaxStatusOfITSAlerts.Value);
                info.AlertStatuses.Add("CustomIts1", (AlertStatus)entity.CustomITS1_Alert.Value);
                info.AlertStatuses.Add("CustomIts2", (AlertStatus)entity.CustomITS2_Alert.Value);
                info.AlertStatuses.Add("CustomIts3", (AlertStatus)entity.CustomITS3_Alert.Value);
                info.AlertStatuses.Add("CustomIts4", (AlertStatus)entity.CustomITS4_Alert.Value);
                info.AlertStatuses.Add("CustomIts5", (AlertStatus)entity.CustomITS5_Alert.Value);

                source.Add(info);
            }

            //Sort resulting rowset on alert status and sorted column in grid.
            List<SortingArgs> sortingArgs = new List<SortingArgs>();
            sortingArgs.Add(new SortingArgs("Alert", false));
            if (!String.IsNullOrEmpty(sortExpression))
                sortingArgs.Add(new SortingArgs(sortExpression, sortOrderAsc));
            source.Sort(new CommonMultiComparer<SurveyActivityInfo>(sortingArgs));

            return source;
        }

        public List<TaskActivityInfo> GetTasksActivityData(
           string sortExpression,
           bool sortOrderAsc,
           bool alertsOnTop,
           IEnumerable<int> surveys,
           IEnumerable<int> interviewers,
           string superName,
           bool allCalcenters)
        {
            var callCenterId = allCalcenters ? 0 : ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();
            return GetTasksActivityData(
                  sortExpression,
                  sortOrderAsc,
                  alertsOnTop,
                  surveys,
                  interviewers,
                  superName,
                  callCenterId);
        }

        /// <summary>
        /// Gets task-specific CATI activity data from Fusion.
        /// </summary>
        /// <param name="sortExpression">Database name of column the list is currently sorted on.</param>
        /// <param name="sortOrderAsc">Sorting direction (ascending or descending).</param>
        /// <param name="alertsOnTop">define alerts to be on top of the list</param>
        /// <param name="surveys">List of survey SIDs to get activity data for.</param>
        /// <param name="interviewers">List of groups and interviewers Ids</param>
        /// <param name="superName">logged in supervisor name</param>
        /// <returns>List of task activity data objects.</returns>
        public List<TaskActivityInfo> GetTasksActivityData(
             string sortExpression,
             bool sortOrderAsc,
             bool alertsOnTop,
             IEnumerable<int> surveys,
             IEnumerable<int> interviewers,
             string superName)
        {
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();
            return GetTasksActivityData(
                  sortExpression,
                  sortOrderAsc,
                  alertsOnTop,
                  surveys,
                  interviewers,
                  superName,
                  callCenterId);
        }

        /// <summary>
        /// Gets task-specific CATI activity data from Fusion.
        /// </summary>
        /// <param name="sortExpression">Database name of column the list is currently sorted on.</param>
        /// <param name="sortOrderAsc">Sorting direction (ascending or descending).</param>
        /// <param name="alertsOnTop">define alerts to be on top of the list</param>
        /// <param name="surveys">List of survey SIDs to get activity data for.</param>
        /// <param name="interviewers">List of groups and interviewers Ids</param>
        /// <param name="superName">logged in supervisor name</param>
        /// <param name="callCenterId">callcenter id</param>
        /// <returns>List of task activity data objects.</returns>
        public List<TaskActivityInfo> GetTasksActivityData(
             string sortExpression,
             bool sortOrderAsc,
             bool alertsOnTop,
             IEnumerable<int> surveys,
             IEnumerable<int> interviewers,
             string superName,
             int callCenterId)
        {
            var source = new List<TaskActivityInfo>();

            foreach (BvSpGetListSurveyTasksEntity entity in
                GetActivityData(surveys, interviewers, (surveysBatchId, interviewersBatchId) =>
                                                        BvSpGetListSurveyTasksAdapter.ExecuteEntityList(surveysBatchId,
                                                                                                        interviewersBatchId,
                                                                                                        ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId(),
                                                                                                        callCenterId, superName)))
            {
                var info = SetTaskActivityInfoItem(entity);
                source.Add(info);
            }

            var sortingArgs = new List<SortingArgs>();

            if (alertsOnTop)//Sort resulting rowset on alert status and sorted column in grid.             
            {
                sortingArgs.Add(new SortingArgs("Alert", false));
            }

            if (!String.IsNullOrEmpty(sortExpression))
            {
                sortingArgs.Add(new SortingArgs(sortExpression, sortOrderAsc));

                if (sortExpression == "InterviewState")
                {
                    sortingArgs.Add(new SortingArgs("CallConnectionState", sortOrderAsc));
                }

                if (sortExpression == "StatusLogout")
                {
                    sortingArgs.Add(new SortingArgs("BreakTypeId", sortOrderAsc));
                }
            }

            source.Sort(new CommonMultiComparer<TaskActivityInfo>(sortingArgs));

            return source;
        }

        private TaskActivityInfo SetTaskActivityInfoItem(BvSpGetListSurveyTasksEntity entity)
        {
            var info = new TaskActivityInfo
            {
                SurveySID = entity.SurveySID.Value,
                InterviewID = entity.InterviewID.Value,
                PersonSID = entity.PersonSID.Value,
                InterviewerName = entity.InterviewerName,
                ProjectId = entity.ProjectID,
                ProjectName = entity.SurveyName,
                CallCenterName = entity.CallCenterName,
                TimeCallDelivered = entity.TimeCallDelivered,
                State = entity.State,
                TimezoneID = entity.TzID.Value,
                DiallingMode = (DialingMode)entity.DiallingMode.Value,
                CallOutcome = entity.CallOutcome.Value,
                SecondsElapsed = entity.SecondsSinceLastSubmission,
                StatusLogout = (LoginState)entity.StatusLogout.Value,
                IsMonitored = entity.supervisorName != null,
                LastKeepAliveTime = entity.LastKeepAliveTime,
                InterviewState = entity.InterviewState == (byte)InterviewState.INTERVIEWING && entity.CallType == (byte)CallTypes.Inbound ?
                    InterviewState.INTERVIEWING_INBOUND : (InterviewState)entity.InterviewState.Value,
                LoggedInToDialer = (LoginState)entity.LoggedInToDialerState.Value,
                ProblemState = entity.ProblemId.Value,
                StationIdentifier = entity.StationId,
                DialType = entity.DialType,
                OpenEndReviewInSeconds = entity.OpenEndReviewInSeconds,
                DialerId = entity.DialerId.Value,
                AgentType = (AgentType)entity.Type,
                InterviewDurationInSeconds = entity.TimeCallDelivered.HasValue ? TimeDiff.Seconds(entity.TimeCallDelivered.Value, DateTime.UtcNow) : (int?)null,
                CallType = (CallTypes)entity.CallType,
                LinkedChain = entity.LinkedChain,
                CallConnectionState = (CallConnectionState)entity.CallConnectionState,
                BreakTypeName = entity.BreakTypeName,
                InterviewScreenRecording = entity.InterviewScreenRecording,
                IsWebConsole = entity.IsWebConsole.Value,
                SecondsSinceStateChanged = entity.TimeStateChanged.HasValue ? TimeDiff.Seconds(entity.TimeStateChanged.Value, DateTime.UtcNow) : (int?)null,
            };

            // If interview is not started - we do not show timezone.
            info.TimezoneName = info.InterviewID != 0 ? GetTZNameByBias(entity.Bias) : string.Empty;

            if (info.IsMonitored)
            {
                info.SupervisorName = entity.supervisorName;
                info.MonitoringSessionID = entity.MonitoringSessionID.Value;
            }

            info.LastSubmissionAlert = (AlertStatus)entity.LastSubmissionAlert.Value;
            info.KeepAliveAlert = (AlertStatus)entity.LastKeepAliveTimeAlert.Value;
            info.NoActivityAlert = (AlertStatus)entity.EndOfLastActivityAlert.Value;
            info.InterviewDurationAlert = (AlertStatus)entity.InterviewDurationAlert.Value;
            info.BreakDurationAlert = (AlertStatus)entity.BreakDurationAlert.Value;

            var context = entity.JsonContext != null ? JsonConvert.DeserializeObject<TaskContext>(entity.JsonContext) : new TaskContext();
            info.IsLiveMonitoringEnabled = entity.IsLiveMonitoringEnabled.GetValueOrDefault() ||
                                           context.IsLiveMonitoringEnabled.GetValueOrDefault();

            return info;
        }

        /// <summary>
        /// Gets appointment-specific CATI activity data from Fusion.
        /// </summary>
        /// <param name="sortExpression">Database name of column the list is currently sorted on.</param>
        /// <param name="sortOrderAsc">Sorting direction (ascending or descending).</param>
        /// <param name="filterByExtendedStatus"> If zero no filtering, if not zero return only appointments with this Extended Status</param>
        /// <param name="surveys">List of survey SIDs to get activity data for.</param>
        /// <returns>List of appointment activity data objects.</returns>
        public static List<AppointmentActivityInfo> GetAppointmentActivityData(
             string sortExpression,
             bool sortOrderAsc,
             int filterByExtendedStatus,
             IEnumerable<int> surveys
        )
        {
            List<AppointmentActivityInfo> source = new List<AppointmentActivityInfo>();

            foreach (BvSpGetAppointmentActivityEntity entity in
                GetActivityData(surveys, batchId => BvSpGetAppointmentActivityAdapter.ExecuteEntityList(batchId, null)))
            {
                if (filterByExtendedStatus != 0 && entity.ExtendedStatus != filterByExtendedStatus)
                {
                    continue;
                }

                AppointmentActivityInfo info = new AppointmentActivityInfo()
                {
                    SurveySID = entity.SurveySID.Value,
                    InterviewID = entity.InterviewID.Value,
                    CallID = entity.CallID.Value,
                    InterviewerName = entity.InterviewerName ?? "Any",
                    ProjectID = entity.ProjectID,
                    ProjectName = entity.SurveyName,
                    Alert = (AlertStatus)entity.AlertStatus.Value,
                    AppointmentTime = entity.AppointmentTime.Value,
                    TimezoneID = entity.TZID.Value,
                    TimezoneName = GetTZNameByBias(entity.Bias),
                    ExtendedStatus = entity.ExtendedStatus ?? 0,
                    ExtendedStatusName = entity.ExtendedStatusName
                };

                source.Add(info);
            }

            //Sort resulting rowset on alert status and sorted column in grid.
            List<SortingArgs> sortingArgs = new List<SortingArgs>();
            sortingArgs.Add(new SortingArgs("Alert", false));
            if (!String.IsNullOrEmpty(sortExpression))
                sortingArgs.Add(new SortingArgs(sortExpression, sortOrderAsc));
            source.Sort(new CommonMultiComparer<AppointmentActivityInfo>(sortingArgs));

            return source;
        }

        /// <summary>
        /// Gets survey appoimtment counts from Fusion.
        /// </summary>
        /// <param name="sortExpression">Database name of column the list is currently sorted on.</param>
        /// <param name="sortOrderAsc">Sorting direction (ascending or descending).</param>
        /// <param name="surveys">List of survey SIDs to get activity data for.</param>
        /// <returns>List of survey appointment count objects.</returns>
        public static List<SurveyAppointmentCountInfo> GetSurveyAppointmentCountData(
             string sortExpression,
             bool sortOrderAsc,
             IEnumerable<int> surveys
        )
        {
            //If no surveys are chosen, there is nothing to show.
            if (surveys == null)
                return new List<SurveyAppointmentCountInfo>();

            List<SurveyAppointmentCountInfo> source = new List<SurveyAppointmentCountInfo>();

            foreach (BvSpGetAppointmentCountEntity entity in
                GetActivityData(surveys, batchId => BvSpGetAppointmentCountAdapter.ExecuteEntityList(batchId)))
            {
                SurveyAppointmentCountInfo info = new SurveyAppointmentCountInfo();
                info.SurveySID = entity.SurveySID.Value;
                info.ProjectName = entity.SurveyName;
                info.ProjectId = entity.ProjectID;
                info.ShortIntervalCount = entity.CountForShortInterval.Value;
                info.LongIntervalCount = entity.CountForLongInterval.Value;

                source.Add(info);
            }

            // Add 'total' row
            int totalLong = 0, totalShort = 0;
            foreach (SurveyAppointmentCountInfo info in source)
            {
                totalShort += info.ShortIntervalCount;
                totalLong += info.LongIntervalCount;
            }
            SurveyAppointmentCountInfo totalInfo = new SurveyAppointmentCountInfo();
            totalInfo.SurveySID = 0;
            totalInfo.ProjectId = String.Empty;
            totalInfo.ProjectName = "Total count";
            totalInfo.ShortIntervalCount = totalShort;
            totalInfo.LongIntervalCount = totalLong;
            totalInfo.IsTotalCount = true;
            source.Add(totalInfo);

            //Sort resulting rowset on alert status and sorted column in grid.
            List<SortingArgs> sortingArgs = new List<SortingArgs>();
            sortingArgs.Add(new SortingArgs("IsTotalCount", false));
            if (!String.IsNullOrEmpty(sortExpression))
                sortingArgs.Add(new SortingArgs(sortExpression, sortOrderAsc));
            source.Sort(new CommonMultiComparer<SurveyAppointmentCountInfo>(sortingArgs));

            return source;
        }

        public List<InterviewerPerformanceInfo> GetInterviewerPerformanceData(bool onlyLogged, bool filterBySurveys, bool activeSurveysOnly, int callCenterId, int[] interviewersId = null, IEnumerable<int> surveysId = null)
        {
            var source = BvSpGetInterviewerPerformanceListAdapter.ExecuteEntityList(callCenterId, onlyLogged, filterBySurveys, activeSurveysOnly).
                         Select(entity => new InterviewerPerformanceInfo
                         {
                             InterviewerId = entity.InterviewerId.Value,
                             InterviewerName = entity.InterviewerName,
                             ProjectId = entity.ProjectID,
                             SurveyId = entity.SurveyID.Value,
                             ProjectName = entity.ProjectName,
                             InterviewingTime = TimeSpan.FromSeconds(entity.InterviewingTime.Value),
                             TotalInterviewCount = entity.TotalInterviewCount.Value,
                             CompletedInterviewCount = entity.CompletedInterviewCount.Value,
                             CompletedInLastHourCount = entity.CompletedInLastHourCount.Value,
                             StrikeRateAverage = (float)Math.Round(entity.CompletedInterviewCount.Value / ((float)entity.InterviewingTime.Value / 3600), 2)
                         });

            if (interviewersId != null && interviewersId.Any())
                source = source.Where(x => interviewersId.Contains(x.InterviewerId));

            if (filterBySurveys && surveysId != null && surveysId.Any())
                source = source.Where(x => surveysId.Contains(x.SurveyId));

            return source.ToList();
        }

        /// <summary>
        /// Gets system wide info from Fusion.
        /// </summary>
        /// <param name="surveys">List of survey SIDs to get activity data for.</param>
        /// <returns>System wide info.</returns>
        public static SystemWideInfo GetSystemWideInfo(IEnumerable<int> surveys)
        {
            if (surveys == null)
            {
                return new SystemWideInfo();
            }

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var data = GetActivityData(surveys, batchId => BvSpGetSystemWideInfoAdapter.ExecuteEntity(batchId, callCenterId));

            return new SystemWideInfo
            {
                TotalInterviewersCount = data.TotalInterviewersCount.GetValueOrDefault(),
                LoggedInterviewersCount = data.LoggedInterviewersCount.GetValueOrDefault(),
                LoggedIvrAgentsCount = data.LoggedIvrAgentsCount.GetValueOrDefault(),
                TotalInterviewersWorkedTodayCount = data.TotalInterviewersWorkedTodayCount.GetValueOrDefault(),
                OpenSurveysCount = data.OpenSurveysCount.GetValueOrDefault(),
                CallsCount = data.CallsCount.GetValueOrDefault()
            };
        }

        public static List<TEntity> GetActivityData<TEntity>(IEnumerable<int> surveys,
                                                             IEnumerable<int> interviewers,
                                                             Func<int, int, List<TEntity>> procedureCall)
        {
            using (var surveysBatch = TransferBatch.Create())
            {
                surveysBatch.Insert(surveys);

                using (var interviewersBatch = TransferBatch.Create())
                {
                    interviewersBatch.Insert(interviewers);

                    return procedureCall(surveysBatch.Value, interviewersBatch.Value);
                }
            }
        }

        /// <summary>
        /// Returns productivity data using given procedure call method.
        /// </summary>
        /// <typeparam name="TEntity">Type of entities to return.</typeparam>
        /// <param name="surveys">List of survey identifiers.</param>
        /// <param name="procedureCall">Procedure call method.</param>
        /// <returns>A productivity data entity.</returns>
        public static TEntity GetActivityData<TEntity>(IEnumerable<int> surveys, Func<int, TEntity> procedureCall)
        {
            using (TransferBatch batch = TransferBatch.Create())
            {
                batch.Insert(surveys);

                return procedureCall(batch.Value);
            }
        }

        internal static string GetTZNameByBias(int? bias)
        {
            if (bias == null) return Strings.IncorrectTimezone;

            var biasValue = bias.Value;
            var sign = string.Empty;
            string result;
            var ts = TimeSpan.FromMinutes(biasValue);

            if (biasValue > 0)
            {
                sign = "-";
            }
            else if (biasValue < 0)
            {
                sign = "+";
            }

            if (biasValue == 0)
            {
                result = "(GMT)";
            }
            else if (ts.Minutes == 0)
            {
                result = $"(GMT{sign}{Math.Abs(ts.Hours)})";
            }
            else
            {
                result = $"(GMT{sign}{Math.Abs(ts.Hours)}:{Math.Abs(ts.Minutes)})";
            }
            return result;
        }

        /// <summary>
        /// Gets status breakdown data for the survey.
        /// </summary>
        /// <param name="surveyId">BvFEE surveys sid.</param>
        /// <returns>List of status breakdown data objects for the survey.</returns>
        List<StatusInfo> IActivityManager.GetStatusBreakdown(int surveyId)
        {
            return GetStatusBreakdown(surveyId);
        }

        public static List<StatusInfo> GetStatusBreakdown(int surveyId, bool onlyCatiInterviews = false)
        {
            List<StatusInfo> states = new List<StatusInfo>();

            BvSpSampleStatusSummaryProcessDeltaAdapter.ExecuteNonQuery();

            foreach (BvSpSampleStatusSummary_GetEntity entity in BvSpSampleStatusSummary_GetAdapter.ExecuteEntityList(surveyId, onlyCatiInterviews))
            {
                if (entity.Cnt.HasValue && entity.Cnt.Value != 0)
                {
                    states.Add(
                        new StatusInfo(
                            entity.StateID.Value,
                            entity.StateName,
                            entity.Cnt.Value,
                            (AlertStatus)entity.AlertStatus
                        )
                    );
                }
            }

            return states;
        }

        /// <summary>
        /// Deletes alert for the specified objectSID and thresholdTypeId.
        /// </summary>
        /// <param name="objectSID">BvFEE SID of object (ex., survey SID), now this param is unsupported by BE, always 0.</param>
        /// <param name="thresholdsTypeId">BvFEE thresholds type id (list is defined in BvThresholdsType table of BE).</param>
        public static void DeleteAlert(int objectSID, int thresholdsTypeId)
        {
            string alertName = Enum.GetName(typeof(BvThresholdType), thresholdsTypeId);

            var evt = new DeleteActivityAlertEvent(thresholdsTypeId, alertName);

            BvSpThresholds_deleteAdapter.ExecuteNonQuery(objectSID, thresholdsTypeId);

            evt.Finish();
        }

        /// <summary>
        /// Deletes status alert for the specified surveySID and statusId.
        /// </summary>
        /// <param name="surveySid">BvFEE SID of survey (unsupported by BE, always 0).</param>
        /// <param name="alertId"></param>
        public static void DeleteStatusAlert(int surveySid, int alertId)
        {
            string alertName = Enum.GetName(typeof(BvThresholdType), alertId);

            var evt = new DeleteActivityStatusAlertEvent(alertId, alertName);

            BvSpThresholdITS_DeleteAdapter.ExecuteNonQuery(surveySid, alertId);

            evt.Finish();
        }

        /// <summary>
        /// Sets alert (adds new or changes prevously created).
        /// </summary>
        /// <param name="alert">Alert to be set.</param>
        public static void SetAlert(SurveyAlertInfo alert)
        {
            int alertTypeId = alert.ThresholdsTypeId;
            string alertName = Enum.GetName(typeof(BvThresholdType), alertTypeId);
            new AlertValidator(GetAlertsList()).Validate(alert);
            var evt = new SetActivityAlertEvent(alertTypeId, alertName, alert.Amber, alert.Red);

            alert = UndoModification(alert);
            BvSpThresholds_insertAdapter.ExecuteNonQuery(alert.ObjectSID, alertTypeId, alert.Amber, alert.Red);

            evt.Finish();
        }

        /// <summary>
        /// Sets status alert (adds new or changes prevously created).
        /// </summary>
        /// <param name="alert"></param>
        public static void SetStatusAlert(StatusAlertInfo alert)
        {
            var evt = new SetActivityStatusAlertEvent(alert.StatusId, alert.StatusName, alert.Amber, alert.Red);

            BvSpThresholdITS_SetAdapter.ExecuteNonQuery(alert.ObjectSID, alert.StatusId, alert.Amber, alert.Red);

            evt.Finish();
        }

        /// <summary>
        /// Gets list of currently set alerts.
        /// </summary>
        /// <returns>List of currently set alerts.</returns>
        private static List<SurveyAlertInfo> GetAlertsList()
        {
            List<SurveyAlertInfo> alerts = new List<SurveyAlertInfo>();

            // taking all global allerts
            foreach (BvSpThreshold_ListEntity entity in BvSpThreshold_ListAdapter.ExecuteEntityList(0))
            {
                alerts.Add(
                    ApplyModification(
                        new SurveyAlertInfo(
                            entity.ObjectSID.Value,
                            entity.Amber.Value,
                            entity.Red.Value,
                            entity.ThresholdsTypeID.Value)));
            }

            return alerts;
        }

        /// <summary>
        /// Gets list of currently set survey alerts.
        /// </summary>
        /// <returns>List of currently set survey alerts.</returns>
        public static List<SurveyAlertInfo> GetSurveyAlertsList()
        {
            return GetAlertsList().Where(alert => SurveyListThresholdTypes.Contains(alert.ThresholdType)).ToList();
        }

        /// <summary>
        /// Gets list of currently set status alerts.
        /// </summary>
        /// <param name="includeDefault">If true, default "empty" values are included.</param>
        /// <returns>List of currently set status alerts.</returns>
        public List<StatusAlertInfo> GetStatusAlertsList(bool includeDefault)
        {
            List<StatusAlertInfo> alerts = new List<StatusAlertInfo>();

            // taking all global alerts
            foreach (BvSpThresholdITS_ListEntity entity in BvSpThresholdITS_ListAdapter.ExecuteEntityList(0))
            {
                if (includeDefault || (entity.Amber.HasValue && entity.Amber.Value != Int32.MaxValue &&
                    entity.Red.HasValue && entity.Red.Value != Int32.MaxValue))
                {
                    alerts.Add(
                        new StatusAlertInfo(
                            entity.SurveySID.Value,
                            entity.Amber.Value,
                            entity.Red.Value,
                            entity.ITS.Value,
                            entity.Name
                        )
                    );
                }
            }

            return alerts;
        }

        /// <summary>
        /// Gets list of currently set task alerts.
        /// </summary>
        /// <returns>List of currently set survey alerts.</returns>
        public static List<SurveyAlertInfo> GetTaskAlertsList()
        {
            List<SurveyAlertInfo> alerts = new List<SurveyAlertInfo>();
            foreach (SurveyAlertInfo alert in GetAlertsList())
            {
                if (TaskListThresholdTypes.Contains((BvThresholdType)alert.ThresholdsTypeId))
                {
                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        /// <summary>
        /// Sets appointment alert (adds new or changes prevously created).
        /// </summary>
        /// <param name="amber">Set threshold (minutes) prior to appointment due time.</param>
        /// <param name="red">Red value for alert. Appointment is over due.</param>
        public static void SetAppointmentAlert(int amber, int red)
        {
            int alertId = (int)BvThresholdType.AppointmentListAlert;
            string alertName = Enum.GetName(typeof(BvThresholdType), alertId);

            var evt = new SetActivityAlertEvent(alertId, alertName, amber, red);

            SurveyAlertInfo ai = new SurveyAlertInfo(0, -amber * 60, red * 60, (int)BvThresholdType.AppointmentListAlert);
            SetAlert(ai);

            evt.Finish();
        }

        /// <summary>
        /// Gets currently set appointment alert info. If alert is not set returns null.
        /// </summary>
        public static SurveyAlertInfo GetAppointmentAlert()
        {
            SurveyAlertInfo result = null;
            List<SurveyAlertInfo> alerts = GetAlertsList();
            foreach (SurveyAlertInfo alert in alerts)
            {
                if (alert.ThresholdsTypeId == (int)BvThresholdType.AppointmentListAlert)
                {
                    result = alert;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Sets appointment interval parameters.
        /// </summary>
        /// <param name="shortInterval">Short interval.</param>
        /// <param name="longInterval">Long interval.</param>
        public static void SetAppointmentInterval(TimeSpan shortInterval, TimeSpan longInterval)
        {
            var evt = new SetAppointmentListIntervalsEvent(shortInterval, longInterval);

            int sInt = (int)shortInterval.TotalSeconds;
            int lInt = longInterval.Days == 0 ? (int)longInterval.TotalHours : (-1) * (int)longInterval.TotalDays;

            ServiceLocator.Resolve<ISystemSettings>().AppointmentAlert.ShortInterval = sInt;
            ServiceLocator.Resolve<ISystemSettings>().AppointmentAlert.LongInterval = lInt;

            evt.Finish();
        }

        /// <summary>
        /// Gets appointment interval parameters.
        /// </summary>
        /// <param name="shortInterval">Short interval.</param>
        /// <param name="longInterval">Short interval.</param>
        public static void GetAppointmentInterval(out TimeSpan shortInterval, out TimeSpan longInterval)
        {
            shortInterval = TimeSpan.FromSeconds(ServiceLocator.Resolve<ISystemSettings>().AppointmentAlert.ShortInterval);
            var interval = ServiceLocator.Resolve<ISystemSettings>().AppointmentAlert.LongInterval;
            longInterval = interval > 0 ? TimeSpan.FromHours(interval) : TimeSpan.FromDays(-interval);
        }

        /// <summary>
        /// Applies the modification on alert's red and amber values according to AlertsModificators dictionary.
        /// If alert's threshold type does not contained in AlertsModificators dictionary - no modifications occurs
        /// and method returns original alert.
        /// This method should be called on receiving alert from Fusion.
        /// </summary>
        /// <param name="alert">The alert to modify.</param>
        /// <returns>Modified alert.</returns>
        private static SurveyAlertInfo ApplyModification(SurveyAlertInfo alert)
        {
            BvThresholdType thresholdType = (BvThresholdType)alert.ThresholdsTypeId;
            if (AlertsModificators.ContainsKey(thresholdType))
            {
                alert.Amber /= AlertsModificators[thresholdType];
                alert.Red /= AlertsModificators[thresholdType];
            }

            return alert;
        }

        /// <summary>
        /// Undoes the modification on alert's red and amber values according to AlertsModificators dictionary.
        /// If alert's threshold type does not contained in AlertsModificators dictionary - no modifications occurs
        /// and method returns original alert.
        /// This method should be called on sending alert to Fusion.
        /// </summary>
        /// <param name="alert">The alert to undo modifications.</param>
        /// <returns>Unmodified alert.</returns>
        private static SurveyAlertInfo UndoModification(SurveyAlertInfo alert)
        {
            BvThresholdType thresholdType = (BvThresholdType)alert.ThresholdsTypeId;
            if (AlertsModificators.ContainsKey(thresholdType))
            {
                alert.Amber *= AlertsModificators[thresholdType];
                alert.Red *= AlertsModificators[thresholdType];
            }

            return alert;
        }

        /// <summary>
        /// Termitate task by person ID.
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <param name="reason"> </param>
        /// <exception cref="ArgumentOutOfRangeException">Person ID is out of range.</exception>
        /// <exception cref="UserMessageException">Unable to terminate task.</exception>
        public void TerminateTaskByPerson(int personID, string reason = null)
        {
            if (personID <= 0)
                throw new ArgumentOutOfRangeException("personID");

            string interviewerName = _personRepository.GetById(personID).Name;
            var task = _taskRepository.GetByPerson(personID);

            if (task == null)
            {
                throw new UserMessageException(Strings.UnableToTerminateTaskLoggedOut);
            }

            IManagementActivityEvent evt;
            if (reason == null)
                evt = new TerminateTaskEvent(personID, interviewerName, task);
            else
                evt = new TerminateTaskWithReasonEvent(personID, interviewerName, task, reason);

            if (_supervisorServiceClient.TerminateTaskByPerson(personID, CallOutcome.InterruptedBySystem) == null)
            {
                throw new UserMessageException(Strings.UnableToTerminateTask);
            }

            evt.Finish();
        }
    }
}
