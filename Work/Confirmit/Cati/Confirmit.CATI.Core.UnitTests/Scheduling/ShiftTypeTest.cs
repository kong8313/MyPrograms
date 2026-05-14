using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class ShiftTypeTest
    {
        private SchedulingObjectValidator _validator;

        [TestInitialize]
        public void TestInitialiaze()
        {
            _validator = new SchedulingObjectValidator(null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_UninitializedObject_Fails()
        {
            ErrorCollection errors;
            ShiftType shiftType = new ShiftType();

            Assert.IsFalse(_validator.Validate(shiftType, out errors));
            Assert.AreEqual<int>( errors.Count, 3 );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_InitializedObject_Success()
        {
            ErrorCollection errors;
            ShiftType shiftType = new ShiftType();
            shiftType.Id = 1;
            shiftType.Name = "Shift type";
            shiftType.Color = Color.Black;

            Assert.IsTrue(_validator.Validate(shiftType, out errors));
        }
    }
}