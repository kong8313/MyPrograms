using System;
using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class Shifts: BaseMockedIntegrationTest
    {
        private readonly int _timezoneId;
        private readonly ICallCenterRepository _callCenterRepository;
        
        public Shifts()
        {
            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();
            _callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
            
            TestingFramework.DbEngine.ExecuteNonQuery("INSERT INTO dbo.BvTimezone SELECT *, NULL AS ParentID FROM dbo.BvTimezoneMaster master WHERE master.ID NOT IN ( SELECT ID FROM BvTimezone )",
                CommandType.Text, new SqlParameter[] { });
        }
        
        enum ShiftTypeID
        {
            Type1 = 1,
            Type2 = 2
        }

        /// <summary>
        /// Test base
        /// - activate all TZ
        /// - set default TZ to 1
        /// - create survey with scheduling script( with specific action( "action" ) and "param" )
        /// - create one interview with call for cpecific TZ( param tzID )
        /// - fire event on specific time( param eventTime )
        /// - check call( call time(param resultTime ) and shiftType( param shiftTypeID ) )
        /// </summary>
        /// <param name="tzID">Interview TZ</param>
        /// <param name="action">Scheduling script action</param>
        /// <param name="param">param for scheduling script action</param>
        /// <param name="eventTime">Time of fire event</param>
        /// <param name="resultTime">actual CallTime for call</param>
        /// <param name="shiftTypeID">actual shiftTypeID for call</param>
        void Test_Base(int tzID, Action.Operation action, string param, DateTime eventTime, DateTime resultTime, ShiftTypeID shiftTypeID)
        {
            UpdateDefaultTzId(tzID);

            var script = new TestScript(
                    new Action(action, param),
                    @"Scheduling2007\ScheduleShift.xml");
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TimezoneID = tzID;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            BackendTools.FireEvent(interview, eventTime);

            BackendTools.CheckInterview(interview);
            if (action != Action.Operation.RecallAfterANumberOfMinutes)
            {
                call.ShiftID = script.GetShiftTypeWorkID((int)shiftTypeID);
            }

            call.TimeInShift = resultTime;
            BackendTools.CheckCall(call);
        }

        private void UpdateDefaultTzId(int tzId)
        {
            if (tzId == 0)
                tzId = 1;

            var def = _callCenterRepository.Default;
            def.LocalTimezoneId = tzId;
            _callCenterRepository.Update(def);
        }

        /// <summary>
        /// Test base
        /// - activate all TZ
        /// - set specific default TZ( param siteTzID )
        /// - create survey with scheduling script( param script )
        /// - create one interview with call for cpecific TZ( param interviewTzID )
        /// - fire event on specific time( param eventTime )
        /// - check call( call time(param resultTime ) and shiftType( param shiftTypeID ) )
        /// </summary>
        /// <param name="siteTzID"></param>
        /// <param name="interviewTzID"></param>
        /// <param name="script"></param>
        /// <param name="eventTime"></param>
        /// <param name="resultTime"></param>
        /// <param name="shiftTypeID"></param>
        void Test_Base(int siteTzID, int interviewTzID, TestScript script, DateTime eventTime, DateTime resultTime, int shiftTypeID)
        {
            int surveySID = BackendToolsObject.CreateSurvey(script);

            UpdateDefaultTzId(siteTzID);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TimezoneID = interviewTzID;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            BackendTools.FireEvent(interview, eventTime);

            BackendTools.CheckInterview(interview);
            if (script.Rules[0].SubRules[0].Actions[0].ActionId != Action.Operation.RecallAfterANumberOfMinutes)
            {
                call.ShiftID = script.GetShiftTypeWorkID(shiftTypeID);
            }

            call.TimeInShift = resultTime;
            BackendTools.CheckCall(call);
        }

        #region Simple test of the "Recall after N of minutes"

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_RecallAfter15Min_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "15",
                DateTime.Parse("2008-11-23T10:30:00"),
                DateTime.Parse("2008-11-23T12:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz2AndTimeInShift_RecallAfter15Min_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);
            
            Test_Base(2, Action.Operation.RecallAfterANumberOfMinutes, "15",
                DateTime.Parse("2008-11-23T10:30:00"),
                DateTime.Parse("2008-11-23T10:45:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz3AndTimeInShift_RecallAfter15Min_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(3, Action.Operation.RecallAfterANumberOfMinutes, "15",
                DateTime.Parse("2008-11-23T11:00:00"),
                DateTime.Parse("2008-11-23T11:15:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz8AndTimeOutOfShift_RecallAfter15Min_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(8, Action.Operation.RecallAfterANumberOfMinutes, "15",
                DateTime.Parse("2008-11-23T08:30:00"),
                DateTime.Parse("2008-11-23T10:00:00"),
                ShiftTypeID.Type2);
        }
        #endregion

        #region Time to call is before the shift start, result time - the shift start

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_RecallAfter60Min_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T05:00:00"),
                DateTime.Parse("2008-11-23T12:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz2AndTimeOutOfShift_RecallAfter60Min_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(2, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T05:00:00"),
                DateTime.Parse("2008-11-23T08:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz3AndTimeOutOfShift_RecallAfter60Min_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(3, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T05:00:00"),
                DateTime.Parse("2008-11-23T09:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz8AndTimeOutOfShift_RecallAfter60Min_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(8, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T05:00:00"),
                DateTime.Parse("2008-11-23T10:00:00"),
                ShiftTypeID.Type2);
        }
        #endregion

        #region Time to call is inside the exclusion, result time - the exclusion end (current shift)
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_RecallAfter180MinOnExclusion_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "180",
                DateTime.Parse("2008-11-23T10:30:00"),
                DateTime.Parse("2008-11-23T14:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz2AndTimeInShift_RecallAfter60MinOnExclusion_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(2, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T08:30:00"),
                DateTime.Parse("2008-11-23T10:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz2AndTimeInShift_RecallAfter60MinOnStartExclusion_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(2, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T08:00:00"),
                DateTime.Parse("2008-11-23T10:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz2AndTimeInShift_RecallAfter60MinOnFinishExclusion_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(2, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T09:00:00"),
                DateTime.Parse("2008-11-23T10:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz3AndTimeInShift_RecallAfter60MinOnExclusion_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(3, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T09:30:00"),
                DateTime.Parse("2008-11-23T11:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz8AndTimeInShift_RecallAfter60MinOnExclusion_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(8, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T10:30:00"),
                DateTime.Parse("2008-11-23T12:00:00"),
                ShiftTypeID.Type2);
        }
        #endregion

        #region Time to call is after the shift end, result time - shift start next week

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_RecallAfter60MinOnExclusion_TimeInShiftOnNextWeek(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T17:00:00"),
                DateTime.Parse("2008-11-29T12:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz2AndTimeOutOfShift_RecallAfter60MinOnExclusion_TimeInShiftOnNextWeek(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(2, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T12:30:00"),
                DateTime.Parse("2008-11-24T08:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz3AndTimeOutOfShift_RecallAfter60MinOnExclusion_TimeInShiftOnNextWeek(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(3, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T13:30:00"),
                DateTime.Parse("2008-11-29T09:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz8AndTimeOutOfShift_RecallAfter60Min_TimeInShiftOnNextWeek(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(8, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T14:00:00"),
                DateTime.Parse("2008-11-29T10:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz8AndTimeOutOfShift_RecallAfter60MinOn1MinOfEndShift_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(8, Action.Operation.RecallAfterANumberOfMinutes, "60",
                DateTime.Parse("2008-11-23T13:59:00"),
                DateTime.Parse("2008-11-23T14:59:00"),
                ShiftTypeID.Type2);
        }
        #endregion

        #region Time to call is inside the exclusion, result time - the shift start next day
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_RecallAfter60MinOnExclusion_TimeInShiftOnNextDay(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "1260",
                DateTime.Parse("2008-11-22T14:30:00"),
                DateTime.Parse("2008-11-23T12:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz2AndTimeOutOfShift_RecallAfter60MinOnExclusion_TimeInShiftOnNextDay(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(2, Action.Operation.RecallAfterANumberOfMinutes, "1260",
                DateTime.Parse("2008-11-22T12:30:00"),
                DateTime.Parse("2008-11-23T10:00:00"),
                ShiftTypeID.Type2);
        }
        #endregion

        #region skip shifts under one exlusion
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallBeforeExlusion_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-16T11:00:00"),
                DateTime.Parse("2008-11-22T12:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallOnStartExlusion_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-16T11:30:00"),
                DateTime.Parse("2008-11-22T12:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallInExlusion_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-16T12:30:00"),
                DateTime.Parse("2008-11-22T12:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallInEndExlusion_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-16T12:30:00"),
                DateTime.Parse("2008-11-22T12:00:00"),
                ShiftTypeID.Type1);
        }

        #endregion

        #region skip shift under two exlusions
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallBeforeOneExlusion_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-09T10:00:00"),
                DateTime.Parse("2008-11-14T10:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallOnFirstStartExlusion_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);
            
            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-09T10:30:00"),
                DateTime.Parse("2008-11-14T10:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallInFirstExlusionButBeforeShift_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-09T11:00:00"),
                DateTime.Parse("2008-11-14T10:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallInFirstExlusionAndStartShift_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-09T11:30:00"),
                DateTime.Parse("2008-11-14T10:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallInFirstExlusionAndInShift_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-09T12:00:00"),
                DateTime.Parse("2008-11-14T10:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallInEndFirstAndInStartSecondExlusionsAndInShift_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-09T13:30:00"),
                DateTime.Parse("2008-11-14T10:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallInSecondExlusionAndInShift_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-09T14:00:00"),
                DateTime.Parse("2008-11-14T10:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallInSecondExlusionAndInEndShift_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-09T16:30:00"),
                DateTime.Parse("2008-11-14T10:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallInSecondExlusionAndAfterShift_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-09T17:00:00"),
                DateTime.Parse("2008-11-14T10:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallInEndSecondExlusionAndAfterShift_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-09T17:30:00"),
                DateTime.Parse("2008-11-14T10:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeOutOfShift_CallAfterSecondExlusion_TimeInNextShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-09T18:00:00"),
                DateTime.Parse("2008-11-14T10:00:00"),
                ShiftTypeID.Type1);
        }

        #endregion

        #region call to transient shift on week

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz4AndTimeOutOfShift_CallBeforeTransientShift_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(4, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-29T18:00:00"),
                DateTime.Parse("2008-11-29T19:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz4AndTimeOutOfShift_CallOnStartTransientShift_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(4, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-29T18:30:00"),
                DateTime.Parse("2008-11-29T19:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz4AndTimeInShift_CallInTransientShiftBeforeEndWeek_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(4, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-29T19:00:00"),
                DateTime.Parse("2008-11-29T19:30:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz4AndTimeInShift_CallInTransientShiftAndEndWeekByTZ_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(4, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-29T22:30:00"),
                DateTime.Parse("2008-11-29T23:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz4AndTimeInShift_CallInTransientShiftAndEndWeekByUTC_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(4, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-29T23:30:00"),
                DateTime.Parse("2008-11-30T00:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz4AndTimeInShift_CallInTransientShiftAndNewWeek_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(4, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-30T00:00:00"),
                DateTime.Parse("2008-11-30T00:30:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz4AndTimeInShift_CallBeforeEndTransientShift_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(4, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-30T02:29:00"),
                DateTime.Parse("2008-11-30T02:59:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz4AndTimeInShift_CallOnEndTransientShift_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(4, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-30T02:30:00"),
                DateTime.Parse("2008-11-30T12:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz4AndTimeInShift_CallAfterEndTransientShift_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(4, Action.Operation.RecallAfterANumberOfMinutes, "30",
                DateTime.Parse("2008-11-30T03:00:00"),
                DateTime.Parse("2008-11-30T12:00:00"),
                ShiftTypeID.Type2);
        }
        #endregion endregion

        #region Time to call is inside the exclusion, result time - the shift start next day

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeInShift_CallAfterDay_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfMinutes, "1440",
                DateTime.Parse("2008-11-20T14:30:00"),
                DateTime.Parse("2008-11-22T12:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeInShift_RecallAfterOneShift_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfShifts, "1",
                DateTime.Parse("2008-11-23T05:00:00"),
                DateTime.Parse("2008-11-23T12:00:00"),
                ShiftTypeID.Type2);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeInShift_RecallAfterTwoShift_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfShifts, "2",
                DateTime.Parse("2008-11-22T12:30:00"),
                DateTime.Parse("2008-11-29T12:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeInShift_RecallAfterOneShiftOnNextWeek_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfShifts, "1",
                DateTime.Parse("2008-11-23T14:30:00"),
                DateTime.Parse("2008-11-29T12:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeInShift_RecallAfterTwoShiftOnNextWeek_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfShifts, "2",
                DateTime.Parse("2008-11-22T14:30:00"),
                DateTime.Parse("2008-11-29T12:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz2AndTimeInShift_RecallAfterTwoShiftOnNextWeek_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(2, Action.Operation.RecallAfterANumberOfShifts, "2",
                DateTime.Parse("2008-11-22T14:30:00"),
                DateTime.Parse("2008-11-24T08:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeInShift_RecallAfterFiveShiftOnNextWeek_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfShifts, "5",
                DateTime.Parse("2008-11-20T14:30:00"),
                DateTime.Parse("2008-11-29T12:00:00"),
                ShiftTypeID.Type1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Tz0AndTimeInShift_RecallAfterOneShiftOnNextShift_TimeInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(0, Action.Operation.RecallAfterANumberOfShifts, "1",
                DateTime.Parse("2008-11-20T14:30:00"),
                DateTime.Parse("2008-11-22T12:00:00"),
                ShiftTypeID.Type1);
        }
        #endregion

        #region Bug 32287 Incorrect work of "Recall after number of minutes" action, if time to call is inside the exclusion
        /*
        Bug 32287 Incorrect work of "Recall after number of minutes" action, if time to call is inside the exclusion
        
        Create a schedulign script with 'Recall after number of minutes' action with parameter 15 on Custom1 status.
        If time to call is inside the exclusion, time to call is sset incorrectly in some cases (see cases below):

        Site timezone is Moscow (ID=16)
        There are two shifts:
        1. on Tuesday from 12:00 to 22:00
        2. on Wednesday from 12:00 to 22:00

        We perform an action Move and Reschedule to Custom1 status from Call management in CP on Tuesday 16:00 (so time to call is 16:15)
        Cases (in every case there is only one exclusion, not all together):
        There is an exclusion
         - today from 10:00 to 18:00 (intersects with the beginning of a current shift) - result time should be the end of the exclusion, but it set to today 17:00 (an hour less the end of exclusion)
         - today from 10:00 to 23:00 (current shift is in the exclusion) - OK (result time is 12:00 tomorrow)
         - today from 13:00 to 18:00 (exclusion is in the current shift)  - result time should be the end of the exclusion, but it set to today 17:00 (an hour less the end of exclusion)
         - today from 16:00 to 23:00 (intersects with the end of a current shift)- OK (result time is 12:00 tomorrow)
         - from 16:00 today to 15:00 tomorrow - result time should be the end of the exclusion (tomorrow), but it set to 12:00 next Tuesday 
         
        Node by Maxim Lipatov: The problem with "an hour less the end of exclusion" coresponded with invalid datetime for exclusion in XML( datetime converted to UTC, but it is not to do ), because it case must not be tested here.
        */
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TzMoscowAndTwoShiftsWithCrossExclusionOnFirstBeginShift_CallOnCrosingExclusionWithShift_TimeOnEndExclusion(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TimeSpan tzOffset = TimeSpan.FromHours(-3);

            var script = new TestScript(new Action(Action.Operation.RecallAfterANumberOfMinutes, "15"),
                new Shift(1, 1, "2.12:00:00", "2.22:00:00"),
                new Shift(2, 1, "3.12:00:00", "3.22:00:00"),
                new Exclusion(1, "2009-02-17T10:00:00Z", "2009-02-17T18:00:00Z"));

            Test_Base(16, 0, script,
                DateTime.Parse("2009-02-17T16:00:00") + tzOffset,
                DateTime.Parse("2009-02-17T18:00:00") + tzOffset,
                1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TzMoscowAndTwoShiftsWithCrossExclusionOnFirstEndShiftAndSecondBeginShift_CallOnCrosingExclusionWithShift_TimeOnEndExclusion(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TimeSpan tzOffset = TimeSpan.FromHours(-3);

            var script = new TestScript(new Action(Action.Operation.RecallAfterANumberOfMinutes, "15"),
                new Shift(1, 1, "2.12:00:00", "2.22:00:00"),
                new Shift(2, 1, "3.12:00:00", "3.22:00:00"),
                new Exclusion(1, "2009-02-17T16:00:00Z", "2009-02-18T15:00:00Z"));

            Test_Base(16, 0, script,
                DateTime.Parse("2009-02-17T16:00:00") + tzOffset,
                DateTime.Parse("2009-02-18T15:00:00") + tzOffset,
                1);
        }

        #endregion

        #region Bug 32285 Incorrect work of "Recall after number of shifts" action, if time to call is inside the exclusion
        /*
        Create a schedulign script with 'Recall after number of shifts' action with parameter 1 on Custom1 status.
        If time to call is inside the exclusion, time to call is set incorrectly in some cases (see cases below):
        Site timezone is Moscow (ID=16)
        There are three shifts:
        1. on Tuesday from 12:00 to 22:00
        2. on Wednesday from 12:00 to 22:00
        3. on Thursday from 12:00 to 22:00
        We perform an action Move and Reschedule to Custom1 status from Call management in CP on Tuesday 18:00 (so time to call is tomorrow 12:00)
        Cases (in every case there is only one exclusion, not all together):
        There is an exclusion
         - tomorow from 10:00 to 14:00 (intersects with the beginning of the next shift) - result time should be the end of the exclusion, but it set to tomorrow 13:00 (an hour less the end of exclusion)
         - tomorrow from 10:00 to 23:00 (next shift is in the exclusion) - OK (result time is 12:00 on Thursday)
         - from 12:00 tomorrow to 14:00 the day after tomorrow - result time should be the end of the exclusion (the day after tomorrow), but it set to 12:00 next Tuesday
        Note: The same is actual for 'Recall after number of shifts (random time)' and 'Recall on next shift of specified type' actions
        */

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TzMoscowAndThreeShiftsWithCrossExclusionOnSecondBeginShift_CallOnBetweenFirstAndSecondShifts_TimeOnEndExclusion(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TimeSpan tzOffset = TimeSpan.FromHours(-3);

            var script = new TestScript(new Action(Action.Operation.RecallAfterANumberOfShifts, "1"),
                new Shift(1, 1, "2.12:00:00", "2.22:00:00"),
                new Shift(2, 1, "3.12:00:00", "3.22:00:00"),
                new Shift(3, 1, "4.12:00:00", "4.22:00:00"),
                new Exclusion(1, "2009-02-18T10:00:00Z", "2009-02-18T14:00:00Z"));

            Test_Base(16, 0, script,
                DateTime.Parse("2009-02-17T18:00:00") + tzOffset,
                DateTime.Parse("2009-02-18T14:00:00") + tzOffset,
                1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TzMoscowAndThreeShiftsWithCrossExclusionOnBeginSecondShift_CallOnBetweenFirstAndSecondShifts_TimeOnEndExclusion(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TimeSpan tzOffset = TimeSpan.FromHours(-3);

            var script = new TestScript(new Action(Action.Operation.RecallAfterANumberOfShifts, "1"),
                new Shift(1, 1, "2.12:00:00", "2.22:00:00"),
                new Shift(2, 1, "3.12:00:00", "3.22:00:00"),
                new Shift(3, 1, "4.12:00:00", "4.22:00:00"),
                new Exclusion(1, "2009-02-18T12:00:00Z", "2009-02-19T14:00:00Z"));

            Test_Base(16, 0, script,
                DateTime.Parse("2009-02-17T18:00:00") + tzOffset,
                DateTime.Parse("2009-02-19T14:00:00") + tzOffset,
                1);
        }

        #endregion

        #region Bug 33044 Delete Shifts on delete of ShiftType work incorrect

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void RelaunchScheduleScript_LaunchNewScriptConfiguration_CountOfShiftsIsCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(new Action(Action.Operation.SetNewCallPriority, "10"),
                    @"Scheduling2007\Schedule.xml");
            script.Create(null);
            //launch script
            TestScript.Update( script.ScheduleID,
                    new TestScript(
                            new Action(Action.Operation.RecallAfterANumberOfShifts, "1"),
                            new Shift(1, 1, "2.12:00:00", "2.22:00:00"),
                            new Shift(2, 1, "3.12:00:00", "3.22:00:00"),
                            new Shift(3, 1, "4.12:00:00", "4.22:00:00"),
                            new Exclusion(1, "2009-02-18T10:00:00Z", "2009-02-18T14:00:00Z")));

            var size = TestingFramework.DbEngine.ExecuteScalar<int>(@"SELECT COUNT(*) FROM BvShift", CommandType.Text);
            Assert.AreEqual(4, size);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_CallBeforeEndOfShifts_TimeSetSuccessed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TimeSpan tzOffset = TimeSpan.FromHours(-4);

            var script = new TestScript(new Action(Action.Operation.RecallAfterANumberOfMinutes, "60"),
                new Shift(1, 1, "2.12:00:00", "2.22:00:00") );

            Test_Base(16, 0, script,
                DateTime.Parse("2010-03-09T20:59:59.997") + tzOffset,
                DateTime.Parse("2010-03-09T21:59:59.997") + tzOffset,
                1);
        }

        #endregion

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_CallWhenDelayTimeSavingIsOccured1(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "60"),
                new Shift(1, 1, "0.00:00:00", "0.23:59:00"),
                new Shift(2, 1, "1.00:00:00", "1.23:59:00"),
                new Shift(3, 1, "2.00:00:00", "2.23:59:00"),
                new Shift(4, 1, "3.00:00:00", "3.23:59:00"),
                new Shift(5, 1, "4.00:00:00", "4.23:59:00"),
                new Shift(6, 1, "5.00:00:00", "5.23:59:00"),
                new Shift(7, 1, "6.00:00:00", "6.23:59:00"));

            var startTime = TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2010-03-28T01:30:00.000"));

            Test_Base(1, 9, script,
                startTime,
                startTime.AddHours(1),
                1);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_CallWhenDelayTimeSavingIsOccured2(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "60"),
                new Shift(1, 1, "0.00:00:00", "0.23:59:00"),
                new Shift(2, 1, "1.00:00:00", "1.23:59:00"),
                new Shift(3, 1, "2.00:00:00", "2.23:59:00"),
                new Shift(4, 1, "3.00:00:00", "3.23:59:00"),
                new Shift(5, 1, "4.00:00:00", "4.23:59:00"),
                new Shift(6, 1, "5.00:00:00", "5.23:59:00"),
                new Shift(7, 1, "6.00:00:00", "6.23:59:00"));

            var startTime = TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2009-10-25T02:30:00.000"));

            Test_Base(1, 9, script,
                startTime,
                startTime.AddHours(1),
                1);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_CallWhenDelayTimeSavingIsOccured3(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "60"),
                new Shift(1, 1, "0.02:30:00", "0.05:00:00"));

            var startTime = TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2010-03-28T03:34:00.000"));

            Test_Base(1, 9, script,
                startTime,
                startTime.AddHours(1),
                1);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_CallWhenDelayTimeSavingIsOccured4(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "60"),
                new Shift(1, 1, "0.02:30:00", "0.05:00:00"));

            var startTime = TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2009-10-25T03:01:00.000"));

            Test_Base(1, 9, script,
                startTime,
                startTime.AddHours(1),
                1);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_CallWhenDelayTimeSavingIsOccured5(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "60"),
                new Shift(1, 1, "0.04:01:00", "0.05:00:00"));

            var startTime = TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2009-10-25T03:01:00.000"));

            Test_Base(1, 9, script,
                startTime,
                startTime.AddHours(1),
                1);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_CallWhenDelayTimeSavingIsOccured6(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "60"),
                new Shift(1, 1, "0.01:01:00", "0.05:00:00"));

            Test_Base(1, 9, script,
                TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2009-10-25T04:01:00.000")),
                TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2009-11-01T01:01:00.000")),
                1);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_CallWhenDelayTimeSavingIsOccured7(SecurityMode mode)
        {
            SetSecurityMode(mode);
            
            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "01"),
                new Shift(1, 1, "0.01:00:00", "0.03:01:00"));

            Test_Base(1, 9, script,
                TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2010-03-28T01:59:00.000")),
                TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2010-03-28T03:00:00.000")),
                1);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_CallWhenDelayTimeSavingIsOccured8(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "1440"), //one day
                new Shift(1, 1, "0.01:00:00", "0.03:00:00"),
                new Shift(2, 1, "6.01:00:00", "6.03:00:00"));

            Test_Base(1, 9, script,
                TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2010-03-27T01:00:00.000")),
                TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2010-03-28T01:00:00.000")),
                1);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(TestDataGenerator))]
        public void TimeInShift_CallWhenDelayTimeSavingIsOccured9(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "1440"), //one day
                new Shift(1, 1, "0.01:00:00", "0.03:01:00"),
                new Shift(2, 1, "6.01:00:00", "6.03:00:00"));

            Test_Base(1, 9, script,
                TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2010-03-27T02:00:00.000")),
                TimezoneService.ConvertTimeToUtc(9, DateTime.Parse("2010-03-28T03:00:00.000")),
                1);
        }

        [Theory,Owner(@"Firm\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void ShiftFromMondeyToSundey_CallInSunday_CallIsDelivered(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var test = new TestCati2(false, false, false, BackendToolsObject);
            test.CreateSurveyWithPerson(DialingMode.Manual, "user", "pwd", AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "15"), //one day
                new Shift(1, 1, "1.00:00:00", "0.23:59:00"));

            BackendToolsObject.LaunchScript(test.SurveySID, script);
            var interview = test.Interviews[0];
            var call = BackendTools.NewCall(interview);

            BackendTools.FireEvent(new BvInterviewWithOriginEntity(interview), DateTime.Parse("2011-10-02 12:00:00.000"));

            BackendTools.CheckInterview(interview);
            call.ShiftID = script.GetShiftTypeWorkID(1);
            call.TimeInShift = DateTime.Parse("2011-10-02 12:15:00.000");
            //BackendTools.CheckCall(call); 

            test.Login("user", "pwd", AgentTaskChoiceMode.Automatic, false);
            var now = DateTime.Parse("2011-10-02 13:00:00.000");
            BvSpQueueUpSheduleTask3Adapter.ExecuteNonQuery(now, _timezoneId, 0);

            var deliveredInterview = test.StartInterview_ManualOrPreview(null, 0);
            Assert.IsNotNull(deliveredInterview);
        }

        [Theory, Owner(@"Firm\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void AllHoursWithWindowOnTuesday_CallInTuesday_CallIsNotDelivered(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var test = new TestCati2(false, false, false, BackendToolsObject);
            test.CreateSurveyWithPerson(DialingMode.Manual, "user", "pwd", AgentTaskChoiceMode.Automatic);

            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "15"), //one day
                new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                new Shift(2, 2, "1.00:00:00", "2.00:00:00"),
                new Shift(3, 3, "2.00:00:00", "2.00:01:00"),
                new Shift(4, 4, "3.12:00:00", "4.00:00:00"),
                new Shift(5, 5, "4.00:00:00", "5.00:00:00"),
                new Shift(6, 6, "5.00:00:00", "6.00:00:00"),
                new Shift(7, 7, "6.00:00:00", "0.00:00:00"));

            BackendToolsObject.LaunchScript(test.SurveySID, script);

            var interview = BackendTools.NewInterview(test.SurveySID);
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            call.ShiftID = 0;//Any valid
            BackendTools.CreateCall(call);

            test.Login("user", "pwd", AgentTaskChoiceMode.Automatic, false);
            var now = DateTime.Parse("2011-10-18 17:00:00.000");
            BvSpQueueUpSheduleTask3Adapter.ExecuteNonQuery(now, _timezoneId, 0);
            
            test.WS.StartInterview(null, 0);
            var state = test.WaitState(x => x.interviewState == (int)InterviewState.NO_CALLS || x.interviewState == (int)InterviewState.INTERVIEWING);
            Assert.AreEqual((int)InterviewState.NO_CALLS, state.interviewState);
        }
    }
}
