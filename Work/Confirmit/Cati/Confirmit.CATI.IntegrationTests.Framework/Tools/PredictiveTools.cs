using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Random;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.TimeService;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class PredictiveTools
    {
        private readonly BackendTools _backendTools;

        private readonly ISurveyStateService _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();

        public PredictiveTools(BackendTools backendTools)
        {
            _backendTools = backendTools;
        }
        
        /// <summary>
        /// Creates new survey with predictive dialing mode.
        /// </summary>
        /// <returns>Sid of created survey.</returns>
        public int CreatePredictiveSurvey()
        {
            int surveySID = _backendTools.CreateSurvey("p000" + Randomizer.Next(100, 1000));

            SurveyService.SetDialingMode(surveySID, DialingMode.Predictive);

            return surveySID;
        }

        /// <summary>
        /// Opens surveys and launches "All hours" scheduling script for it.
        /// </summary>
        /// <param name="surveySID">Survey identifier.</param>
        public void OpenSurvey(int surveySID)
        {
            _backendTools.LaunchAllHoursScript();
            _surveyStateService.Open(surveySID);
        }

        /// <summary>
        /// Gets user groups for survey.
        /// </summary>
        /// <param name="personSid">The person SID.</param>
        public static List<int> GetGroups(int personSid)
        {
            return BvCallHandlerLibrary.Tools.PersonTools.GetUserGroups(personSid).ToList();
        }

        /// <summary>
        /// Checks the result of GetGroups method. Result's order does not matter.
        /// </summary>
        public static void CheckGetGroupsResult(IEnumerable<int> expected, IEnumerable<int> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());
            foreach (int sid in expected)
            {
                Assert.IsTrue(actual.Contains(sid), "GetGroups method does not return expected SID {0}", sid);
            }
        }

        /// <summary>
        /// Creates given number of calls for given survey.
        /// </summary>
        /// <param name="surveySID">Survey identifier.</param>
        /// <param name="count">Calls count.</param>
        /// <returns>Array of surveys.</returns>
        public static BvCallEntity[] CreateCalls(int surveySID, int count, DialType dialType = DialType.Landline)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            var result = new BvCallEntity[count];

            for (int i = 0; i < count; i++)
            {
                BvInterviewEntity interview = BackendTools.NewInterview(surveySID, dialType);
                BackendTools.CreateInterview(interview);
                result[i] = BackendTools.NewCall(interview);
                BackendTools.CreateCall(result[i]);
                result[i].CallID = CallQueueService.GetCallAndNoLock(surveySID, interview.ID).CallID;
            }

            return result;
        }

        /// <summary>
        /// Assigns given calls of given survey to given person.
        /// </summary>
        /// <param name="surveySID">Survey identifier.</param>
        /// <param name="calls">Calls to assign.</param>
        /// <param name="personOrGroupSID">Person or group identifier assign to.</param>
        public static void AssignCallsToPerson(int surveySID, IEnumerable<BvCallEntity> calls, int personOrGroupSID)
        {
            var interviewIds = new int[calls.Count()];
            int i = 0;
            foreach (BvCallEntity call in calls)
            {
                interviewIds[i++] = call.InterviewID;
            }

            CallTools.AssignCalls(surveySID, interviewIds, personOrGroupSID);
        }


        /// <summary>
        /// Sets time in shift and priority properties to given calls in given survey.
        /// </summary>
        /// <param name="surveySID">Survey identifier.</param>
        /// <param name="calls">Calls to set properties.</param>
        /// <param name="priority">Priority value. If null, priority shouldn't be set.</param>
        /// <param name="timeToCall">Time to call. null means set to now. If setTime parameter is false,
        /// time to call shouldn't be set.</param>
        /// <param name="setTime">Flag indicates that time property shouldn't be set.</param>
        public static void SetTimePriorityToCall(int surveySID, IEnumerable<BvCallEntity> calls, short? priority, DateTime? timeToCall, bool setTime)
        {
            if (setTime == false && priority.HasValue == false)
            {
                // nothing to do. return
                return;
            }

            foreach (BvCallEntity testCall in calls)
            {
                BvCallEntity call = CallQueueService.GetCallAndNoLock(surveySID, testCall.InterviewID);
                if (setTime)
                {
                    call.TimeInShift = timeToCall.HasValue ? timeToCall.Value : DateTime.FromOADate(0);
                }

                if (priority.HasValue)
                {
                    call.Priority = priority.Value;
                }

                // save modified call
                CallQueueService.UpdateCall(call, 0);
            }
        }

        /// <summary>
        /// Gets given amount of calls for predictive survey for given group.
        /// </summary>
        /// <param name="surveySID">Survey identifier.</param>
        /// <param name="groupSID">Group identifier.</param>
        /// <param name="count">Number of calls to return.</param>
        /// <returns>Calls</returns>
        public static IEnumerable<PredictiveCall> GetCallsPerGroup(int surveySID, int groupSID, int count, DialType dialType = DialType.Landline)
        {
            var currentTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            using (var connection = new SqlConnection(IntegrationTestingFramework.Instance.DbEngine.ConnectionString))
            using (var command = BvSpGetCachedCallsForPredictiveSurveyByPersonGroupAdapter.CreateCommand(
                surveySID,
                groupSID,
                count,
                currentTime,
                (int)dialType,
                null))
            {
                command.Connection = connection;
                connection.Open();

                var calls = IntegrationTestingFramework.Instance.DbEngine.ExecuteDataTable<DataTable>(command);

                return calls.AsEnumerable().Select(call =>
                    new PredictiveCall
                    {
                        ID = call.Field<int>("ID"),
                        InterviewID = call.Field<int>("InterviewID"),
                        SurveySID = call.Field<int>("SurveySID"),
                        ExplicitSid = call.Field<int>("ExplicitSid"),
                        DialingMode = call.Field<byte>("DiallingMode"),
                        PhoneNumber = call.Field<string>("TelephoneNumber"),
                        TimeInShift = call.Field<DateTime>("TimeInShift")
                    }
                );
            }
        }

        /// <summary>
        /// Gets given amount of calls for predictive survey for given group.
        /// </summary>
        /// <param name="surveySID">Survey identifier.</param>
        /// <param name="groupSID">Group identifier.</param>
        /// <param name="callSelectionAlgorithm"> </param>
        /// <param name="count">Number of calls to return.</param>
        /// <returns>Calls</returns>
        public static IEnumerable<PredictiveCall> GetCallsForPredictive(int surveySID, int groupSID, CallsSelectionAlgorithm callSelectionAlgorithm, int count, DialType dialType = DialType.Landline)
        {
            SqlCommand command;
            var currentTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            switch (callSelectionAlgorithm)
            {
                case CallsSelectionAlgorithm.ByCampaign:
                    command = BvSpGetCachedCallsForPredictiveSurveyBySurveyAdapter.CreateCommand(surveySID, 1, count, currentTime, (int)dialType,null);
                    break;
                case CallsSelectionAlgorithm.ByPersonGroup:
                    command = BvSpGetCachedCallsForPredictiveSurveyByPersonGroupAdapter.CreateCommand(surveySID, groupSID, count, currentTime, (int)dialType,null);
                    break;
                case CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly:
                    command = BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssignedAdapter.CreateCommand(surveySID, 1, count, currentTime, (int)dialType,null);
                    break;
                case CallsSelectionAlgorithm.CallsAssignedToCampaignOnly:
                    command = BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnlyAdapter.CreateCommand(surveySID, count, currentTime, (int)dialType,null);
                    break;
                default:
                    throw new ArgumentException(
                        string.Format("Incorrect callSelectionAlgorithm: {0}", callSelectionAlgorithm));
            }

            using (var connection = new SqlConnection(IntegrationTestingFramework.Instance.DbEngine.ConnectionString))
            using (command)
            {
                command.Connection = connection;
                connection.Open();

                var calls = IntegrationTestingFramework.Instance.DbEngine.ExecuteDataTable<DataTable>(command);

                return calls.AsEnumerable().Select(call =>
                    new PredictiveCall
                    {
                        ID = call.Field<int>("ID"),
                        InterviewID = call.Field<int>("InterviewID"),
                        SurveySID = call.Field<int>("SurveySID"),
                        ExplicitSid = call.Field<int>("ExplicitSid"),
                        DialingMode = call.Field<byte>("DiallingMode"),
                        PhoneNumber = call.Field<string>("TelephoneNumber"),
                        TimeInShift = call.Field<DateTime>("TimeInShift")
                    }
                );
            }
        }

        /// <summary>
        /// Checks if given actual calls list is equal to expected one. They should be equal in alphabetical order.
        /// </summary>
        /// <param name="expectedCalls">Expected calls.</param>
        /// <param name="actualCalls">Actual calls.</param>
        public static void CheckCalls(IEnumerable<int> expectedCalls, IEnumerable<PredictiveCall> actualCalls)
        {
            Assert.AreEqual(expectedCalls.Count(), actualCalls.Count(), "Actual and expected lists have different lenght");
            int i = 0;
            foreach (PredictiveCall call in actualCalls)
            {
                Assert.AreEqual(expectedCalls.ElementAt(i++), call.ID, "Actual call differs from expected");
            }
        }

        /// <summary>
        /// Checks if given calls assigned to specified person or group.
        /// </summary>
        /// <param name="calls">Calls to check.</param>
        /// <param name="groupOrPersonSID">Group or person identifier.</param>
        public static void CheckCallsAssignment(IEnumerable<PredictiveCall> calls, int groupOrPersonSID)
        {
            foreach (PredictiveCall call in calls)
            {
                Assert.AreEqual(groupOrPersonSID, call.ExplicitSid);
            }
        }

        /// <summary>
        /// Releases calls which were sent to dialler predictively.
        /// Calls can be sent to dialler predictively AGAIN only after they are released.
        /// </summary>
        /// <param name="calls">calls to release</param>
        public static void ReleaseCalls(BvCallEntity[] calls)
        {
            foreach (BvCallEntity call in calls)
            {
                int isReleased;
                ReleaseCall(call.SurveySID, call.InterviewID, out isReleased);
            }
        }

        /// <summary>
        /// Releases a call which was sent to dialler predictively.
        /// Call can be sent to dialler predictively AGAIN only after it is released.
        /// </summary>
        /// <param name="surveySid">call survey sid</param>
        /// <param name="iid">call interview id </param>
        /// <param name="isReleased">out parameter, indicates that call is released.</param>
        public static void ReleaseCall(int surveySid, int iid, out int isReleased)
        {
            BvSpReleaseCallAdapter.ExecuteNonQuery(surveySid, iid, out isReleased);
        }

        public static void CheckUpdatingPhase(int surveySid, IEnumerable<int> interviewIds)
        {
            var expected = interviewIds.Select(x => -2);
            TestAssert.AreEqual(
                expected,
                BvSvyScheduleAdapter.GetAll().Where(x => x.SurveySID == surveySid).Join(interviewIds, x => x.InterviewID, y => y, (x, y) => x.CallState));
        }
    }
}
