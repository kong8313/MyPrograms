using System;
using Confirmit.CATI.Common.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class RuleTest
    {
        private SchedulingObjectValidator _validator;

        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
            ServiceLocator.RegisterInstance<ISchedulingObjectValidator>(new SchedulingObjectValidator(null));

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
            Rule rule = new Rule();

            Assert.IsFalse(_validator.Validate(rule, out errors));
            Assert.AreEqual(1, errors.Count);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_InitializedObject_Success()
        {
            ErrorCollection errors;
            Rule rule = new Rule();
            rule.Id = Guid.NewGuid();

            Assert.IsTrue(_validator.Validate(rule, out errors));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void OnRemovingSubRule_UnusedRule_Success()
        {
            Rule rule = ScheduleCreator.GetSchedule().Rules[1];
            ErrorCollection errors;
            Assert.IsTrue( rule.SubRules.RemoveAt( 1, out errors ) );
        }
    }
}