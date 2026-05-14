using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using BvCallHandlerLibrary;

using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.PerformanceCounters;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Resources;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Tasks;
using Confirmit.CATI.Core.Telephony.Dial;
using ConfirmitDialerInterface;
using TerminateTaskWhileAutoLogoutEvent = Confirmit.CATI.Core.ActivityLogging.TerminateTaskWhileAutoLogoutEvent;

namespace Confirmit.CATI.Core.Services
{
    public static class TaskService
    {
        /// <summary>
        /// Terminates task only if it exists in the database.
        /// </summary>
        public static BvTasksEntity TerminateTask(
            int personSid,
            DatabaseTransactionOptions transactionOptions,
            CallOutcome? explicitIts = CallOutcome.InterruptedBySystem)
        {
            var task = TaskRepository.GetByPerson(personSid);

            if (task == null)
            {
                var person = PersonRepository.GetById(personSid);

                Trace.TraceWarning(
                    "{0}: Interviewer {1} ({2}) is not logged in.",
                    transactionOptions.Name,
                    person.Name,
                    person.SID);

                return null;
            }

            TerminateTask(
                task,
                transactionOptions,
                explicitIts);

            return task;
        }

        /// <summary>
        /// Terminates task even if task does not exist in the database.
        /// </summary>
        public static BvTasksEntity TerminateTask(
            BvTasksEntity task,
            DatabaseTransactionOptions transactionOptions,
            CallOutcome? explicitIts = CallOutcome.InterruptedBySystem)
        {
            var person = PersonRepository.GetById(task.PersonSID);

            var dial = ServiceLocator.Resolve<IActiveDialRepository>().TryGetById(task.Context.ActiveDialId);
            bool isInterviewOwner = dial == null || ActiveDialService.IsDialOwned(dial, task);

            try
            {
                TerminateTaskOnDialer(task);
                TaskRepository.Update(task);
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    "Error terminating task on dialer for the person {0}({1}).\r\nException:\r\n{2}",
                    person.SID,
                    person.Name,
                    ex);
            }

            if (task.InterviewID > 0 && isInterviewOwner)
            {
                InsertAnswerSubmissionAlertIfNeeded(task.PersonSID);

                if (explicitIts != null)
                {
                    ExecuteSchedulingScriptForTerminatedInterviewAsync(
                        task,
                        explicitIts.Value);
                }
            }

            RemoveTaskAndLogoutPersonInTransaction(
                task.PersonSID,
                transactionOptions, explicitIts, person.Type);

            return task;
        }

        public static void TerminateTaskOnDialer(BvTasksEntity task)
        {
            if (task.DialerId == 0)
            {
                return;
            }

            // TODO: Review do we really need to have try/catch here
            try
            {
                var callHandlerRoot = ServiceLocator.Resolve<IBvCallHandlerRoot>();

                callHandlerRoot.CompleteCallAtTaskTerminationIfNeeded(task);

                callHandlerRoot.LogoutFromDialerAtTaskTerminationIfNeeded(task);
            }
            catch (Exception ex)
            {
                var person = PersonRepository.GetById(task.PersonSID);

                Trace.TraceError(
                    "Error terminating task on dialer for the person {0}({1}).\r\nException:\r\n{2}",
                    person.SID,
                    person.Name,
                    ex);
            }
        }
        public static BvTasksEntity RemoveTaskAndLogoutPersonInTransaction(
            int personId,
            DatabaseTransactionOptions transactionOptions, CallOutcome? explicitIts = null, byte personType = 0)
        {
            List<string> customFieldValues = null;
            var task = ServiceLocator.Resolve<ITaskRepository>().GetByPerson(personId);
            if (task != null && NeedToAddHistoryRecord(explicitIts, task) && task.InterviewID > 0)
            {
                var surveyDbService = ServiceLocator.Resolve<ISurveyDatabaseService>();
                customFieldValues = surveyDbService.GetCustomFieldValues(task.SurveySID, task.InterviewID);
                //use survey db outside the transaction
            }

            using (var transaction = new DatabaseTransactionScope(transactionOptions))
            {
                var monitoringSessionId = ServiceLocator.Resolve<IMonitoringService>().GetActiveMonitoring(personId)
                    ?.MonitoringSessionId;
                var result = RemoveTaskAndLogoutPerson(personId, explicitIts, customFieldValues);

                transaction.Commit();
                if (personType != (byte)AgentType.IvrAgent)
                {
                    ServiceLocator.Resolve<IInterviewerApiClient>().NotifyConsoleTerminating(BackendInstance.Current.CompanyId, personId, monitoringSessionId);
                }

                return result;
            }
        }

        public static BvTasksEntity RemoveTaskAndLogoutPerson(
            int personId, CallOutcome? explicitIts = null, List<string> customFieldValues = null)
        {
            if (personId == 0)
            {
                throw ExceptionManager.NewArgumentException("personId");
            }

            var removedTask = ServiceLocator.Resolve<ITaskRepository>().DeleteByPerson(personId);

            if (removedTask == null)
            {
                return null;
            }

            return LogoutPersonAfterTaskTermination(removedTask, explicitIts, customFieldValues);
        }

        public static BvTasksEntity LogoutPersonAfterTaskTermination(
            BvTasksEntity task, CallOutcome? explicitIts, List<string> customFieldValues)
        {
            AddHistoryRecordIfNeeded(task, explicitIts, customFieldValues);

            FinishInterviewerBreak(task.PersonSID);

            ClearLoginGroup(task.PersonSID);

            CompleteDeferredRecordIfNeeded(task.SurveySID, task.InterviewID, task.PersonSID);

            RevertEnforcedTaskChoiceIfNeeded(task.PersonSID);

            return task;
        }
        
        private static void RevertEnforcedTaskChoiceIfNeeded(int personId)
        {
            var person = PersonRepository.TryGetById(personId);
            // Used when task choice was enforced by Console.EnforceManualSelectionForCellPhonePerson
            if (person?.ManualSelectionOnLogin != null)
            {
                person.ManualSelection = (int)person.ManualSelectionOnLogin;
                person.AllowedChoices = person.AllowedChoicesOnLogin;
                person.ManualSelectionOnLogin = null;
                person.AllowedChoicesOnLogin = null;
                PersonRepository.Update(person);
            }
        }

        private static void CompleteDeferredRecordIfNeeded(int surveySID, int interviewID, int personSID)
        {
            var deferredRecord = BvPersonDeferredMonitoringAdapter.GetByCondition(
            "InterviewID = @InterviewID AND SurveySID = @SurveySID AND PersonSID = @PersonSID ORDER BY ID Desc",
                new[] { new SqlParameter("@SurveySID", surveySID), new SqlParameter("@InterviewID", interviewID), new SqlParameter("@PersonSID", personSID) }).FirstOrDefault();

            if (deferredRecord != null)
            {
                BvPersonDeferredMonitoringAdapterEx.CompleteDeferredMonitoringRecord(deferredRecord.ID, DateTime.UtcNow);
            }
        }
        
        private static bool NeedToAddHistoryRecord(CallOutcome? explicitIts, BvTasksEntity task)
        {
            return explicitIts != null && task.SurveySID > 0 && task.StatusLogout != (byte)LoginState.BREAK && task.StartTime.HasValue;
        }

        private static void AddHistoryRecordIfNeeded(BvTasksEntity task, CallOutcome? explicitIts, List<string> customFieldValues)
        {
            //Store timings to BvHistory
            //Do not store it when called from ConfirmLogout, we are alredy on a Break, SurveySid > 0, and StartTime is null
            //TODO: probably need to refactor this and have better way to distinguish how where we call this function from (explicitIts != null): TaskTerminate or ConfirmLogout
            //looks like we need to check task.StartTime - case: from the break inter select logout and then immediatly closes console.
            if (NeedToAddHistoryRecord(explicitIts, task))
            {
                var interview = ServiceLocator.Resolve<IInterviewRepository>().GetById(task.SurveySID, task.InterviewID);
                
                var history = new BvHistoryEntity() {
                    SurveyId = task.SurveySID,
                    PersonSID = task.PersonSID,
                    RoleID = (byte)Role.Interviewer,
                    FiredTime = task.CurrentUtcTime.Value,
                    InterviewId = task.InterviewID,
                    ITS = (byte?)explicitIts,
                    Duration = task.TimeCallDelivered != null ? TimeDiff.Seconds(task.TimeCallDelivered.Value, task.CurrentUtcTime.Value) : 0,
                    WaitingTime = task.TimeCallDelivered != null ? TimeDiff.Seconds(task.StartTime.Value, task.TimeCallDelivered.Value) : TimeDiff.Seconds(task.StartTime.Value, task.CurrentUtcTime.Value),
                    CallCenterID = task.CallCenterID,
                    OpenEndReviewDuration = task.OpenEndReviewStartTime != null ? TimeDiff.Seconds(task.OpenEndReviewStartTime.Value, task.CurrentUtcTime.Value) : 0,
                    DialTypeId = task.DialTypeId,
                    TelephoneNumber = interview?.TelephoneNumber,
                    SessionId = task.SessionId,
                    Custom1 = customFieldValues?[0],
                    Custom2 = customFieldValues?[1],
                    Custom3 = customFieldValues?[2],
                    Custom4 = customFieldValues?[3],
                    Custom5 = customFieldValues?[4],
                };

                var historyId = ServiceLocator.Resolve<IHistoryRepository>().Insert(history);

                foreach (var dialHistory in task.Context.DialHistories)
                {
                    var entity = new BvDialHistoryToInterviewHistoryEntity()
                    {
                        DialHistoryId = dialHistory.DialId,
                        StartTime = dialHistory.StartTime,
                        FinishTime = dialHistory.FinishTime,
                        InterviewHistoryId = historyId,
                        PersonId = task.PersonSID
                    };
                    BvDialHistoryToInterviewHistoryAdapter.Insert(entity);
                }
            }
        }

        private static void FinishInterviewerBreak(int personSid)
        {
            TimeBreaksHistoryService.FinishInterviewerBreak(personSid);
        }

        private static void InsertAnswerSubmissionAlertIfNeeded(int personSid)
        {
            BvSpTasks_InsertAnswerSubmissionAlertIfNeededAdapter.ExecuteNonQuery(personSid);
        }

        private static void ClearLoginGroup(int personId)
        {
            BvLoginGroupAdapter.DeleteByCondition(
                "PersonSID = @PersonSID",
                new SqlParameter("@PersonSID", personId));
        }

        private static void ExecuteSchedulingScriptForTerminatedInterviewAsync(
            BvTasksEntity task,
            CallOutcome explicitIts)
        {
            var options = new SchedulingScriptExecutionOptions
            {
                ExecutionReason = SchedulingScriptExecutionReason.Terminated,
                ITS = (int)explicitIts,
                IsLogToHistory = false,
                LastCallTime = task.TimeCallDelivered ?? DateTime.UtcNow,
                LastCallPersonSID = task.PersonSID,
                CallCenterID = task.CallCenterID,
                opType = OperationType.TerminateTask
            };

            var asyncManager = ServiceLocator.Resolve<IAsyncManager>();
            asyncManager.QueueWorkItem(() => InterviewService.Schedule(task.SurveySID, task.InterviewID, options));
        }


        /// <summary>
        /// Returns task if exists for specified person otherwise null.
        /// Used in the tests only.
        /// </summary>
        /// <remarks>
        /// If it is needed does relogin into CallsCache.
        /// </remarks>
        public static BvTasksEntity LookupByPersonSid(
            int personSid,
            int surveySid,
            int interviewId)
        {
            var task = ServiceLocator.Resolve<ITaskRepository>().GetByPerson(personSid);
            task.SurveySID = surveySid;

            var interview = LookupByPersonSid(task, interviewId);

            return interview == null ? null : task;
        }

        public static ILookupCallEntity SetNextInterviewForPerson(int personId, int surveySid, int interviewId)
        {
            using (var transaction = new DatabaseTransactionScope("SetNextInterviewForPerson", DeadlockPriority.High))
            {
                var call = BvSpSetNextInterviewForPersonAdapter.ExecuteEntity(personId, surveySid, interviewId, PersonRepository.GetById(personId).AssignmentsListMode);
                transaction.Commit();

                if (call != null)
                {
                    SetLinkedCallIdInTask(personId, call.CallId);
                }

                return call;
            }
        }

        public static void SetNextLinkedInterviewToPrevious(int personId, int surveySid, int interviewId)
        {
            using (var transaction = new DatabaseTransactionScope("SetNextLinkedInterviewToPrevious", DeadlockPriority.High))
            {
                var call = BvSpSetNextInterviewForPersonAdapter.ExecuteEntity(personId, surveySid, interviewId, PersonRepository.GetById(personId).AssignmentsListMode);
                transaction.Commit();

                if (call != null)
                {
                    SetLinkedCallIdInTask(personId, call.CallId);
                    return;
                }

                var newCallEntity = new BvCallEntity{
                    SurveySID = surveySid,
                    InterviewID = interviewId,
                    CallState = (int)CallState.InterviewInProgress
                };
                var newCallId = CallQueueService.AddCallToDb(newCallEntity, (int)CallOutcome.Connected, null);

                SetLinkedCallIdInTask(personId, newCallId);
            }
        }

        private static void SetLinkedCallIdInTask(int personId, int? callId)
        {
            var task = TaskRepository.GetByPerson(personId);
            task.LinkedCallId = callId;
            TaskRepository.Update(task);
        }


        /// <summary>
        /// Returns task if exists for specified person otherwise null.
        /// Used in the tests only.
        /// NOTE: This is test methods and should be moved to IT framework
        /// </summary>
        /// <remarks>
        /// If it is needed does relogin into CallsCache.
        /// </remarks>
        public static BvTasksEntity LookupByPersonSid(
            int personSid,
            int surveySid)
        {
            var task = ServiceLocator.Resolve<ITaskRepository>().GetByPerson(personSid);

            var interview = LookupByPersonSid(task, 0);

            return interview == null ? null : task;
        }

        /// <summary>
        /// Delivers call if it exists and updates task.
        /// If call does not exist returns null.
        /// </summary>
        public static BvInterviewEntity LookupByPersonSid(
            BvTasksEntity task,
            int interviewId)
        {
            var performanceCounters = ServiceLocator.Resolve<IPerformanceCountersContainer>();
            var interviewRepository = ServiceLocator.Resolve<IInterviewRepository>();
            var surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
            var callDeliveryService = ServiceLocator.Resolve<ICallDeliveryService>();
            performanceCounters.GetCallCount.Increment();

            try
            {
                var timer = Stopwatch.StartNew();

                var activityEvent = new GetCallEvent();

                var lookupCall = callDeliveryService.LookupCall(task.PersonSID, task.SurveySID, interviewId, activityEvent);
                performanceCounters.GetCallDuration.IncrementBy(timer.Elapsed);

                if (lookupCall == null)
                {
                    var taskSurvey = surveyRepository.TryGetById(task.SurveySID);
                    activityEvent.Save(task.PersonSID, 0, task.SurveySID,
                        taskSurvey != null ? taskSurvey.Name : "", null);

                    return null;
                }

                var survey = surveyRepository.GetById(lookupCall.SurveyId);

                var call = CallQueueService.GetCallInfo(lookupCall.CallId);

                var interview = interviewRepository.GetByIdWithCheck(lookupCall.SurveyId, lookupCall.InterviewId);

                ServiceLocator.Resolve<ITaskExtension>().AssignCallOnTask(task, survey, interview, call, lookupCall.ActiveDial);

                activityEvent.Save(task.PersonSID, (int)lookupCall.InterviewId, lookupCall.SurveyId, survey.Name, lookupCall.CallId);

                return interview;
            }
            finally
            {
                performanceCounters.GetCallCount.Decrement();
            }
        }

        /// <summary>
        /// Delivers call if it exists and updates task.
        /// If call does not exist returns null.
        /// </summary>

        /// <summary>
        /// Terminates all tasks that are inactive during the time passed as a parameter.
        /// </summary>
        /// <param name="autoLogoutTimeoutInSeconds">Auto terminate timeout in seconds</param>
        public static void RunAutoLogout(int autoLogoutTimeoutInSeconds)
        {
            var tasks = BvTasksAdapter.GetByCondition(
                "DATEDIFF(second, TimeStateChanged, dbo.GetUtcNow()) >= @MaxSecondsSinceLastSubmition",
                new SqlParameter("@MaxSecondsSinceLastSubmition", autoLogoutTimeoutInSeconds));

            TerminateAutoLogoutTasks(tasks);
        }

        /// <summary>
        /// Terminates all web console tasks that are inactive during the time passed as a parameter.
        /// </summary>
        /// <param name="autoLogoutTimeoutInSeconds">Auto terminate timeout in seconds</param>
        public static void RunAutoLogoutWebConsoles(int autoLogoutTimeoutInSeconds)
        {
            var tasks = BvTasksAdapter.GetByCondition(
                "IsWebConsole = 1 AND LastKeepAliveTime IS NOT NULL AND DATEDIFF(second, LastKeepAliveTime, dbo.GetUtcNow()) >= @MaxSecondsSinceLastDisconnect",
                new SqlParameter("@MaxSecondsSinceLastDisconnect", autoLogoutTimeoutInSeconds));

            TerminateAutoLogoutTasks(tasks, true);
        }

        private static void TerminateAutoLogoutTasks(IEnumerable<BvTasksEntity> tasks, bool improperLogout = false)
        {
            foreach (var task in tasks)
            {
                var person = PersonRepository.GetById(task.PersonSID);
                if (person.Type == (int)AgentType.IvrAgent && 
                    (task.InterviewState == (byte)InterviewState.SELECTING || task.InterviewState == (byte)InterviewState.NO_CALLS))
                {
                    continue;
                }

                if(improperLogout)
                {
                    person.ImproperLogoutBBCC = true;
                    PersonRepository.Update(person);
                }

                try
                {
                    var managementEvent = new TerminateTaskWhileAutoLogoutEvent(person.SID, person.Name, task);
                    var interviewerEvent = new TerminateTaskByAutoLogoutEvent();
                    interviewerEvent.UpdateEventPropertiesFromTask(task);
                    interviewerEvent.Details.Task = task;

                    TerminateTask(
                        task.PersonSID,
                        new DatabaseTransactionOptions("TerminateTaskWhileAutoLogout", DeadlockPriority.PeriodicalThread));

                    managementEvent.Finish();
                    interviewerEvent.Save();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(
                        "Error occured while AutoLogout when terminate task for the person {0}({1}).\r\nException:\r\n{2}",
                        person.SID,
                        person.Name,
                        ex);
                }
            }
        }

        /// <summary>
        /// Moves task connected with person into specified state
        /// </summary>
        /// <param name="task">Task entity</param>
        /// <param name="interviewState">New task state</param>
        /// <param name="dialingMode">Dialing mode</param>
        public static void MoveTaskToState(BvTasksEntity task, InterviewState interviewState, DialingMode dialingMode)
        {
            BvSpTasks_UpdateInterviewStateAdapter.ExecuteNonQuery(task.PersonSID, (int)interviewState, (byte)dialingMode);
        }

        public static bool IsSurveyHasTasks(int surveySid)
        {
            return (TaskRepository.GetBySurvey(surveySid).Count() > 0);
        }

        public static void GenerateAndUpdateAuthenticationKeyForTask(BvTasksEntity task)
        {
            task.AuthenticationKey = Guid.NewGuid();
            task.StartSessionTime = DateTime.UtcNow;

            var databaseEngine = new DatabaseEngine();
            databaseEngine.ExecuteNonQuery(
                "UPDATE [BvTasks] SET [AuthenticationKey] = @AuthenticationKey, [StartSessionTime] = @StartSessionTime WHERE [PersonSID] = @PersonSID",
                CommandType.Text,
                new SqlParameter("@AuthenticationKey", task.AuthenticationKey),
                new SqlParameter("@StartSessionTime", task.StartSessionTime),
                new SqlParameter("@PersonSID", task.PersonSID));
        }

        public static void CheckNotLoggedInFromAnotherStation(BvPersonEntity person, BvTasksEntity task, string stationId)
        {
            if (task != null && task.StatusLogout != (byte)LoginState.NOT_LOGGED_IN)
            {
                if (StringComparer.InvariantCultureIgnoreCase.Compare(stationId ?? String.Empty, task.StationId ?? String.Empty) != 0 || task.IsWebConsole)
                {
                    // user is trying to log in from different machine
                    Trace.TraceWarning("CheckNotLoggedInFromAnotherStation: User {0} is trying to log in from station \"{1}\" but he is already logged from \"{2}\"", person.Name, stationId, task.StationId);

                    throw new UserAlreadyLoggedInException(
                        Strings.UserAlreadyLoggedInFromAnotherStation, task.StationId, stationId);
                }
            }
        }

        public static void ResetNewSurveyId(BvTasksEntity task)
        {
            task.NewSurveySID = 0;

            BvSpTasks_UpdateNewSurveySidAdapter.ExecuteNonQuery(
                task.PersonSID,
                task.NewSurveySID);

            EventDetailsScope.Current.AddTiming("BvSpTasks_UpdateNewSurveySidAdapter");
        }

        public static void ApplyNewSurveyId(BvTasksEntity task)
        {
            task.SurveySID = task.NewSurveySID;
            task.SelectedSurveyId = task.NewSurveySID;

            BvSpTasks_UpdateSurveySidAdapter.ExecuteNonQuery(
                task.PersonSID,
                task.NewSurveySID);

            EventDetailsScope.Current.AddTiming("BvSpTasks_UpdateSurveySidAdapter");
        }

        public static void TerminateTasksAsync(int dialerId, DatabaseTransactionOptions databaseTransactionOptions)
        {
            var asyncManager = ServiceLocator.Resolve<IAsyncManager>();
            asyncManager.QueueWorkItem(() => TerminateTasks(dialerId, databaseTransactionOptions));
        }

        public static void TerminateTasks(int dialerId, DatabaseTransactionOptions databaseTransactionOptions)
        {
            var tasks = TaskRepository.GetByDialerId(dialerId);

            foreach (var task in tasks)
            {
                try
                {
                    TerminateTask(
                        task,
                        databaseTransactionOptions);
                }
                catch (Exception ex)
                {
                    var person = PersonRepository.GetById(task.PersonSID);

                    TraceHelper.TraceException(
                        ex,
                        string.Format(
                            "TaskService.TerminateTasks: Terminate task for person '{0}'({1}) is failed. Task: [{2}].",
                            person.Name,
                            person.SID,
                            task.LogString()));
                }
            }
        }
    }
}
