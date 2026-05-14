using Confirmit.CATI.Common.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.Core.ScheduleDom.Script.Action;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.ScheduleDom.Script;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class SubRuleActionTest : BaseTest
    {
        private SchedulingObjectValidator _validator;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            _validator = new SchedulingObjectValidator(null);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_UninitializedObject_Fails()
        {
            ErrorCollection errors;
            SubRuleAction subRuleAction = new SubRuleAction();

            Assert.IsFalse(_validator.Validate(subRuleAction, out errors));
            Assert.AreEqual<int>(errors.Count, 2);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_UnknownActionId_Fails()
        {
            ErrorCollection errors;
            SubRuleAction subRuleAction = new SubRuleAction();
            subRuleAction.Id = 1;
            subRuleAction.ActionId = 100000;

            IActionCollection actionCollection = new ActionCollection();

            IScheduleService scheduleServiceStub = new StubIScheduleService
            {
                Inner = ServiceLocator.Resolve<IScheduleService>(),
                GetActions = () => actionCollection
            };
            ServiceLocator.RegisterInstance(scheduleServiceStub);

            Assert.IsFalse(_validator.Validate(subRuleAction, out errors));
            Assert.AreEqual<int>(errors.Count, 1);

        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_ActionParameterNotSpecified_Fails()
        {
            ErrorCollection errors;
            SubRuleAction subRuleAction = new SubRuleAction();
            subRuleAction.Id = 1;
            subRuleAction.ActionId = 2;

            Action action = new Action();
            action.Id = 2;
            action.HasParameter = true;

            IActionCollection actionCollection = new ActionCollection();
            actionCollection.Add(action);

            IScheduleService scheduleServiceStub = new StubIScheduleService
            {
                Inner = ServiceLocator.Resolve<IScheduleService>(),
                GetActions = () => actionCollection
            };
            ServiceLocator.RegisterInstance(scheduleServiceStub);

            Assert.IsFalse(_validator.Validate(subRuleAction, out errors));
            Assert.AreEqual<int>(errors.Count, 1);
        }
        
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_InitializedObject_Success()
        {
            ErrorCollection errors;
            SubRuleAction subRuleAction = new SubRuleAction();
            subRuleAction.Id = 1;
            subRuleAction.ActionId = 2;
            subRuleAction.Parameter.Constant = "1";

            Action action = new Action();
            action.Id = 2;
            action.HasParameter = true;
            action.ParameterTypeName = "System.Int32";

            IActionCollection actionCollection = new ActionCollection();
            actionCollection.Add(action);

            IScheduleService scheduleServiceStub = new StubIScheduleService
            {
                Inner = ServiceLocator.Resolve<IScheduleService>(),
                GetActions = () => actionCollection
            };
            ServiceLocator.RegisterInstance(scheduleServiceStub);

            Assert.IsTrue(_validator.Validate(subRuleAction, out errors));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_ParameterTypeNameNotSpecifiedForAction_Fails()
        {
            ErrorCollection errors;
            SubRuleAction subRuleAction = new SubRuleAction();
            subRuleAction.Id = 1;
            subRuleAction.ActionId = 2;
            subRuleAction.Parameter.Constant = "1";

            Action action = new Action();
            action.Id = 2;
            action.HasParameter = true;

            IActionCollection actionCollection = new ActionCollection();
            actionCollection.Add(action);

            IScheduleService scheduleServiceStub = new StubIScheduleService
            {
                Inner = ServiceLocator.Resolve<IScheduleService>(),
                GetActions = () => actionCollection
            };
            ServiceLocator.RegisterInstance(scheduleServiceStub);

            Assert.IsFalse(_validator.Validate(subRuleAction, out errors));
            Assert.AreEqual<int>(errors.Count, 1);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_ParameterOfWrongTypeIsSpecified_Fails()
        {
            ErrorCollection errors;
            SubRuleAction subRuleAction = new SubRuleAction();
            subRuleAction.Id = 1;
            subRuleAction.ActionId = 2;
            subRuleAction.Parameter.Constant = "sdf";

            Action action = new Action();
            action.Id = 2;
            action.ParameterTypeName = "System.Int32";
            action.HasParameter = true;

            IActionCollection actionCollection = new ActionCollection();
            actionCollection.Add(action);

            IScheduleService scheduleServiceStub = new StubIScheduleService
            {
                Inner = ServiceLocator.Resolve<IScheduleService>(),
                GetActions = () => actionCollection
            };
            ServiceLocator.RegisterInstance(scheduleServiceStub);

            Assert.IsFalse(_validator.Validate(subRuleAction, out errors));
            Assert.AreEqual<int>(errors.Count, 1);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_ParameterOfWrongTypeIsSpecified2_Fails()
        {
            ErrorCollection errors;
            SubRuleAction subRuleAction = new SubRuleAction();
            subRuleAction.Id = 1;
            subRuleAction.ActionId = 2;
            subRuleAction.Parameter.Constant = "10.32";

            Action action = new Action();
            action.Id = 2;
            action.ParameterTypeName = "System.Guid";
            action.HasParameter = true;

            IActionCollection actionCollection = new ActionCollection();
            actionCollection.Add(action);

            IScheduleService scheduleServiceStub = new StubIScheduleService
            {
                Inner = ServiceLocator.Resolve<IScheduleService>(),
                GetActions = () => actionCollection
            };
            ServiceLocator.RegisterInstance(scheduleServiceStub);

            Assert.IsFalse(_validator.Validate(subRuleAction, out errors));
            Assert.AreEqual<int>(errors.Count, 1);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_ParameterGuidType_Success()
        {
            ErrorCollection errors;
            SubRuleAction subRuleAction = new SubRuleAction();
            subRuleAction.Id = 1;
            subRuleAction.ActionId = 25;
            subRuleAction.Parameter.Constant = "5DE4D75D-CFE1-48b3-99A7-37ED798DB418";

            Action action = new Action();
            action.Id = 25;
            action.ParameterTypeName = "System.Guid";
            action.HasParameter = true;

            IActionCollection actionCollection = new ActionCollection();
            actionCollection.Add(action);

            IScheduleService scheduleServiceStub = new StubIScheduleService
            {
                Inner = ServiceLocator.Resolve<IScheduleService>(),
                GetActions = () => actionCollection
            };
            ServiceLocator.RegisterInstance(scheduleServiceStub);

            Assert.IsTrue(_validator.Validate(subRuleAction, out errors));
        }
    }
}