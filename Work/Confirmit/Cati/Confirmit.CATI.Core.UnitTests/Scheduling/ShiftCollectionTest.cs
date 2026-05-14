using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core.Timezones.Fakes;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class ShiftCollectionTest : BaseTest
    {
        private SchedulingObjectValidator _validator;
        private StubITimezoneManager _timezoneManager = new StubITimezoneManager();

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            _validator = new SchedulingObjectValidator(_timezoneManager);
            _timezoneManager.TimezonesListGet = ()=> new BvTimezoneEntityCollection();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetItemsForTimezone_EmptyCollection_ReturnsEmptyCollection()
        {
            ShiftCollection shiftCollection = new ShiftCollection();
            ShiftCollection result = shiftCollection.GetItemsForTimezone<ShiftCollection>(1);

            Assert.AreEqual<int>(result.Count, 0);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetItemsForTimezone_FilledCollection_Success()
        {
            ShiftCollection coll = ScheduleCreator.GetSchedule().Shifts;
            ShiftCollection result = coll.GetItemsForTimezone<ShiftCollection>(2);

            Assert.AreEqual<int>(result.Count, 1);
            Assert.AreEqual<int>(result[0].Id.Value, 2);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void RemoveById_RemovingUnusedShift_Success()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            ErrorCollection errors;
            bool result = schedule.ShiftTypes.RemoveById(4, out errors);

            Assert.IsTrue(result);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void RemoveById_RemovingUsedShift_Fails()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            ErrorCollection errors;
            bool result = schedule.ShiftTypes.RemoveById(1, out errors);

            Assert.IsFalse(result);
            Assert.AreEqual<int>(errors.Count, 3);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetNewId_CollectionWithRemovedItems_Success()
        {
            ShiftCollection coll = new ShiftCollection();
            Shift shift = new Shift();
            shift.Id = 1;
            shift.ShiftTypeId = 1;

            Shift shift2 = new Shift();
            shift2.Id = 2;
            shift2.ShiftTypeId = 2;
            ErrorCollection errors;

            coll.Add(shift);
            coll.Add(shift2);
            coll.Remove(shift2, out errors);

            Assert.IsTrue(coll.GetNewId() > 2);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void OnItemValidate_NullShift_Fails()
        {
            ShiftCollection coll = new ShiftCollection();
            ValidationEventArgs e = new ValidationEventArgs(false);
            ErrorCollection errors;
            var result = _validator.ValidateWithCollection(coll, null, out errors);

            Assert.AreEqual(false, result);
            Assert.AreEqual<int>(errors.Count, 1);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void OnItemValidate_NotInitializedShift_Fails()
        {
            ShiftCollection coll = new ShiftCollection();
            Shift shift = new Shift();
            ErrorCollection errors;
            var result = _validator.ValidateWithCollection(coll, shift, out errors);

            Assert.AreEqual(false, result);
            Assert.AreEqual<int>(errors.Count, 2);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void OnItemValidate_IntersectedShifts_Fails()
        {
            ShiftCollection coll = new ShiftCollection();
            Shift shift1 = new Shift();
            shift1.Id = 1;
            shift1.ShiftTypeId = 1;
            shift1.SetDataForTimezone(
                Shift.RespondentTimezoneId,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(10, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(17, 0, 0)));
            shift1.SetDataForTimezone(
                1,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(17, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(18, 0, 0)));

            coll.Add(shift1);

            Shift shift2 = new Shift();
            shift2.Id = 2;
            shift2.ShiftTypeId = 1;
            shift2.SetDataForTimezone(
                Shift.RespondentTimezoneId,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(17, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(18, 0, 0)));

            ValidationEventArgs e = new ValidationEventArgs(false);
            
            ErrorCollection errors;
            var result = _validator.ValidateWithCollection(coll, shift2, out errors);

            Assert.AreEqual(false, result);
            Assert.AreEqual(errors.Count, 1);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void OnItemValidate_CorrectShifts_Success()
        {
            ShiftCollection coll = new ShiftCollection();
            Shift shift1 = new Shift();
            shift1.Id = 1;
            shift1.ShiftTypeId = 1;
            shift1.SetDataForTimezone(
                Shift.RespondentTimezoneId,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(10, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(17, 0, 0)));
            shift1.SetDataForTimezone(
                1,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(17, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(18, 0, 0)));

            coll.Add(shift1);

            Shift shift2 = new Shift();
            shift2.Id = 2;
            shift2.ShiftTypeId = 1;
            shift2.SetDataForTimezone(
                Shift.RespondentTimezoneId,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(17, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(18, 0, 0)));
            shift2.SetDataForTimezone(
                1,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(14, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(15, 0, 0)));

            ErrorCollection errors;
            var result = _validator.ValidateWithCollection(coll, shift2, out errors);

            Assert.AreEqual(true, result);
            Assert.AreEqual(errors.Count, 0);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void OnItemValidate_SkipExistingShift_Success()
        {
            ShiftCollection coll = new ShiftCollection();
            Shift shift1 = new Shift();
            shift1.Id = 1;
            shift1.ShiftTypeId = 1;
            shift1.SetDataForTimezone(
                Shift.RespondentTimezoneId,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(10, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(17, 0, 0)));
            shift1.SetDataForTimezone(
                1,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(17, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(18, 0, 0)));

            coll.Add(shift1);

            ErrorCollection errors;
            var result = _validator.ValidateWithCollection(coll, shift1, out errors);

            Assert.AreEqual(true, result);
            Assert.AreEqual(errors.Count, 0);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void OnItemValidate_IntersectedShiftsRespondentNonRespondent_Fails()
        {
            ShiftCollection coll = new ShiftCollection();
            Shift shift1 = new Shift();
            shift1.Id = 1;
            shift1.ShiftTypeId = 1;
            shift1.SetDataForTimezone(
                Shift.RespondentTimezoneId,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(10, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(12, 0, 0)
                    )
                );

            coll.Add(shift1);

            Shift shift2 = new Shift();
            shift2.Id = 2;
            shift2.ShiftTypeId = 1;
            shift2.SetDataForTimezone(
                2,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(10, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(12, 0, 0)
                    )
                );

            ValidationEventArgs e = new ValidationEventArgs(false);
            
            ErrorCollection errors;
            var result = _validator.ValidateWithCollection(coll, shift2, out errors);

            Assert.AreEqual(false, result);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void OnItemValidate_IntersectedShiftsWithoutRespondent_Fails()
        {
            ShiftCollection coll = new ShiftCollection();
            Shift shift1 = new Shift();
            shift1.Id = 1;
            shift1.ShiftTypeId = 1;
            shift1.SetDataForTimezone(
                2,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(10, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(12, 0, 0)
                    )
                );

            coll.Add(shift1);

            Shift shift2 = new Shift();
            shift2.Id = 2;
            shift2.ShiftTypeId = 1;
            shift2.SetDataForTimezone(
                2,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(11, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(13, 0, 0)
                    )
                );

            ValidationEventArgs e = new ValidationEventArgs(false);
            
            ErrorCollection errors;
            var result = _validator.ValidateWithCollection(coll, shift2, out errors);

            Assert.AreEqual(false, result);
        }


        [TestMethod, Owner(@"FIRM\DenisM")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddShiftToCollection_IncorrectShift_ExceptionThrown()
        {
            var shiftCollection = new ShiftCollection();
            var shift = new Shift
            {
                Id = 1
            };
            
            shiftCollection.Add(shift);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddShiftToCollectionWithIndexer_IncorrectShift_ExceptionThrown()
        {
            var shiftCollection = new ShiftCollection();
            var shift = new Shift
            {
                Id = 1
            };

            var correctShift = new Shift();
            correctShift.Id = 1;
            correctShift.ShiftTypeId = 1;
            correctShift.SetDataForTimezone(
                2,
                new ShiftData(
                    DayOfWeek.Monday,
                    new TimeSpan(10, 0, 0),
                    DayOfWeek.Monday,
                    new TimeSpan(12, 0, 0)
                    )
                );

            shiftCollection.Add(correctShift);

            shiftCollection[0] = shift;
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        [ExpectedException(typeof(ArgumentException))]
        public void InsertShiftToCollection_IncorrectShift_ExceptionThrown()
        {
            var shiftCollection = new ShiftCollection();
            var shift = new Shift
            {
                Id = 1
            };

            shiftCollection.Insert(0, shift);
        }
    }
}
