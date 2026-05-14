using System;
using Confirmit.CATI.Common.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class ShiftDataTest
    {
        private ISchedulingObjectValidator _validator;


        [TestInitialize]
        public void TestInitialiaze()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
            _validator = new SchedulingObjectValidator(null);
            ServiceLocator.RegisterInstance(_validator);

        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_UninitializedObject_ValidationFails()
        {
            ShiftData data = new ShiftData();
            ErrorCollection errors;

            Assert.IsFalse(_validator.Validate(data, out errors));
            Assert.IsTrue(errors.Count == 2);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_CorrectPeriod_ValidationSuccess()
        {
            DayOfWeek startDay = DayOfWeek.Monday;
            TimeSpan startTime = new TimeSpan(12, 0, 0);
            DayOfWeek endDay = DayOfWeek.Monday;
            TimeSpan endTime = new TimeSpan(13, 0, 0);

            ErrorCollection errors;
            ShiftData data = new ShiftData(startDay, startTime, endDay, endTime);

            Assert.IsTrue(_validator.Validate(data, out errors));
            Assert.IsTrue(errors.Count == 0);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void HasIntersection_IntersectedObjects_ReturnsTrue()
        {
            ShiftData data1 = new ShiftData(
                DayOfWeek.Monday,
                new TimeSpan( 10, 0, 0 ),
                DayOfWeek.Friday,
                new TimeSpan( 10, 0, 0 )
                );

            ShiftData data2 = new ShiftData(
                DayOfWeek.Thursday,
                new TimeSpan( 10, 0, 0 ),
                DayOfWeek.Friday,
                new TimeSpan( 10, 0, 0 )
                );

            Assert.IsTrue( data1.HasIntersection( data2 ) );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void HasIntersection_NonintersectedObjects_ReturnsFalse()
        {
            ShiftData data1 = new ShiftData(
                DayOfWeek.Monday,
                new TimeSpan( 10, 0, 0 ),
                DayOfWeek.Tuesday,
                new TimeSpan( 10, 0, 0 )
                );

            ShiftData data2 = new ShiftData(
                DayOfWeek.Wednesday,
                new TimeSpan( 10, 0, 0 ),
                DayOfWeek.Friday,
                new TimeSpan( 10, 0, 0 )
                );

            Assert.IsFalse( data1.HasIntersection( data2 ) );
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void HasIntersection_IntersectedCrossWeekendObjects_Exception()
        {
            ShiftData data1 = new ShiftData(
                DayOfWeek.Tuesday,
                new TimeSpan(10, 0, 0),
                DayOfWeek.Friday,
                new TimeSpan(10, 0, 0)
                );

            ShiftData data2 = new ShiftData(
                DayOfWeek.Thursday,
                new TimeSpan(10, 0, 0),
                DayOfWeek.Monday,
                new TimeSpan(10, 0, 0)
                );

            data1.HasIntersection(data2);
        }

        [ExpectedException(typeof(ApplicationException))]
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void HasIntersection_NonintersectedCrossWeekendObjects_Exception()
        {
            ShiftData data1 = new ShiftData(
                DayOfWeek.Saturday,
                new TimeSpan(10, 0, 0),
                DayOfWeek.Tuesday,
                new TimeSpan(10, 0, 0)
                );

            ShiftData data2 = new ShiftData(
                DayOfWeek.Wednesday,
                new TimeSpan(10, 0, 0),
                DayOfWeek.Friday,
                new TimeSpan(10, 0, 0)
                );

            data1.HasIntersection(data2);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void HasIntersection_NonintersectedEndsOnTheWeekStart_False()
        {
            ShiftData data1 = new ShiftData(
                DayOfWeek.Tuesday,
                new TimeSpan(10, 0, 0),
                DayOfWeek.Wednesday,
                new TimeSpan(10, 0, 0)
                );

            ShiftData data2 = new ShiftData(
                DayOfWeek.Thursday,
                new TimeSpan(10, 0, 0),
                DayOfWeek.Monday,
                new TimeSpan(0, 0, 0)
                );

            Assert.IsFalse(data1.HasIntersection(data2));
        }
    }
}