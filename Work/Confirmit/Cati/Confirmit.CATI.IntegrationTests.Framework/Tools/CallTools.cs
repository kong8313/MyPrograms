using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class CallTools
    {
        public static AsyncOperationResult ChangeCallsShiftType(int surveyId, IEnumerable<int> interviewIds, CallStates scheduled, int shiftTypeId)
        {
            var operation = CallManager.ChangeShiftTypeOfCalls(surveyId, shiftTypeId, new SelectedBatchParameters(interviewIds));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult ChangeCallsShiftType(int surveyId, int filterId, CallStates callStates, int shiftTypeId)
        {
            var operation = CallManager.ChangeShiftTypeOfCalls(surveyId, shiftTypeId, new FilteredBatchParameters(surveyId, filterId, 1, callStates, null));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult AssignCalls(int surveyId, IEnumerable<int> interviewIds, int resourceId)
        {
            return AssignCalls(surveyId, interviewIds, new[] {resourceId});
        }

        public static AsyncOperationResult AssignCalls(int surveyId, IEnumerable<int> interviewIds, int[] resources)
        {
            var operation = CallManager.AssignCalls(surveyId, resources, new SelectedBatchParameters(interviewIds));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult AssignCalls(int surveyId, int filterId, int personId)
        {
            return AssignCalls(surveyId, filterId, new int[] {personId});
        }

        public static AsyncOperationResult AssignCalls(int surveyId, int filterId, int[] resources)
        {
            var operation = CallManager.AssignCalls(surveyId, resources, new FilteredBatchParameters(surveyId, filterId, 1, CallStates.Scheduled, null));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult MoveCalls(int surveyId, int[] interviewIds, int its)
        {
            var operation = CallManager.MoveCalls(surveyId, its, new SelectedBatchParameters(interviewIds));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult MoveCalls(int surveyId, int filterSid, CallStates callStates, int its, int localTimezoneId, SearchParameterCollection searchParams)
        {
            var operation = CallManager.MoveCalls(surveyId, its, new FilteredBatchParameters(surveyId, filterSid, localTimezoneId, callStates, searchParams));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult MoveAndRescheduleCalls(int surveyId, IEnumerable<int> interviewIds, int newIts, Appointment appointment = null)
        {
            var operation = CallManager.MoveAndRescheduleCalls(surveyId, newIts, new SelectedBatchParameters(interviewIds), appointment);

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult MoveAndRescheduleCalls(int surveyId, int filterId, int newIts)
        {
            var operation = CallManager.MoveAndRescheduleCalls(surveyId, newIts, new FilteredBatchParameters(surveyId, filterId, 1, CallStates.All, null));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult ChangeCallsPriority(int surveyId, IEnumerable<int> interviewIds, CallStates scheduled, int priority)
        {
            var operation = CallManager.ChangeCallsPriority(surveyId, priority, new SelectedBatchParameters(interviewIds));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult ChangeCallsPriority(int surveyId, int filterId, CallStates callStates, short priority)
        {
            var operation = CallManager.ChangeCallsPriority(surveyId, priority, new FilteredBatchParameters(surveyId, filterId, 1, callStates, null));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult ChangeCallsPriority(int surveyId, int filterId, short priority, SearchParameterCollection searchParameters)
        {
            var operation = CallManager.ChangeCallsPriority(surveyId, priority, new FilteredBatchParameters(surveyId, filterId, 1, CallStates.Scheduled, searchParameters));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult DeleteCalls(int surveyId, int filterId, int timezoneId, CallStates callStates, SearchParameterCollection searchParameters)
        {
            var operation = CallManager.DeleteCalls(surveyId, new FilteredBatchParameters(surveyId, filterId, timezoneId, callStates, searchParameters));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult DeleteCalls(int surveyId, IEnumerable<int> interviewIds)
        {
            var operation = CallManager.DeleteCalls(surveyId, new SelectedBatchParameters(interviewIds));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult EnableCalls(int surveyId, bool state, IEnumerable<int> interviewIds)
        {
            var operation = CallManager.EnableCalls(surveyId, state, new SelectedBatchParameters(interviewIds));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult ActivateCalls(int surveyId, int filterId, int timezoneId, int priority,
            CallStates callState, int personOrGroupId,
            int shiftTypeId, DateTime? timeToCall, bool enableDisabledCalls)
        {
            return ActivateCalls(surveyId, filterId, timezoneId, priority, callState, new[] {personOrGroupId},
                shiftTypeId, timeToCall, enableDisabledCalls);
        }

        public static AsyncOperationResult ActivateCalls(int surveyId, int filterId, int timezoneId, int priority, CallStates callState, int[] resources,
            int shiftTypeId, DateTime? timeToCall, bool enableDisabledCalls)
        {
            var operation = CallManager.ActivateCalls(surveyId, priority, callState, resources, shiftTypeId, null,
                timeToCall, enableDisabledCalls, new FilteredBatchParameters(surveyId, filterId, timezoneId, callState, null));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        public static AsyncOperationResult ActivateCalls(int surveyId, int priority, CallStates callState,
            int personOrGroupId,
            int shiftTypeId, DateTime? timeToCall, bool enableDisabledCalls, IEnumerable<int> intreviewIds)
        {
            return ActivateCalls(surveyId, priority, callState, new []{personOrGroupId}, shiftTypeId, 
                timeToCall, enableDisabledCalls, intreviewIds);
        }

        public static AsyncOperationResult ActivateCalls(int surveyId, int priority, CallStates callState, int[] resources,
            int shiftTypeId, DateTime? timeToCall, bool enableDisabledCalls, IEnumerable<int> intreviewIds)
        {
            var operation = CallManager.ActivateCalls(surveyId, priority, callState, resources, shiftTypeId, null,
                timeToCall, enableDisabledCalls, new SelectedBatchParameters(intreviewIds));

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        /// <summary>
        /// Checks if call phase is right in BvSvySchedule table.
        /// </summary>
        /// <param name="callID">Call ID to check</param>
        /// <param name="modelPhase">CallState that call should have</param>
        public static void CheckCallPhaseInBvSvySchedule(int callID, int modelPhase)
        {
            IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;
            int phase = framework.DbEngine.ExecuteScalar<int>("select CallState from bvSvySchedule where ID=@CallID",
                                                                CommandType.Text,
                                                                new SqlParameter("@CallID", callID)
                );
            Assert.IsTrue(phase == modelPhase);
        }

        /// <summary>
        /// Checks if call exists is right in bvSvySchedule table.
        /// </summary>
        /// <param name="callID">Call ID to check</param>
        public static void CheckCallNotExistInbvSvySchedule(int callID)
        {
            IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;
            DataTable dt = framework.DbEngine.ExecuteDataTable<DataTable>("select 1 from bvSvySchedule where ID=@CallID and CallState != 0",
                                                                            CommandType.Text,
                                                                            new SqlParameter("@CallID", callID)
                );
            Assert.IsTrue(dt.Rows.Count == 0);
        }

        /// <summary>
        /// Checks that right call are given by LookupByPersonSID and calls phase are right in database.
        /// Survey selection mode.
        /// </summary>
        /// <param name="personID">Person ID for calls lookup</param>
        /// <param name="modelCallID">Call ID that should be given to person</param>
        /// <param name="surveyID">Survey SID</param>
        public static void AssertCallWasGiven(int personID, int modelCallID, int surveyID)
        {
            BvTasksEntity task = TaskService.LookupByPersonSid(personID, surveyID);
            Assert.IsTrue(task != null);
            if (task != null)
            {
                int callId = task.CallID.Value;
                Assert.AreEqual(modelCallID, callId);
                CheckCallPhaseInBvSvySchedule(callId, -1);
            }
        }

        /// <summary>
        /// Checks that right call are given by LookupByPersonSID and calls phase are right in database.
        /// Automatic mode.
        /// </summary>
        /// <param name="personID">Person ID for calls lookup</param>
        /// <param name="modelCallID">Call ID that should be given to person</param>
        public static void AssertCallWasGiven(int personID, int modelCallID)
        {
            AssertCallWasGiven(personID, modelCallID, 0);
        }

        /// <summary>
        /// Checks that no call was given by LookupByPersonSID.
        /// Survey selection mode.
        /// </summary>
        /// <param name="personID">Person ID for calls lookup</param>
        /// <param name="surveyID">Survey ID</param>
        public static void AssertNoCallWasGiven(int personID, int surveyID)
        {
            BvTasksEntity task = TaskService.LookupByPersonSid(personID, surveyID);
            Assert.IsTrue(task == null);
        }

        /// <summary>
        /// Checks that no call was given by LookupByPersonSID.
        /// Automatic mode.
        /// </summary>
        /// <param name="personID">Person ID for calls lookup</param>
        public static void AssertNoCallWasGiven(int personID)
        {
            AssertNoCallWasGiven(personID, 0);
        }

        /// <summary>
        /// Checks that call exists in BvSvySchedule table (and not marked for deleting).
        /// </summary>
        /// <param name="callID">Call ID to check</param>
        public static void CheckCallExistsInBvSvySchedule(int callID)
        {
            IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;
            int count = framework.DbEngine.ExecuteScalar<int>("select count(*) from bvSvySchedule where ID=@CallID and CallState > 0",
                                                    CommandType.Text,
                                                    new SqlParameter("@CallID", callID)
                );
            Assert.AreEqual(1, count);
        }

        /// <summary>
        /// Checks if call time is right in BvSvySchedule table.
        /// </summary>
        /// <param name="callID">Call ID to check</param>
        /// <param name="modelTime">Time in shift that call should have</param>
        public static void CheckCallTimeInBvSvySchedule(int callID, DateTime modelTime)
        {
            IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;
            DateTime time = framework.DbEngine.ExecuteScalar<DateTime>("select TimeInShift from bvSvySchedule where ID=@CallID",
                                                                         CommandType.Text,
                                                                         new SqlParameter("@CallID", callID)
                );
            Assert.IsTrue(time == modelTime);
        }

        /// <summary>
        /// Checks if appointment state is right in BvAppointment table.
        /// </summary>
        /// <param name="surveySID">Survey ID</param>
        /// <param name="interviewSID">Interview ID</param>
        /// <param name="modelState">State that call should have</param>
        public static void CheckAppointmentState(int surveySID, int interviewSID, int modelState)
        {
            IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;
            int state = framework.DbEngine.ExecuteScalar<int>("select State from BvAppointment where InterviewSID = @InterviewSID and SurveySID = @SurveySID",
                                                        CommandType.Text,
                                                        new SqlParameter("@InterviewSID", interviewSID),
                                                        new SqlParameter("@SurveySID", surveySID));
            Assert.AreEqual(modelState, state);
        }


        
    }
}
