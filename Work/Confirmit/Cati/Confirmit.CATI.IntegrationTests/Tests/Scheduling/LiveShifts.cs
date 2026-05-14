using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.Scheduling
{
    [TestClass]
    public class LiveShifts : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        public class CallData
        {
            public int TzID;
            public int ShiftTypeId;
        }

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        internal void Test_Base(int[] activeTzIDs,
                                KeyValuePair<TestScript, CallData[]>[] scripts,
                                DateTime eventTime,
                                string xmlResult)
        {
            //activate TZ
            foreach (int id in activeTzIDs)
            {
                TimezoneManager.AddTimezone(id);
            }

            //create scripts and surveys
            var surveySIDs = new List<int>();

            foreach (var script in scripts)
            {
                int sid = BackendToolsObject.CreateSurvey(script.Key);
                _surveyStateService.Open(sid);
                surveySIDs.Add(sid);

                if (script.Value == null)
                {
                    foreach (var tz in activeTzIDs)
                    {
                        CreateCallSetForSpecificTz(script.Key, sid, tz);
                    }

                    CreateCallSetForSpecificTz(script.Key, sid, 0);
                }
                else
                {
                    foreach (var call in script.Value)
                    {
                        CreateCall(sid, call.TzID, script.Key.ScheduleID, call.ShiftTypeId);
                    }
                }
                
            }

            BackendTools.ForceProcessingAsyncTriggers();

            /*
             Format of resultXml following:
             
             <Records>
                <result ShiftTypeID="1" TzID="0" SurveyNumber="1"/>
                <result ShiftTypeID="1" TzID="1" SurveyNumber="1"/>
                <result ShiftTypeID="1" TzID="16" SurveyNumber="1"/>
                <result ShiftTypeID="-16" SurveyNumber="1"/>
                <result ShiftTypeID="-1" SurveyNumber="1"/>
                <result ShiftTypeID="0" SurveyNumber="1"/>
                <survey SurveyNumber="1" SID="47"/>
            </Records>
             
             Note: There positive ShiftTypeID will be resolved to ShiftZoneID( with using value from TzID )
             
             */
            string xmlParam = null;
            if (xmlResult != null)
            {
                xmlParam = "<Records>" + xmlResult;
                for (int i = 0; i < surveySIDs.Count; i++)
                {
                    xmlParam += String.Format("<survey SurveyNumber=\"{0}\" SID=\"{1}\"/>",
                        i, surveySIDs[i]);
                }
                xmlParam += "</Records>";
            }

            using (var connection = new SqlConnection(TestingFramework.DbEngine.ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
--params
--DECLARE @resultXml AS NVARCHAR(MAX)
--DECLARE @eventTime AS DATETIME

--unwraping data
DECLARE @hXML AS INT

EXEC sp_xml_preparedocument @hXML OUTPUT, @resultXml

SELECT result.ShiftTypeID, surveySIDs.SID, result.TzID 
INTO #result
FROM OPENXML( @hXML, '/Records/result', 0 )
    WITH (  ShiftTypeID         [int],
            TzID                [int],
            SurveyNumber        [int] ) result
LEFT JOIN OPENXML( @hXML, '/Records/survey', 0 )
    WITH (  SurveyNumber         [int],
            SID                  [int] ) surveySIDs
ON surveySIDs.SurveyNumber = result.SurveyNumber

EXEC sp_xml_removedocument @hXML

-- create expected result

SELECT ISNULL( BvShiftZones.ID, #result.ShiftTypeID ) as ID, 
    #result.SID as SID
INTO #expected
FROM #result 
LEFT JOIN BvSurvey
ON BvSurvey.SID = #result.SID
LEFT JOIN BvShiftType
ON #result.ShiftTypeID = BvShiftType.ID AND BvShiftType.OwnerSID = BvSurvey.ScheduleID
LEFT JOIN BvShiftZones
ON BvShiftZones.TimeZoneID = #result.TzID AND BvShiftZones.ShiftTypeID = BvShiftType.ObjectID

-- get actual result
CREATE TABLE #actual(
ID INT, 
SID INT
)

DECLARE @Temp Table (ID INT, SID INT, ShiftPriority TINYINT)
INSERT INTO @Temp exec BvSpGetLiveShifts @eventTime, {0}

INSERT INTO #actual SELECT ID, SID FROM @Temp

-- compare results
DECLARE @expected NVARCHAR(MAX)
DECLARE @actual NVARCHAR(MAX)

SET @expected = ( SELECT * FROM #expected ORDER BY SID, ID FOR XML RAW('value'), ROOT ( 'values' ) )
SET @actual = ( SELECT * FROM #actual ORDER BY SID, ID FOR XML RAW('value'), ROOT ( 'values' ) )
SELECT @expected as expected, @actual as actual, CASE WHEN @expected = @actual THEN 1 WHEN @expected IS NULL AND @actual IS NULL THEN 1 ELSE 0 END as result

DROP TABLE #actual
DROP TABLE #expected
DROP TABLE #result
", ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId());
                if (xmlParam == null)
                    command.Parameters.AddWithValue("@resultXml", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@resultXml", xmlParam);

                command.Parameters.AddWithValue("@eventTime", eventTime);

                SqlDataReader sdr = command.ExecuteReader();
                Assert.IsTrue(sdr.Read());

                object expected = sdr["expected"];
                object actual = sdr["actual"];
                bool result = (((Int32)sdr["result"]) == 1);

                Assert.AreEqual(expected, actual);

                Assert.IsTrue(result);
            }
        }

        public static int GetShiftTypeObjectId(int scheduleID, int shiftTypeId)
        {
            if (shiftTypeId <= 0)
                return shiftTypeId;

            return BvShiftTypeAdapter.GetAll().Single(x => x.OwnerSID == scheduleID && x.ID == shiftTypeId).ObjectID;
        }

        private static void CreateCallSetForSpecificTz(TestScript script, int sid, int tz)
        {
            foreach (var shiftType in script.ShiftTypes.Where(x => x.Id > 0))
            {
                CreateCall(sid, tz, script.ScheduleID, shiftType.Id);
            }
            CreateCall(sid, tz, script.ScheduleID, (int) CallShiftType.AnyValid);
            CreateCall(sid, tz, script.ScheduleID, (int)CallShiftType.None);
        }

        private static void CreateCall(int sid, int tz, int scheduleId, int shiftTypeId)
        {
            var shiftType = BackendTools.NewInterview(sid);
            if (tz != 0)
            {
                shiftType.TimezoneID = tz;
            }
            BackendTools.CreateInterview(shiftType);
            var call = BackendTools.NewCall(shiftType);
            call.ShiftID = GetShiftTypeObjectId(scheduleId, shiftTypeId);
            BackendTools.CreateCall(call);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeOutOfAllShiftsForAnyTZ_ResultEmpty()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                            new Shift(3, 2, "5.12:00:00", "5.22:00:00" ) ),
                        null
                    )
                },
                DateTime.Parse("2009-02-16T07:00:00"),
                "<result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftForAllTZ_ResultWithShiftType1ForAllTz()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                            new Shift(3, 2, "5.12:00:00", "5.22:00:00" ) ),
                        null
                    )
                },
                DateTime.Parse("2009-02-17T16:00:00"), @"
                <result ShiftTypeID='1' TzID='0' SurveyNumber='0'/>
                <result ShiftTypeID='1' TzID='1' SurveyNumber='0'/>
                <result ShiftTypeID='1' TzID='16' SurveyNumber='0'/>
                <result ShiftTypeID='0' SurveyNumber='0'/>
                <result ShiftTypeID='-1' SurveyNumber='0'/>
                <result ShiftTypeID='-16' SurveyNumber='0'/>
                <result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftForDefaultTZ_ResultWithShiftType1ForDefaultTZ()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                            new Shift(3, 2, "5.12:00:00", "5.22:00:00" ) ),
                        null
                    )
                },
                DateTime.Parse("2009-02-17T21:00:00"), @"
                <result ShiftTypeID='1' TzID='0' SurveyNumber='0'/>
                <result ShiftTypeID='1' TzID='1' SurveyNumber='0'/>
                <result ShiftTypeID='0' SurveyNumber='0'/>
                <result ShiftTypeID='-1' SurveyNumber='0'/>
                <result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeForFirstSurveyInShiftsFor16TzTimeForSecondSurveyInShiftsForDefaultTz_Result16TzForFirstSurveyAndDefaultTzForSecondSurvey()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.22:00:00", "2.10:00:00" ),
                        new Shift(2, 1, "2.22:00:00", "3.10:00:00" ),
                        new Shift(3, 2, "5.22:00:00", "6.10:00:00" ) ),
                        null
                    ),
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                            new Shift(3, 2, "5.12:00:00", "5.22:00:00" ) ),
                        null
                    )

                },
                DateTime.Parse("2009-02-17T21:00:00"), @"
                <result ShiftTypeID='1' TzID='16' SurveyNumber='0'/>
                <result ShiftTypeID='-16' SurveyNumber='0'/>
                <result ShiftTypeID='-2147483648' SurveyNumber='0'/>
                <result ShiftTypeID='1' TzID='0' SurveyNumber='1'/>
                <result ShiftTypeID='1' TzID='1' SurveyNumber='1'/>
                <result ShiftTypeID='0' SurveyNumber='1'/>
                <result ShiftTypeID='-1' SurveyNumber='1'/>
                <result ShiftTypeID='-2147483648' SurveyNumber='1'/>");
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftForOnly16TZ_ResultWithShiftType1For16TZ()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                            new Shift(3, 2, "5.12:00:00", "5.22:00:00" ) ),
                        null
                    )
                },
                DateTime.Parse("2009-02-17T10:00:00"), @"
                <result ShiftTypeID='1' TzID='16' SurveyNumber='0'/>
                <result ShiftTypeID='-16' SurveyNumber='0'/>
                <result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftForAllTZButAllTzCrossingWithExclusion_ResultEmpty()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.10:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.10:00:00", "2.22:00:00" ),
                        new Shift(3, 2, "5.10:00:00", "5.22:00:00" ),
                            new Exclusion(1, "2009-02-17T13:00:00Z", "2009-02-17T21:00:00Z")),
                        null
                    )
                },
                DateTime.Parse("2009-02-17T16:00:00"),
                "<result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftForAllTZAndTimeBeforeExclInDefTZAndCrossWithExclIn16Tz_ResultWithShiftType1ForDefTz()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                        new Shift(3, 2, "5.12:00:00", "5.22:00:00" ),
                            new Exclusion(1, "2009-02-17T15:00:00Z", "2009-02-17T21:00:00Z")),
                        null
                    )
                },
                DateTime.Parse("2009-02-17T14:58:00"), @"
                <result ShiftTypeID='1' TzID='0' SurveyNumber='0'/>
                <result ShiftTypeID='1' TzID='1' SurveyNumber='0'/>
                <result ShiftTypeID='0' SurveyNumber='0'/>
                <result ShiftTypeID='-1' SurveyNumber='0'/>
                <result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftForAllTZAndTimeBeforeExclInDefTZAndCrossWithExclIn16Tz_CallsOnlyForShiftsWithTz1AndNone_ResultWithShiftType1ForDefTz()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                        new TestScript( new Action(Action.Operation.TerminateTheInterview),
                            new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                            new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                            new Shift(3, 2, "5.12:00:00", "5.22:00:00" ),
                            new Exclusion(1, "2009-02-17T15:00:00Z", "2009-02-17T21:00:00Z")),
                        new[]
                        {
                            new CallData{ShiftTypeId = 1, TzID = 1},
                            new CallData{ShiftTypeId = (int)CallShiftType.None, TzID = 1},
                        }
                    )
                },
                DateTime.Parse("2009-02-17T14:58:00"), @"
                <result ShiftTypeID='1' TzID='1' SurveyNumber='0'/>
                <result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftForAllTZAndTimeBeforeExclInDefTZAndCrossWithExclIn16Tz_CallsOnlyForAnyValidWithTz1AndSpecificShiftTypeInDefTz_ResultWithShiftType1ForDefTz()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                        new TestScript( new Action(Action.Operation.TerminateTheInterview),
                            new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                            new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                            new Shift(3, 2, "5.12:00:00", "5.22:00:00" ),
                            new Exclusion(1, "2009-02-17T15:00:00Z", "2009-02-17T21:00:00Z")),
                        new[]
                        {
                            new CallData{ShiftTypeId = 1, TzID = 0},
                            new CallData{ShiftTypeId = (int)CallShiftType.AnyValid, TzID = 1},
                        }
                    )
                },
                DateTime.Parse("2009-02-17T14:58:00"), @"
                <result ShiftTypeID='1' TzID='0' SurveyNumber='0'/>
                <result ShiftTypeID='-1' SurveyNumber='0'/>");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftForAllTZButTimeAfterStartExclInDefTzAndInExclIn16Tz_ResultEmpty()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                        new Shift(3, 2, "5.12:00:00", "5.22:00:00" ),
                            new Exclusion(1, "2009-02-17T15:00:00Z", "2009-02-17T21:00:00Z")),
                        null
                    )
                },
                DateTime.Parse("2009-02-17T15:01:00"),
                "<result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftButTimeBeforeEndOfExclIn16TzAndInExclInDefTz_ResultEmpty()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.10:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.10:00:00", "2.22:00:00" ),
                        new Shift(3, 2, "5.10:00:00", "5.22:00:00" ),
                            new Exclusion(1, "2009-02-17T13:00:00Z", "2009-02-17T18:00:00Z")),
                        null
                    )
                },
                DateTime.Parse("2009-02-17T13:48:00"),
                "<result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftButTimeAfterEndExclIn16TzAndCrossInDefTZ_ResultShiftType1For16Tz()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                        new Shift(3, 2, "5.12:00:00", "5.22:00:00" ),
                            new Exclusion(1, "2009-02-17T15:00:00Z", "2009-02-17T17:00:00Z")),
                        null
                    )
                },
                DateTime.Parse("2009-02-17T15:01:00"), @"
                <result ShiftTypeID='1' TzID='16' SurveyNumber='0'/>
                <result ShiftTypeID='-16' SurveyNumber='0'/>
                <result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftButTimeOutOfExclIn16TzAndBeforeEndExclInDefTZ_ResultShiftType1For16Tz()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                        new Shift(3, 2, "5.12:00:00", "5.22:00:00" ),
                            new Exclusion(1, "2009-02-17T15:00:00Z", "2009-02-17T17:00:00Z")),
                        null 
                    )
                },
                DateTime.Parse("2009-02-17T16:48:00"), @"
                <result ShiftTypeID='1' TzID='16' SurveyNumber='0'/>
                <result ShiftTypeID='-16' SurveyNumber='0'/>
                <result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetLiveShifts_TimeInShiftButTimeAfterExclsForAllTz_ResultShiftType1ForAllTz()
        {
            Test_Base(new[] { 1, 16 },
                new[]
                {
                    new KeyValuePair<TestScript, CallData[]>
                    (
                    new TestScript( new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00" ),
                        new Shift(3, 2, "5.12:00:00", "5.22:00:00" ),
                            new Exclusion(1, "2009-02-17T15:00:00Z", "2009-02-17T17:00:00Z")),
                        null
                    )
                },
                DateTime.Parse("2009-02-17T17:01:00"), @"
                <result ShiftTypeID='1' TzID='0' SurveyNumber='0'/>
                <result ShiftTypeID='1' TzID='1' SurveyNumber='0'/>
                <result ShiftTypeID='1' TzID='16' SurveyNumber='0'/>
                <result ShiftTypeID='0' SurveyNumber='0'/>
                <result ShiftTypeID='-1' SurveyNumber='0'/>
                <result ShiftTypeID='-16' SurveyNumber='0'/>
                <result ShiftTypeID='-2147483648' SurveyNumber='0'/>");
        }

        public class CallInfo
        {
            public int InterviewID = 0;
            public int TzID = 0;
            public int ShiftTypeID = 0;
            public bool SholdBeGiven = false;

            public CallInfo(int tzID, int shiftTypeID, bool isGive)
            {
                TzID = tzID;
                ShiftTypeID = shiftTypeID;
                SholdBeGiven = isGive;
            }
        };

        State WaitState(CatiWsHelper ws, Func<State,bool> comparer)
        {
            DateTime deadLine = DateTime.Now + TimeSpan.FromMinutes(2);

            do
            {
                State state = ws.ConsoleStateService.GetState();

                if (comparer(state))
                    return state;

                if (state.interviewState == (int)InterviewState.NO_CALLS)
                    return null;

                Thread.Sleep(300);
            } while (deadLine > DateTime.Now);

            Assert.Fail("WaitInterviewState timeout expired");
            return null;
        }

        internal State WaitInterviewState(CatiWsHelper ws, InterviewState interviewState)
        {
            return WaitState(ws, state => state.interviewState == (int) interviewState);
        }

        internal void Test2_Base(int[] activeTzIDs,
                                TestScript script,
                                DateTime eventTime,
                                params CallInfo[] callsInfo)
        {
            /*
             'Mock' scheduling date time
             */

            using (var connection = new SqlConnection(TestingFramework.DbEngine.ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"
IF EXISTS( select 1 from sys.procedures where name = 'BvSpQueueUpSheduleTask3_Stub' )
    DROP PROCEDURE BvSpQueueUpSheduleTask3_Stub
DECLARE @text NVARCHAR(MAX)
SET @text = ''
select @text = @text + text from syscomments where id = object_id('BvSpQueueUpSheduleTask3')
set @text = REPLACE( @text, 'BvSpQueueUpSheduleTask3', 'BvSpQueueUpSheduleTask3_Stub' )
EXEC( @text )
DECLARE @query NVARCHAR(MAX)
SET @query = 'ALTER PROCEDURE [dbo].[BvSpQueueUpSheduleTask3]
    @NowUTC           datetime      /*in*/,
    @DefaultTZ        INT,
    @TzBalancingThreshold INT=0
as
    DECLARE @result INT
    EXEC @result = [BvSpQueueUpSheduleTask3_Stub] ''' + CAST( @DATE AS NVARCHAR(MAX) ) + ''', @DefaultTZ
    RETURN @result'
EXEC( @query )";
                command.Parameters.AddWithValue("@date", eventTime);
                command.ExecuteNonQuery();
            }

            const string personLogin = "login";
            const string personPwd = "password";
            const string surveyName = "p123456789";

            //activate TZ
            foreach (int id in activeTzIDs)
            {
                TimezoneManager.AddTimezone(id);
            }

            // create and open survey with person assigned

            int surveySID = BackendToolsObject.CreateSurvey(script, surveyName);
            _surveyStateService.Open(surveySID);
            int personSID = PersonTools.CreatePerson(personLogin, personPwd, AgentTaskChoiceMode.Automatic, null);
            BackendTools.AssignCatiPersonToSurvey(surveySID, personSID);

            var expectedInterviewIDs = new List<int>();

            // create test interviews and calls
            foreach (CallInfo callInfo in callsInfo)
            {
                var interview = BackendTools.NewInterview(surveySID);
                interview.TimezoneID = callInfo.TzID;
                BackendTools.CreateInterview(interview);
                callInfo.InterviewID = interview.ID;

                var call = BackendTools.NewCall(interview);
                call.ShiftID = callInfo.ShiftTypeID > 0 ? script.GetShiftTypeWorkID(callInfo.ShiftTypeID) : callInfo.ShiftTypeID;

                BackendTools.CreateCall(call);

                if (callInfo.SholdBeGiven)
                    expectedInterviewIDs.Add(callInfo.InterviewID);
            }
          
            //login to CATI console CATIConsole
            
            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer properties;
            string stationId = string.Empty;

            var serviceHelper = new CatiWsHelper(personLogin, personPwd);
            var consoleDescriptor = new ConsoleDescription();
            
            serviceHelper.ConsoleService.Login(
                   stationId,
                   consoleDescriptor,
                   out personInfo,
                   out diallerInfo,
                   out properties);

            BackendTools.RunSchedulingProcedure();

            Assert.IsNotNull(properties);
            Assert.AreEqual(personInfo.PersonMode, (int)AgentTaskChoiceMode.Automatic);
            Assert.AreEqual(diallerInfo.ConnectedToDialer, false);

            // get list of issue interviews
            var actualInterviewIDs = new List<int>();
            Assert.IsTrue(serviceHelper.ConsoleService.StartInterview(null, 0));

            State state = WaitInterviewState(serviceHelper, InterviewState.INTERVIEWING);
            while (state != null)
            {
                state = WaitInterviewState(serviceHelper, InterviewState.INTERVIEWING);
                Assert.AreEqual(surveyName, state.surveyId);

                actualInterviewIDs.Add(state.interviewId);

                serviceHelper.ConsoleService.WrapUp(state.interviewId, 1);

                state = WaitInterviewState(serviceHelper, InterviewState.INTERVIEWING);
            }

            // Check result

            string actual = String.Join(",", actualInterviewIDs.Select(x => x.ToString(CultureInfo.InvariantCulture)).OrderBy(x => x).ToArray());
            string expected = String.Join(",", expectedInterviewIDs.Select(x => x.ToString(CultureInfo.InvariantCulture)).OrderBy(x => x).ToArray());

            Assert.AreEqual(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void IssueInterviewsInAutoMode_TimeOutOfAllShiftsForAnyTZ_ResultEmpty()
        {
            Test2_Base(new[] { 1, 16 },
                    new TestScript(new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00"),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00"),
                        new Shift(3, 2, "5.12:00:00", "5.22:00:00")),
                    DateTime.Parse("2009-02-16T07:00:00"),
                    new[]{ 
                        new CallInfo(0, 1, false),
                        new CallInfo(0, 2, false),
                        new CallInfo(1, 1, false),
                        new CallInfo(1, 2, false),
                        new CallInfo(16, 1, false),
                        new CallInfo(16, 2, false),
                        new CallInfo(0, 0, false),
                        new CallInfo(1, -1, false),
                        new CallInfo(16, -16, false),
                        new CallInfo(0, (int)CallShiftType.None, true),
                        new CallInfo(1, (int)CallShiftType.None, true),
                        new CallInfo(16, (int)CallShiftType.None, true)
                    }
                );
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void IssueInterviewsInAutoMode_TimeInShiftForAllTZ_ResultWithShiftType1ForAllTz()
        {
            Test2_Base(new[] { 1, 16 },
                    new TestScript(new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00"),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00"),
                        new Shift(3, 2, "5.12:00:00", "5.22:00:00")),
                    DateTime.Parse("2009-02-17T16:00:00"),
                    new[]{ 
                        new CallInfo(0, 1, true),
                        new CallInfo(0, 2, false),
                        new CallInfo(1, 1, true),
                        new CallInfo(1, 2, false),
                        new CallInfo(16, 1, true),
                        new CallInfo(16, 2, false),
                        new CallInfo(0, 0, true),
                        new CallInfo(1, -1, true),
                        new CallInfo(16, -16, true),
                        new CallInfo(0, (int)CallShiftType.None, true),
                        new CallInfo(1, (int)CallShiftType.None, true),
                        new CallInfo(16, (int)CallShiftType.None, true)
                    }
                );
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void IssueInterviewsInAutoMode_TimeInShiftForDefaultTZ_ResultWithShiftType1ForDefaultTZ()
        {
            Test2_Base(new[] { 1, 16 },
                    new TestScript(new Action(Action.Operation.TerminateTheInterview),
                        new Shift(1, 1, "1.12:00:00", "1.22:00:00"),
                        new Shift(2, 1, "2.12:00:00", "2.22:00:00"),
                        new Shift(3, 2, "5.12:00:00", "5.22:00:00")),
                    DateTime.Parse("2009-02-17T21:00:00"),
                    new[]{ 
                        new CallInfo(0, 1, true),
                        new CallInfo(0, 2, false),
                        new CallInfo(1, 1, true),
                        new CallInfo(1, 2, false),
                        new CallInfo(16, 1, false),
                        new CallInfo(16, 2, false),
                        new CallInfo(0, 0, true),
                        new CallInfo(1, -1, true),
                        new CallInfo(16, -16, false),
                        new CallInfo(0, (int)CallShiftType.None, true),
                        new CallInfo(1, (int)CallShiftType.None, true),
                        new CallInfo(16, (int)CallShiftType.None, true)
                    }
                );
        }

    }
}
