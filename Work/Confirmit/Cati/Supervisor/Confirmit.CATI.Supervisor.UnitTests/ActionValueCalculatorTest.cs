using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.CallManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class ActionValueCalculatorTest
    {
        private int _expectedShiftId;

        private ActionValueCalculator _calculator;

        [TestInitialize]
        public void TestInitialize()
        {
            _expectedShiftId = 7;
            _calculator = new ActionValueCalculator(new CallMemoryProvider(
                new BvCallEntity { ShiftID = _expectedShiftId }));
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void Calculate_OneEntityPassed_CorrectValueOfThisEntityIsReturned()
        {
            var ids = new List<int> { 1 };
            var value = _calculator.Calculate(1, ids, entity => entity.ShiftID, 1);
            Assert.AreEqual(_expectedShiftId, value);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void Calculate_TwoEntitiesPassed_DefaultValueOfThisEntityIsReturned()
        {
            var ids = new List<int> { 1, 2 };
            var value = _calculator.Calculate(1, ids, entity => entity.ShiftID, 0);
            Assert.AreEqual(0, value);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Calculate_NoEntitiesPassed_ExceptionIsThrown()
        {
            var ids = new List<int>();
            _calculator.Calculate(1, ids, entity => entity.ShiftID, 0);
        }
    }
}
