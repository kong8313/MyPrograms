using System;
using Confirmit.CATI.Common.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class SchedulingUtilitiesTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
            ServiceLocator.RegisterInstance<ISchedulingObjectValidator>(new SchedulingObjectValidator(null));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void CompareDayOfWeekTimePare_CorrectPares_FirstGreater()
        {
            Assert.IsTrue(
                SchedulingUtilities.CompareDayOfWeekTimePare(
                    DayOfWeek.Monday,
                    new TimeSpan( 10, 0, 0 ),
                    DayOfWeek.Monday,
                    new TimeSpan( 9, 0, 0 )
                    ) > 0
                );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void CompareDayOfWeekTimePare_CorrectPares_SecondGreater()
        {
            Assert.IsTrue(
                SchedulingUtilities.CompareDayOfWeekTimePare(
                    DayOfWeek.Monday,
                    new TimeSpan( 10, 0, 0 ),
                    DayOfWeek.Wednesday,
                    new TimeSpan( 9, 0, 0 )
                    ) < 0
                );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void CompareDayOfWeekTimePare_CorrectPares_Equal()
        {
            Assert.AreEqual<int>(
                SchedulingUtilities.CompareDayOfWeekTimePare(
                    DayOfWeek.Monday,
                    new TimeSpan( 10, 0, 0 ),
                    DayOfWeek.Monday,
                    new TimeSpan( 10, 0, 0 )
                    ),
                0
                );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetSubRuleById_WrongScheduleParameter_ExceptionThrows()
        {
            SchedulingUtilities.GetSubRuleById( null, Guid.Empty );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetSubRuleById_UnexistingSubRuleId_ReturnsNull()
        {
            Assert.IsNull( 
                SchedulingUtilities.GetSubRuleById(
                    ScheduleCreator.GetSchedule(),
                    Guid.Empty
                    )
                );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetSubRuleById_ExistingSubRuleId_ReturnsSubRule()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            SubRule subRule = schedule.Rules[0].SubRules[0];
            SubRule found = SchedulingUtilities.GetSubRuleById( schedule, subRule.Id.Value);

            Assert.AreEqual<SubRule>( subRule, found );
        }
    }
}