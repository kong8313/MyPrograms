using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class ShiftTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
            ServiceLocator.RegisterInstance(new SchedulingObjectValidator(null));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetDataForTimezone_InvalidTimezone_ExceptionThrows()
        {
            Shift shift = new Shift();
            ShiftData shiftData = new ShiftData( DayOfWeek.Monday, new TimeSpan( 10, 0, 0 ),
                                                 DayOfWeek.Monday, new TimeSpan( 12, 0, 0 ) );
            shift.SetDataForTimezone( 1, shiftData );

            shift.GetDataForTimezone( 2 );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetDataForTimezone_CorrectData_Success()
        {
            Shift shift = new Shift();
            ShiftData shiftData = new ShiftData( DayOfWeek.Monday, new TimeSpan( 10, 0, 0 ),
                                                 DayOfWeek.Monday, new TimeSpan( 12, 0, 0 ) );
            shift.SetDataForTimezone( 1, shiftData );

            ShiftData data = shift.GetDataForTimezone( 1 );

            Assert.AreEqual( data.StartDayOfWeek.Value, shiftData.StartDayOfWeek.Value);
            Assert.AreEqual( data.StartTime.Value, shiftData.StartTime.Value );
            Assert.AreEqual( data.EndDayOfWeek.Value, shiftData.EndDayOfWeek.Value );
            Assert.AreEqual( data.EndTime.Value, shiftData.EndTime.Value );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SetDataForTimezone_ValidData_Success()
        {
            Shift shift = new Shift();
            ShiftData shiftData = new ShiftData( DayOfWeek.Monday, new TimeSpan( 12, 0, 0 ),
                                                 DayOfWeek.Monday, new TimeSpan( 13, 0, 0 ) );
            shift.SetDataForTimezone( Shift.RespondentTimezoneId, shiftData );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetTimezoneIds_EmptyTimezoneCollection_Success()
        {
            Shift shift = new Shift();
            int[] result = shift.GetTimezoneIds();

            Assert.IsTrue( result.Length == 0 );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetTimezoneIds_TwoElementTimezoneCollection_Success()
        {
            Shift shift = new Shift();
            ShiftData shiftData1 = new ShiftData( DayOfWeek.Monday, new TimeSpan(12,0,0),
                                                  DayOfWeek.Wednesday, new TimeSpan( 12, 0, 0 ) );
            ShiftData shiftData2 = new ShiftData( DayOfWeek.Wednesday, new TimeSpan( 12, 0, 0 ),
                                                  DayOfWeek.Friday, new TimeSpan( 12, 0, 0 ) );
            shift.SetDataForTimezone( Shift.RespondentTimezoneId, shiftData1 );
            shift.SetDataForTimezone( 1, shiftData2 );

            int[] result = shift.GetTimezoneIds();

            Assert.IsTrue( result.Length == 2 );

            List<int> tmp = new List<int>( result );

            Assert.IsTrue( tmp.Contains( Shift.RespondentTimezoneId ) &&
                           tmp.Contains( 1 ) );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void TryGetDataForTimezone_NonExistingTimezoneWithoutRespondent_Fails()
        {
            Shift shift = new Shift();
            ShiftData data;

            Assert.IsFalse( shift.TryGetDataForTimezone( 1, out data ) );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void TryGetDataForTimezone_NonExistingTimezoneWithRespondent_ReturnsRespondentData()
        {
            Shift shift = new Shift();
            ShiftData shiftData = new ShiftData(
                DayOfWeek.Monday,
                new TimeSpan( 10, 0, 0 ),
                DayOfWeek.Monday,
                new TimeSpan( 11, 0, 0 )
                );
            shift.SetDataForTimezone( Shift.RespondentTimezoneId, shiftData );
            ShiftData shiftData2 = new ShiftData(
                DayOfWeek.Tuesday,
                new TimeSpan( 10, 0, 0 ),
                DayOfWeek.Tuesday,
                new TimeSpan( 11, 0, 0 )
                );
            shift.SetDataForTimezone( 2, shiftData2 );

            ShiftData data;

            Assert.IsTrue( shift.TryGetDataForTimezone( 1, out data ) );
            Assert.IsTrue( data.StartDayOfWeek == DayOfWeek.Monday );			
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void TryGetDataForTimezone_ExistingTimezone_ReturnsTimezoneData()
        {
            Shift shift = new Shift();
            ShiftData shiftData = new ShiftData(
                DayOfWeek.Monday,
                new TimeSpan( 10, 0, 0 ),
                DayOfWeek.Monday,
                new TimeSpan( 11, 0, 0 )
                );
            shift.SetDataForTimezone( Shift.RespondentTimezoneId, shiftData );
            ShiftData shiftData2 = new ShiftData(
                DayOfWeek.Tuesday,
                new TimeSpan( 10, 0, 0 ),
                DayOfWeek.Tuesday,
                new TimeSpan( 11, 0, 0 )
                );
            shift.SetDataForTimezone( 2, shiftData2 );

            ShiftData data;

            Assert.IsTrue( shift.TryGetDataForTimezone( 2, out data ) );
            Assert.IsTrue( data.StartDayOfWeek == DayOfWeek.Tuesday );
        }
    }
}