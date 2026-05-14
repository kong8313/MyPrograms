using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class CustomParameterCollectionTest
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

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void RemoveById_RemovingUnusedCustomParameter_Success()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            ErrorCollection errors;
            bool result = schedule.CustomParameters.RemoveById(2, out errors);

            Assert.IsTrue(result);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void RemoveById_RemovingUsedCustomParameter_Fails()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            ErrorCollection errors;
            bool result = schedule.CustomParameters.RemoveById(1, out errors);

            Assert.IsFalse(result);
            Assert.AreEqual<int>(errors.Count, 1);
        }
    }
}
