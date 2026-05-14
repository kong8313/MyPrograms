using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class ShiftTypeCollectionTest
    {
        private SchedulingObjectValidator _validator;

        [TestInitialize]
        public void TestInitialize()
        {
            _validator = new SchedulingObjectValidator(null);
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
            ServiceLocator.RegisterInstance<ISchedulingObjectValidator>(_validator);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullValue_ExceptionThrows()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            coll.Add(null);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_InvalidObject_ExceptionThrows()
        {
            var coll = new ShiftTypeCollection();
            var shiftType = new ShiftType();

            coll.Add(shiftType);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Add_ValidObject_Success()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            ShiftType shiftType = new ShiftType();
            shiftType.Id = 1;
            shiftType.Name = "Shift type 1";
            shiftType.Color = Color.Blue;

            coll.Add(shiftType);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Insert_NullValue_ExceptionThrows()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            coll.Insert(0, null);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Insert_IndexOutOfRange_ExceptionThrows()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            ShiftType shiftType = new ShiftType
            {
                Id = 1,
                Name = "name",
                Color = Color.AliceBlue
            };

            coll.Insert(10, shiftType);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void Insert_InvalidObject_ExceptionThrows()
        {
            var coll = new ShiftTypeCollection();
            var shiftType = new ShiftType();

            coll.Insert(0, shiftType);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Insert_ValidObject_Success()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            ShiftType shiftType = new ShiftType();
            shiftType.Id = 1;
            shiftType.Name = "Shift type 1";
            shiftType.Color = Color.Blue;

            coll.Insert(0, shiftType);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThisSetter_NullValue_ExceptionThrows()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            coll[0] = null;
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThisSetter_IndexOutOfRange_ExceptionThrows()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            ShiftType shiftType = new ShiftType
            {
                Id = 1,
                Name = "name",
                Color = Color.AliceBlue
            };

            coll[0] = shiftType;
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void ThisSetter_InvalidObject_ExceptionThrows()
        {
            var coll = new ShiftTypeCollection();
            var shiftType = new ShiftType();

            coll[0] = shiftType;
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ThisSetter_ValidObject_Success()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            ShiftType shiftType = new ShiftType();
            shiftType.Id = 1;
            shiftType.Name = "Shift type 1";
            shiftType.Color = Color.Blue;

            coll.Add(shiftType);
            coll[0] = shiftType;
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Remove_NullValue_ExceptionThrows()
        {
            ErrorCollection errors;
            ShiftTypeCollection coll = new ShiftTypeCollection();
            coll.Remove(null, out errors);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Remove_ItemDoesNotExists_RemoveFails()
        {
            ErrorCollection errors;
            ShiftTypeCollection coll = new ShiftTypeCollection();
            ShiftType shiftType = new ShiftType();

            Assert.IsFalse(coll.Remove(shiftType, out errors));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Remove_ItemExists_RemoveSuccess()
        {
            ErrorCollection errors;
            ShiftTypeCollection coll = new ShiftTypeCollection();
            ShiftType shiftType = new ShiftType();
            shiftType.Id = 1;
            shiftType.Name = "Shift type 1";
            shiftType.Color = Color.Blue;

            coll.Add(shiftType);
            Assert.IsTrue(coll.Remove(shiftType, out errors));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetItemById_ItemDoesNotExist_Fail()
        {
            var coll = new ShiftTypeCollection();
            var shiftType1 = new ShiftType { Id = 1, Name = "name1", Color = Color.AliceBlue };
            var shiftType2 = new ShiftType { Id = 2, Name = "name2", Color = Color.AliceBlue };

            coll.Add(shiftType1);
            coll.Add(shiftType2);

            Assert.IsNull(coll.GetItemById(10));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetItemById_ItemExists_Success()
        {
            var coll = new ShiftTypeCollection();
            var shiftType1 = new ShiftType { Id = 1, Name = "name1", Color = Color.AliceBlue };
            var shiftType2 = new ShiftType { Id = 2, Name = "name2", Color = Color.AliceBlue };

            coll.Add(shiftType1);
            coll.Add(shiftType2);

            ShiftType shiftType3 = coll.GetItemById(shiftType2.Id.Value);
            Assert.IsNotNull(shiftType3);
            Assert.IsTrue(shiftType3.Id.Value == 2);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void RemoveById_ItemDoesNotExist_Fail()
        {
            var coll = new ShiftTypeCollection();
            var shiftType1 = new ShiftType { Id = 1, Name = "name1", Color = Color.AliceBlue };
            var shiftType2 = new ShiftType { Id = 2, Name = "name2", Color = Color.AliceBlue };
            ErrorCollection errors;

            coll.Add(shiftType1);
            coll.Add(shiftType2);

            Assert.IsFalse(coll.RemoveById(10, out errors));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void RemoveById_ItemExists_Success()
        {
            var coll = new ShiftTypeCollection();
            var shiftType1 = new ShiftType { Id = 1, Name = "name1", Color = Color.AliceBlue };
            var shiftType2 = new ShiftType { Id = 2, Name = "name2", Color = Color.AliceBlue };
            ErrorCollection errors;

            coll.Add(shiftType1);
            coll.Add(shiftType2);

            Assert.IsTrue(coll.RemoveById(shiftType2.Id.Value, out errors));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Swap_InvalidIndex_ExceptionThrows()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            coll.Swap(1, 2);
        }


        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Swap_SameIndex_DoNothing()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            ShiftType shiftType1 = new ShiftType();
            shiftType1.Id = 1;
            shiftType1.Name = "Shift type";
            shiftType1.Color = Color.Blue;
            coll.Add(shiftType1);

            coll.Swap(0, 0);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Swap_CorrectIndices_Success()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            ShiftType shiftType1 = new ShiftType();
            shiftType1.Id = 1;
            shiftType1.Name = "Shift type 1";
            shiftType1.Color = Color.Blue;
            ShiftType shiftType2 = new ShiftType();
            shiftType2.Id = 2;
            shiftType2.Name = "Shift type 2";
            shiftType2.Color = Color.Blue;
            ShiftType shiftType3 = new ShiftType();
            shiftType3.Id = 3;
            shiftType3.Name = "Shift type 3";
            shiftType3.Color = Color.Blue;
            ShiftType shiftType4 = new ShiftType();
            shiftType4.Id = 4;
            shiftType4.Name = "Shift type 4";
            shiftType4.Color = Color.Blue;
            coll.Add(shiftType1);
            coll.Add(shiftType2);
            coll.Add(shiftType3);
            coll.Add(shiftType4);

            coll.Swap(2, 0);

            Assert.IsTrue(coll[0].Id == 3);
            Assert.IsTrue(coll[1].Id == 2);
            Assert.IsTrue(coll[2].Id == 1);
            Assert.IsTrue(coll[3].Id == 4);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetNewId_EmptyCollection_Returns1()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();

            Assert.IsTrue(coll.GetNewId() == 1);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetNewId_FilledCollection_Success()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            ShiftType shiftType = new ShiftType();
            shiftType.Id = 1;
            shiftType.Name = "Shift type 1";
            shiftType.Color = Color.Blue;

            coll.Add(shiftType);

            Assert.IsTrue(coll.GetNewId() > 1);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetNewId_CollectionWithRemovedItems_Success()
        {
            ShiftTypeCollection coll = new ShiftTypeCollection();
            ShiftType shiftType = new ShiftType();
            shiftType.Id = 1;
            shiftType.Name = "Shift type 1";
            shiftType.Color = Color.Blue;

            ShiftType shiftType2 = new ShiftType();
            shiftType2.Id = 2;
            shiftType2.Name = "Shift type 2";
            shiftType2.Color = Color.Chartreuse;
            ErrorCollection errors;

            coll.Add(shiftType);
            coll.Add(shiftType2);
            coll.Remove(shiftType2, out errors);

            Assert.IsTrue(coll.GetNewId() > 2);
        }
    }
}