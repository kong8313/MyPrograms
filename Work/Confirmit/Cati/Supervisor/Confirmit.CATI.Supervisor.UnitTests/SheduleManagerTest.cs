using System;
using System.Drawing;
using System.Reflection;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.ScheduleDom.Script;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Confirmit.CATI.Supervisor.Script.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Classes.Script;
using Confirmit.CATI.Supervisor.Script.Classes.Fakes;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class ScheduleManagerTest
    {
        private IServiceRegistrator _serviceRegistrator;

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _serviceRegistrator = UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
            _serviceRegistrator.Register<ISchedulingObjectValidator, SchedulingObjectValidator>();
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void GetShiftTypes_CorrectData_Success()
        {
            int count;
            ShiftTypeCollection shiftTypeCollection = new ShiftTypeCollection();
            ShiftType shiftType = new ShiftType();
            shiftType.Id = 1;
            shiftType.Name = "TestName";
            shiftType.Color = Color.Black;
            shiftTypeCollection.Add(shiftType);

            ShiftTypeInfo[] shiftTypes = ScheduleManager.GetShiftTypes(shiftTypeCollection, out count);

            Assert.AreEqual(count, 1);
            Assert.AreEqual(shiftTypes[0].Id, 1);
            Assert.AreEqual(shiftTypes[0].Name, "TestName");
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void GetShiftsByTimezone_CorrectData_Success()
        {
            int totalCount;
            TimeSpan ts = new TimeSpan(1);
            Shift shift = new Shift();
            ShiftCollection shiftCollection = new ShiftCollection();
            ShiftData shiftData = new ShiftData(DayOfWeek.Monday, ts, DayOfWeek.Monday, ts);

            shift.Id = 1;
            shift.ShiftTypeId = 1;
            shift.SetDataForTimezone(Shift.RespondentTimezoneId, shiftData);
            shiftCollection.Add(shift);

            ShiftInfo[] shifts = ScheduleManager.GetShiftsByTimezone(Shift.RespondentTimezoneId, shiftCollection, out totalCount);

            Assert.AreEqual<int>(shifts.Length, 1);
            Assert.AreEqual<int>(shifts[0].Id.Value, 1);
            Assert.AreEqual<DayOfWeek>(shifts[0].StartDay, DayOfWeek.Monday);
            Assert.AreEqual<DayOfWeek>(shifts[0].EndDay, DayOfWeek.Monday);
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void GetExclusionByTimezone_CorrectData_Success()
        {
            int total;
            int timeZone = 1;
            DateTime dtDefault = DateTime.Now;
            DateTime dtOverridden = DateTime.Now.AddTicks(1);
            Exclusion exclusion = new Exclusion();
            ExclusionCollection exclusionCollection = new ExclusionCollection();
            ExclusionData exclusionData = new ExclusionData(dtDefault, dtDefault);

            exclusion.Id = 1;
            exclusion.ShiftTypeId = 0;
            exclusion.SetDataForTimezone(Exclusion.RespondentTimezoneId, exclusionData);
            exclusionCollection.Add(exclusion);

            exclusionData = new ExclusionData(dtOverridden, dtOverridden);
            exclusion.SetDataForTimezone(timeZone, exclusionData);

            ExclusionInfo[] exclusions = ScheduleManager.GetExclusionsByTimezone(Exclusion.RespondentTimezoneId, exclusionCollection, out total);

            Assert.AreEqual(exclusions.Length, 1);
            Assert.AreEqual(exclusions[0].Id, 1);
            Assert.AreEqual(exclusions[0].StartDay, dtDefault.Date);
            Assert.AreEqual(exclusions[0].EndDay, dtDefault.Date);

            exclusions = ScheduleManager.GetExclusionsByTimezone(timeZone, exclusionCollection, out total);

            Assert.AreEqual(exclusions.Length, 1);
            Assert.AreEqual(exclusions[0].Id, 1);
            Assert.AreEqual(exclusions[0].StartDay, dtOverridden.Date);
            Assert.AreEqual(exclusions[0].EndDay, dtOverridden.Date);
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void GetRules_CorrectData_ReturnRules()
        {
            int count;
            Rule rule;
            SubRule subRule;
            SubRuleAction subRuleAction;
            RuleCollection ruleCollection = new RuleCollection();

            rule = new Rule();
            rule.Id = ruleCollection.GetNewId();
            rule.Description = "rule";

            subRule = new SubRule();
            subRule.Id = rule.SubRules.GetNewId();
            subRule.ItsId = 1;
            subRule.ShiftTypeId = 1;
            subRule.Filter = "filter";
            subRule.Description = "subrule";

            subRuleAction = new SubRuleAction();
            subRuleAction.Id = 1;
            subRuleAction.ActionId = 1;
            subRuleAction.Enabled = true;
            subRuleAction.FilterEnabled = true;
            subRuleAction.Filter = "filter";
            subRuleAction.Parameter.Value = "parameter";

            var action = new CATI.Core.ScheduleDom.Script.Action { Id = 1 };

            var stub = new StubIScheduleService { GetActions = () => new ActionCollection() { action } };
            _serviceRegistrator.RegisterInstance<IScheduleService>(stub);

            subRule.SubRuleActions.Add(subRuleAction);
            rule.SubRules.Add(subRule);
            ruleCollection.Add(rule);

            RuleInfo[] rules = ScheduleManager.GetRules(ruleCollection, out count);

            Assert.AreEqual(count, 1);

            RuleInfo ri = rules[0];
            Assert.AreEqual(rule.Id, ri.Id);
            Assert.AreEqual("rule", ri.Description);

            SubRuleInfo sri = rules[0].SubRules[0];
            Assert.AreEqual(subRule.Id, sri.Id);
            Assert.AreEqual(1, sri.ItsId);
            Assert.AreEqual(1, sri.ShiftTypeId);
            Assert.AreEqual("filter", sri.Filter);
            Assert.AreEqual("subrule", sri.Description);

            ActionInfo ai = rules[0].SubRules[0].Actions[0];
            Assert.AreEqual(1, ai.Id);
            Assert.AreEqual(1, ai.ActionId);
            Assert.AreEqual("filter", ai.Filter);
            Assert.AreEqual(true, ai.Enabled);
            Assert.AreEqual(true, ai.FilterEnabled);
            Assert.AreEqual("parameter", ai.ParameterValue);

        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void ScheduleById_ValidParameter_ReturnSchedule()
        {
            BvScheduleEntity bvSchedule = new BvScheduleEntity();
            bvSchedule.XmlUnderDev = "12345";
            var scheduleRepository = new StubIScheduleRepository();
            scheduleRepository.GetByIdInt32 = id => bvSchedule;
            var scheduleManager = new StubIScheduleManager();
            scheduleManager.DeserializeScheduleString = schedule => new Schedule();

            _serviceRegistrator.RegisterInstance<IScheduleManager>(scheduleManager);
            _serviceRegistrator.RegisterInstance<IScheduleRepository>(scheduleRepository);

            ScheduleManager.ScheduleById(1);
        }

        private static RowReadAttribute GetRowReadAttribute(PropertyInfo prop)
        {
            object[] attrs = prop.GetCustomAttributes(typeof(RowReadAttribute), true);
            return attrs.Length > 0 ? (RowReadAttribute)attrs[0] : null;
        }

    }
}
