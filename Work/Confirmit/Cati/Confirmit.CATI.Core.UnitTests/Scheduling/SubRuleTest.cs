using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class SubRuleTest
    {
        private ISchedulingObjectValidator _validator;

        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
            _validator = new SchedulingObjectValidator(null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_UninitializedObject_Fails()
        {
            ErrorCollection errors;
            SubRule subRule = new SubRule();

            Assert.IsFalse(_validator.Validate(subRule, out errors));
            Assert.AreEqual<int>(errors.Count, 1);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_InitializedObject_Success()
        {
            ErrorCollection errors;
            SubRule subRule = new SubRule();
            subRule.Id = Guid.NewGuid();

            Assert.IsTrue(_validator.Validate(subRule, out errors));
        }
    }
}