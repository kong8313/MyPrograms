using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CallGroups
{
    [TestClass]
    public class CallGroupConditionTriggerTests : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        [TestMethod, Owner(@"Firm\MaximL")]
        public void CreateCondition_PriorityOneAndClosedSurveyWithDefaultSchedulingMode_ConditionIsNotSurveySpecificed()
        {
            var surveyId = CreateSurvey(false, false);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                };

            var callGroupId = CreateCallGroup(conditions);

            var expected = new BvCallGroupConditionPerSurveyEntity[]{};

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void CreateCondition_PriorityOneAndOpenedSurveyWithDefaultSchedulingMode_ConditionIsNotSurveySpecificed()
        {
            var surveyId = CreateSurvey(true, false);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                };

            var callGroupId = CreateCallGroup(conditions);

            var expected = new BvCallGroupConditionPerSurveyEntity[] { };

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void CreateCondition_PriorityOneAndClosedSurveyWithCallGroupSchedulingMode_ConditionIsNotSurveySpecificed()
        {
            var surveyId = CreateSurvey(false, true);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                };

            var callGroupId = CreateCallGroup(conditions);

            var expected = new BvCallGroupConditionPerSurveyEntity[] { };

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void CreateCondition_PriorityZeroAndOpenedSurveyWithCallGroupSchedulingMode_ConditionIsNotSurveySpecificed()
        {
            var surveyId = CreateSurvey(true, true);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 0},
                };

            var callGroupId = CreateCallGroup(conditions);

            var expected = new BvCallGroupConditionPerSurveyEntity[] { };

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void CreateCondition_PriorityOneAndOpenedSurveyWithCallGroupSchedulingMode_ConditionIsSurveySpecificed()
        {
            var surveyId = CreateSurvey(true, true);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                };

            var callGroupId = CreateCallGroup(conditions);

            var expected = new []
                           {
                               new BvCallGroupConditionPerSurveyEntity{ConditionValue = 1, ConditionPriority = 1}
                           };

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void UpdateCondition_ConditionAreSurveySpecifiedAndNewPriority2_ConditionIsSurveySpecificed()
        {
            var surveyId = CreateSurvey(true, true);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                };

            var callGroupId = CreateCallGroup(conditions);

            conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 2},
                };

            UpdatePriority(callGroupId, conditions);

            var expected = new[]
                           {
                               new BvCallGroupConditionPerSurveyEntity{ConditionValue = 1, ConditionPriority = 2}
                           };

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void UpdateCondition_ConditionAreSurveySpecifiedAndNewPriority0_ConditionIsNotSurveySpecificed()
        {
            var surveyId = CreateSurvey(true, true);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                };

            var callGroupId = CreateCallGroup(conditions);

            conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 0},
                };

            UpdatePriority(callGroupId, conditions);

            var expected = new BvCallGroupConditionPerSurveyEntity[] {};

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void DeleteCondition_OpenedSurveyWithCallGroupMode_ConditionAreDeleted()
        {
            var surveyId = CreateSurvey(true, true);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                };

            var callGroupId = CreateCallGroup(conditions);

            

            DeleteCondition(callGroupId, conditions);

            var expected = new BvCallGroupConditionPerSurveyEntity[] { };

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void DeleteCondition_ClosedSurveyWithCallGroupMode_ConditionAreDeleted()
        {
            var surveyId = CreateSurvey(false, true);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                };

            var callGroupId = CreateCallGroup(conditions);

            DeleteCondition(callGroupId, conditions);

            var expected = new BvCallGroupConditionPerSurveyEntity[] { };

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void CreateConditions_OpenedSurveyWithCallGroupSchedulingMode_ConditionsAreCorrect()
        {
            var surveyId = CreateSurvey(true, true);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 1},
                    new BvCallGroupConditionEntity{ConditionValue = 3, ConditionPriority = 3},
                    new BvCallGroupConditionEntity{ConditionValue = 4, ConditionPriority = 3},
                    new BvCallGroupConditionEntity{ConditionValue = 7, ConditionPriority = 0},
                    new BvCallGroupConditionEntity{ConditionValue = 8, ConditionPriority = 0},
                };

            var callGroupId = CreateCallGroup(conditions);

            var expected = new[]
                           {
                                new BvCallGroupConditionPerSurveyEntity{ConditionValue = 1, ConditionPriority = 1},
                                new BvCallGroupConditionPerSurveyEntity{ConditionValue = 2, ConditionPriority = 1},
                                new BvCallGroupConditionPerSurveyEntity{ConditionValue = 3, ConditionPriority = 3},
                                new BvCallGroupConditionPerSurveyEntity{ConditionValue = 4, ConditionPriority = 3},
                           };

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void UpdateConditions_OpenedSurveyWithCallGroupSchedulingMode_ConditionsAreCorrect()
        {
            var surveyId = CreateSurvey(true, true);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 1},
                    new BvCallGroupConditionEntity{ConditionValue = 3, ConditionPriority = 3},
                    new BvCallGroupConditionEntity{ConditionValue = 4, ConditionPriority = 3},
                    new BvCallGroupConditionEntity{ConditionValue = 7, ConditionPriority = 0},
                    new BvCallGroupConditionEntity{ConditionValue = 8, ConditionPriority = 0},
                };

            var callGroupId = CreateCallGroup(conditions);

            conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 1},
                    new BvCallGroupConditionEntity{ConditionValue = 3, ConditionPriority = 0},
                    new BvCallGroupConditionEntity{ConditionValue = 4, ConditionPriority = 0},
                    new BvCallGroupConditionEntity{ConditionValue = 7, ConditionPriority = 5},
                    new BvCallGroupConditionEntity{ConditionValue = 8, ConditionPriority = 5},
                };

            UpdatePriority(callGroupId, conditions);

            var expected = new[]
                           {
                                new BvCallGroupConditionPerSurveyEntity{ConditionValue = 1, ConditionPriority = 1},
                                new BvCallGroupConditionPerSurveyEntity{ConditionValue = 2, ConditionPriority = 1},
                                new BvCallGroupConditionPerSurveyEntity{ConditionValue = 7, ConditionPriority = 5},
                                new BvCallGroupConditionPerSurveyEntity{ConditionValue = 8, ConditionPriority = 5},
                           };

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void DeleteConditions_OpenedSurveyWithCallGroupSchedulingMode_ConditionsAreCorrect()
        {
            var surveyId = CreateSurvey(true, true);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 1},
                    new BvCallGroupConditionEntity{ConditionValue = 3, ConditionPriority = 3},
                    new BvCallGroupConditionEntity{ConditionValue = 4, ConditionPriority = 3},
                    new BvCallGroupConditionEntity{ConditionValue = 7, ConditionPriority = 0},
                    new BvCallGroupConditionEntity{ConditionValue = 8, ConditionPriority = 0},
                };

            var callGroupId = CreateCallGroup(conditions);

            conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 1},
                    new BvCallGroupConditionEntity{ConditionValue = 8, ConditionPriority = 5},
                };

            DeleteCondition(callGroupId, conditions);

            var expected = new[]
                           {
                                new BvCallGroupConditionPerSurveyEntity{ConditionValue = 2, ConditionPriority = 1},
                                new BvCallGroupConditionPerSurveyEntity{ConditionValue = 3, ConditionPriority = 3},
                                new BvCallGroupConditionPerSurveyEntity{ConditionValue = 4, ConditionPriority = 3},
                           };

            CheckCallGroupPerSurvey(surveyId, callGroupId, expected);
        }

        private void CheckCallGroupPerSurvey(int surveyId, int callGroupId, BvCallGroupConditionPerSurveyEntity[] expected)
        {
            var actual = BvCallGroupConditionPerSurveyAdapter.GetAll()
                .Where(x => x.SurveyId == surveyId && x.CallGroupId == callGroupId)
                .OrderBy(c => c.ConditionValue);
            var sw = Stopwatch.StartNew();
            TestAssert.AreEqual(expected, actual, x => new { x.ConditionValue, x.ConditionPriority });
            Trace.TraceInformation("Stopwatch:{0}", sw.ElapsedMilliseconds);
        }

        private void DeleteCondition(int callGroupId, BvCallGroupConditionEntity[] conditions)
        {
            var service = ServiceLocator.Resolve<ICallGroupService>();
            foreach (var condition in conditions)
            {
                service.DeleteCondition(callGroupId, condition.ConditionValue);
            }
        }

        private void UpdatePriority(int callGroupId, BvCallGroupConditionEntity[] conditions)
        {
            var service = ServiceLocator.Resolve<ICallGroupService>();
            foreach (var condition in conditions)
            {
                service.UpdateConditionPriority(callGroupId, new []{condition.ConditionValue}, condition.ConditionPriority);
            }
        }

        private int CreateCallGroup(BvCallGroupConditionEntity[] conditions)
        {
            var group = new BvCallGroupEntity { Name = "Group" };
            
            var service = ServiceLocator.Resolve<ICallGroupService>();
            var repository = ServiceLocator.Resolve<ICallGroupRepository>(); 
            
            repository.Insert(group);
            service.SetListOfCondition(group.Id, conditions);
            return group.Id;
        }

        private int CreateSurvey(bool isOpen, bool isCallGroup)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p000000010");

            if (isCallGroup)
            {
                var survey = SurveyRepository.GetById(surveyId);
                survey.SurveySchedulingMode = (short)SurveySchedulingMode.CallGroup;
                SurveyRepository.Update(survey);
            }

            if (isOpen)
            {
                _surveyStateService.Open(surveyId);
            }

            return surveyId;
        }
    }
}
