using System;
using System.Collections.Generic;
using System.Globalization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Timezones;
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
    public class FiltersByShiftTypeID : BaseMockedIntegrationTest
    {
        private void CallManagementSchedule_TestBase(
            IEnumerable<int> activeTzList,
            Shift[] shifts,
            int tzId,
            DateTime schedulingTime,
            int shiftTypeID,
            bool filterResult)
        {
            const int initITS = (int)CallOutcome.FreshSample;
            const int newITS = 40;/*Custom10*/

            foreach (var id in activeTzList)
                TimezoneManager.AddTimezone(id);

            var script = new TestScript(
                new SubRule( new Action(Action.Operation.SetNewITS, newITS.ToString(CultureInfo.InvariantCulture) ) )
                { 
                    ShiftTypeId = shiftTypeID
                }, shifts);

            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            interview.TimezoneID = tzId;
            interview.TransientState = initITS;
            interview.LastCallTime = TimezoneManager.ConvertToUTC(
                ServiceLocator.Resolve<ITimezoneService>().GetTimezoneIdOrDefaultCallCenterTimezoneId(tzId),
                schedulingTime);

            BackendTools.CreateInterview(interview);

            BackendTools.FireEvent( interview );
            
            if( filterResult )
                interview.TransientState = newITS;

            BackendTools.CheckInterview(interview);
            Assert.IsFalse( BackendTools.IsCallExists( interview.SurveySID, interview.ID ));
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewInDefTz_TimeInOutOfShifts_FilterFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                    new[]{ 6 }, 
                    new[]{ 
                            new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                            new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                            new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                            new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                            },
                    0,
                    DateTime.Parse("2010-01-24T07:00:00")/*Time in interview tz*/,
                    1,
                    false );
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewInDefTz_TimeOnStartShift_FilterTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                    new[] { 6 },
                    new[]{ 
                            new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                            new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                            new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                            new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                            },
                    0,
                    DateTime.Parse("2010-01-24T08:00:00")/*Time in interview tz*/,
                    1,
                    true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewInDefTz_TimeBeforeEndShift_FilterTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                    new[] { 6 },
                    new[]{ 
                            new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                            new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                            new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                            new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                            },
                    0,
                    DateTime.Parse("2010-01-24T19:59:59")/*Time in interview tz*/,
                    1,
                    true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewInDefTz_TimeOnEndShift_FilterFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                    new[] { 6 },
                    new[]{ 
                            new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                            new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                            new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                            new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                            },
                    0,
                    DateTime.Parse("2010-01-24T20:00:00")/*Time in interview tz*/,
                    1,
                    false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewInDefTz_TimeOnOtherShift_FilterFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                    new[] { 6 },
                    new[]{ 
                            new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                            new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                            new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                            new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                            },
                    0,
                    DateTime.Parse("2010-01-25T12:00:00")/*Time in interview tz*/,
                    1,
                    false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewInDefTz_TimeAfterEndShift_FilterFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                    new[] { 6 },
                    new[]{ 
                            new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                            new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                            new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                            new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                            },
                    0,
                    DateTime.Parse("2010-01-24T21:00:00")/*Time in interview tz*/,
                    1,
                    false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewIn6Tz_TimeInOutOfShifts_FilterFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                    new[] { 6 },
                    new[]{ 
                            new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                            new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                            new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                            new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                            },
                    6,
                    DateTime.Parse("2010-01-24T09:00:00")/*Time in interview tz*/,
                    1,
                    false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewIn6Tz_TimeOnStartShift_FilterTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                    new[] { 6 },
                    new[]{ 
                            new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                            new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                            new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                            new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                            },
                    6,
                    DateTime.Parse("2010-01-24T10:00:00")/*Time in interview tz*/,
                    1,
                    true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewIn6Tz_TimeBeforeEndShift_FilterTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                    new[] { 6 },
                    new[]{ 
                            new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                            new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                            new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                            new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                            },
                    6,
                    DateTime.Parse("2010-01-24T17:59:59")/*Time in interview tz*/,
                    1,
                    true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewIn6Tz_TimeOnEndShift_FilterFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                    new[] { 6 },
                    new[]{ 
                            new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                            new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                            new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                            new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                            },
                    6,
                    DateTime.Parse("2010-01-24T18:00:00")/*Time in interview tz*/,
                    1,
                    false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewIn6Tz_TimeAfterEndShift_FilterFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                    new[] { 6 },
                    new[]{ 
                            new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                            new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                            new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                            new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                            },
                    6,
                    DateTime.Parse("2010-01-24T19:00:00")/*Time in interview tz*/,
                    1,
                    false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewIn6Tz_TimeOnOtherShift_FilterFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            CallManagementSchedule_TestBase(
                new[] {6},
                new[]
                {
                    new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                        new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                    new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                        new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))
                },
                6,
                DateTime.Parse("2010-01-25T12:00:00") /*Time in interview tz*/,
                1,
                false);
        }
    }
}
