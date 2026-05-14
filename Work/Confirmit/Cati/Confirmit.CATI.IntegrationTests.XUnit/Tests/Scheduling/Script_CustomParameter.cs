using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using System.Collections.Generic;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ScriptCustomParameter : BaseMockedIntegrationTest
    {
        private readonly IScheduleService _scheduleService;

        public ScriptCustomParameter()
        {
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
        }
        
        /* Check: "actions with params set through custom parameters should work correctly"
         * Create person
         * Create survey with following scheduling script
         *    AssignResource to person through custom parameter
         * Create Interview with call
         * Schedule interview
         * Scheduling correct( assign resource is correct )
         */
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Interview_AssignPersonThroughConstantParam_PersonAssigned(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int personSid = PersonTools.CreatePerson(null);

            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, personSid.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");

            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);

            call.Resource = personSid;
            BackendTools.CheckCall(call);
        }

        /* Check: "Redefine survey specific custom parameter should be applyed"
         * create person and person group
         * Create survey with following scheduling script
         *  Actions:
         *    AssignResource with person through custom parameter
         *  CustomParams:
         *     resource
         * Create Interview with call
         * Schedule interview
         * Scheduling correct( assign resource is person )
         * Change value of AssignResource param for survey to person group
         * Schedule same interview
         * Scheduling correct( assign resource is person group )
         */
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithParamScript_ChangeSurveyParam_SchedulingActionWorksCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int personSid = PersonTools.CreatePerson(null);
            int personGroupSid = PersonTools.CreatePersonGroup(null);

            var resourceParam = new CustomParameter
            {
                Id = 1,
                Name = "resource",
                Description = "assign resource to X",
                Type = SchedulingParameterType.Resource,
                Value = personSid
            };


            var script = new TestScript(
                    new Action( Action.Operation.AssignResource, resourceParam ),
                    @"Scheduling2007\Schedule.xml");

            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);

            call.Resource = personSid;
            BackendTools.CheckCall(call);

            //
            // redefine custom parameter
            //

            _scheduleService.SetParamValue(surveySid, resourceParam.Id, personGroupSid);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);

            call.Resource = personGroupSid;
            BackendTools.CheckCall(call);

        }

        /* Check: "exchange parametrize scheduling script for survey should work correctly"
         * create person
         * Create survey with following scheduling script
         *  Actions:
         *    AssignResource with person through custom parameter
         *  CustomParams:
         *     resource
         * Create Interview with call
         * Schedule interview
         * Scheduling correct( assign resource is person )
         * create following scheduling script and assign it to survey
         *  Actions:
         *    SetNewITS with 31 through custom parameter
         *  CustomParams:
         *    ITS
         * Schedule same interview
         * Scheduling correct( ITS is 31 )
         */
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithParamScript_ChangeScript_SchedulingActionWorksCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int personSid = PersonTools.CreatePerson(null);

            var resourceParam = new CustomParameter
            {
                Id = 1,
                Name = "resource",
                Description = "assign resource to X",
                Type = SchedulingParameterType.Resource,
                Value = personSid
            };

            const int its = 31;
            var itsParam = new CustomParameter
            {
                Id = 1,
                Name = "ITS",
                Description = "transient state",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = its
            };


            var scriptOne = new TestScript(
                    new Action(Action.Operation.AssignResource, resourceParam),
                    @"Scheduling2007\Schedule.xml");

            var scriptTwo = new TestScript(
                    new Action(Action.Operation.SetNewITS, itsParam),
                    @"Scheduling2007\Schedule.xml");
            
            //update all hours script
            TestScript.Update(_scheduleService.DefaultScheduleId, scriptOne);
            //create second script
            scriptTwo.Create(null);
            //create survey with all hours script by default
            int surveySid = BackendToolsObject.CreateSurvey("p01010101");

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);

            call.Resource = personSid;
            BackendTools.CheckCall(call);

            //
            // redefine custom parameter
            //

            //change survey script
            var survey = SurveyRepository.GetById(surveySid);
            survey.ScheduleID = scriptTwo.ScheduleID;
            SurveyRepository.Update(survey);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            interview.TransientState = its;
            BackendTools.CheckInterview(interview);

            Assert.IsFalse( BackendTools.IsCallExists(interview.SurveySID, interview.ID), "call should be deleted");

        }

        /* "Check: Custom variables is available and work correct in Custom script
         * Create survey with following scheduling script
         *    Actions:
         *      RunCustomScript "ChangeITS"
         *    CustomScript:
         *          function ChangeITS()
                    {
                        Scheduling.Interview.TransientState = Convert.ToInt32( GetParamValue(""ITS""));
                    }  
         *    CustomParams:
         *      ITS = 31
         * Create Interview with call
         * Schedule interview
         * Scheduling correct( ITS is 31 )
         * Redefine Schedule param "ITS" = 32
         * Schedule same interview
         * Scheduling correct( ITS is 32 )
         */
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithParamScript_ChangeSurveyParam_SchedulingCustomScriptWorksCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            const int defaultIts = 31;
            const int surveyIts = 32;

            var itsParam = new CustomParameter
            {
                Id = 1,
                Name = "ITS",
                Description = "transient state",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = defaultIts
            };

            var script = new TestScript(
                    new Action(Action.Operation.RunCustomScript, "ChangeITS"),
                    new Shift(1, 1, "0.00:00:00", "0.00:00:00"));
            script.CustomParameters.Add(itsParam);
            script.CustomScript = @"
function ChangeITS()
{
   Scheduling.Interview.TransientState = Convert.ToInt32( GetParamValue(""ITS""));
}";

            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            interview.TransientState = defaultIts;
            BackendTools.CheckInterview(interview);

            Assert.IsFalse(BackendTools.IsCallExists(call.SurveySID, call.InterviewID));
            //
            // redefine custom parameter
            //

            _scheduleService.SetParamValue(surveySid, itsParam.Id, surveyIts);
            
            BackendTools.CreateCall(call);
            
            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            interview.TransientState = surveyIts;
            BackendTools.CheckInterview(interview);

            Assert.IsFalse(BackendTools.IsCallExists(call.SurveySID, call.InterviewID));
        }

        [Theory, Owner(@"firm\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithParamScript_ChangeSurveyParamUsingGetParamNumericMethod_SchedulingCustomScriptWorksCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            const int defaultIts = 31;

            var context = new TestData {
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] { new InterviewData { Tag="S1.I1" } }
                    }
                },
                Scripts = new[]  {
                    new ScriptData() { Tag ="SS1",
                        Script = new TestScript(new Action(Action.Operation.RunCustomScript, "ChangeITS"), Shift.Week) {
                            CustomParameters = new List<CustomParameter>() {
                                new CustomParameter {
                                    Id = 1,
                                    Name = "ITS",
                                    Description = "transient state",
                                    Type = SchedulingParameterType.ExtendedStatus,
                                    Value = defaultIts
                                }
                            },
                            CustomScript = @"
function ChangeITS()
{
   Scheduling.Interview.TransientState = GetParamNumeric(""ITS"");
}"
                        },                        
                    }
                }
            }.Create();

            var surveySid = context.GetSurvey("S1").Id;
            var interview = context.GetInterview("S1.I1");

            interview.Assert.IsTrue(x => x.TransientState != defaultIts);

            BackendTools.FireEvent(interview.Model);

            interview.Assert.AreEqual(defaultIts, x => x.TransientState);
        }

        /* "Check: survey specific params should not be reseted after update survey without changing of scheduling script"
         * create person and person group
         * Create survey with following scheduling script
         *  Actions:
         *    AssignResource with person through custom parameter
         *  CustomParams:
         *     resource
         * Create Interview with call
         * Schedule interview
         * Scheduling correct( assign resource is person )
         * Change value of AssignResource param for survey to person group
         * Update survey( without change assign of scheduling script )
         * Schedule same interview
         * Scheduling correct( assign resource is person group )
         */
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithParamScript_RedefineParamAndUpdateSurvey_RedefinedParameterIsNotReseted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int personSid = PersonTools.CreatePerson(null);
            int personGroupSid = PersonTools.CreatePersonGroup(null);

            var resourceParam = new CustomParameter
            {
                Id = 1,
                Name = "resource",
                Description = "assign resource to X",
                Type = SchedulingParameterType.Resource,
                Value = personSid
            };


            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, resourceParam),
                    @"Scheduling2007\Schedule.xml");

            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);

            call.Resource = personSid;
            BackendTools.CheckCall(call);

            //
            // redefine custom parameter
            //

            _scheduleService.SetParamValue(surveySid, resourceParam.Id, personGroupSid);
            //Update survey
            var survey = SurveyRepository.GetById(surveySid);
            survey.Description = "new description";
            SurveyRepository.Update(survey);
            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);

            call.Resource = personGroupSid;
            BackendTools.CheckCall(call);

        }

        /* "Check: survey specific params should not be reseted after relaunch script"
         * create person and person group
         * Create survey with following scheduling script
         *  Actions:
         *    AssignResource with person through custom parameter
         *  CustomParams:
         *     resource
         * Create Interview with call
         * Schedule interview
         * Scheduling correct( assign resource is person )
         * Change value of AssignResource param for survey to person group
         * Relaunch script
         * Schedule same interview
         * Scheduling correct( assign resource is person group )
         */
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithParamScript_RedefineParamAndRelaunchScript_RedefinedParameterIsNotReseted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int personSid = PersonTools.CreatePerson(null);
            int personGroupSid = PersonTools.CreatePersonGroup(null);

            var resourceParam = new CustomParameter
            {
                Id = 1,
                Name = "resource",
                Description = "assign resource to X",
                Type = SchedulingParameterType.Resource,
                Value = personSid
            };


            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, resourceParam),
                    @"Scheduling2007\Schedule.xml");

            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);

            call.Resource = personSid;
            BackendTools.CheckCall(call);

            //
            // redefine custom parameter
            //

            _scheduleService.SetParamValue(surveySid, resourceParam.Id, personGroupSid);
            //relaunch scheduling script
            _scheduleService.Launch(script.ScheduleID);
            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);

            call.Resource = personGroupSid;
            BackendTools.CheckCall(call);

        }
        /* Check: "Launching of scheduling script with invalid custom parameter should failed"
         */
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithInvalidParamScript_LaunchScript_ExceptionThown(SecurityMode mode)
        {
            SetSecurityMode(mode);

            const int personSid = -10;

            var resourceParam = new CustomParameter
            {
                Id = 1,
                Name = "resource",
                Description = "assign resource to X",
                Type = SchedulingParameterType.Resource,
                Value = personSid
            };


            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, resourceParam),
                    @"Scheduling2007\Schedule.xml");

            TestAssert.InvokeMethodAndVerifyExceptionThrown<UserMessageException>(
                    () => BackendToolsObject.CreateSurvey(script));
        }

        /* Check: "Redefining of custom parameter with invalid value should failed"
         */
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithParamScript_SetParameterWithInvalidValue_ExceptionThown(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int personSid = PersonTools.CreatePerson(null);

            var resourceParam = new CustomParameter
            {
                Id = 1,
                Name = "resource",
                Description = "assign resource to X",
                Type = SchedulingParameterType.Resource,
                Value = personSid
            };


            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, resourceParam),
                    @"Scheduling2007\Schedule.xml");

            int surveySid = BackendToolsObject.CreateSurvey(script);

            TestAssert.InvokeMethodAndVerifyExceptionThrown<UserMessageException>(
                () => _scheduleService.SetParamValue(surveySid, resourceParam.Id, -10 /*Wrong assign resource*/));
        }



        /* "Check: custom params should work in action filters"
         * Create survey with following scheduling script
         *  Actions:
         *    SenNewITS with NewITS custom parameter and with filter:
         *         Scheduling.Interview.TransientState == GetParamValue(\"FilterITS\")
         *  CustomParams:
         *     FilterITS = 20
         *     NewITS = 30
         * Create first interview with ITS = 10
         * Create second interview with ITS = 20
         * Schedule two interviews
         * Scheduling correct
         *     first interview with ITS = 10
         *     second interview with ITS = 30
         */
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithParamScript_FilterInterviewsWithUsingCustomParameter_InterviewsFiltered(SecurityMode mode)
        {
            SetSecurityMode(mode);

            const int initIts1 = 10;
            const int initIts2 = 20;
            const int newIts2 = 30;

            var filterItsParam = new CustomParameter
            {
                Id = 1,
                Name = "FilterITS",
                Description = "Filter ITS",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = initIts2
            };

            var newItsParam = new CustomParameter
            {
                Id = 2,
                Name = "NewITS",
                Description = "new ITS",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = newIts2
            };


            var script = new TestScript(
                    new Action(Action.Operation.SetNewITS, newItsParam, "Scheduling.Interview.TransientState == GetParamValue(\"FilterITS\")"),
                    @"Scheduling2007\Schedule.xml");
            script.CustomParameters.Add(filterItsParam);
            script.CustomParameters.Add(newItsParam);

            var surveySid = BackendToolsObject.CreateSurvey(script);

            var interview1 = BackendTools.NewInterview(surveySid);
            var interview2 = BackendTools.NewInterview(surveySid);

            interview1.TransientState = initIts1;
            interview2.TransientState = initIts2;
            
            BackendTools.CreateInterview(interview1);
            BackendTools.CreateInterview(interview2);

            BackendTools.FireEvent(interview1);
            BackendTools.FireEvent(interview2);

            interview2.TransientState = newIts2;

            BackendTools.CheckInterview(interview1);
            BackendTools.CheckInterview(interview2);
        }

        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public void FirstLaunchScript_ComplexCustomParameterConfigurationAllParametersAreCreatedCorrectly(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var param1 = new CustomParameter
            {
                Id = 1,
                Name = "Param1",
                Description = "Description of Param1",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 11
            };

            var param2 = new CustomParameter
            {
                Id = 2,
                Name = "Param2",
                Description = "Description of Param2",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 12
            };

            var param3 = new CustomParameter
            {
                Id = 3,
                Name = "Param3",
                Description = "Description of Param3",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 13
            };

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() {Tag = "S1", SchedulingScript = "SS1"},
                    new SurveyData() {Tag = "S2", SchedulingScript = "SS1"},
                    new SurveyData() {Tag = "S3", SchedulingScript = "SS2"},
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag = "SS1",
                        Script = new TestScript(new Action(Action.Operation.IncrementPriority, "1"), new Shift(1,1,"0.00:00:00", "1.00:00:00"))
                        {
                            CustomParameters = new[] {param1, param2, param3}.ToList()
                        }
                    },
                    new ScriptData()
                    {
                        Tag = "SS2",
                        Script = new TestScript(new Action(Action.Operation.IncrementPriority, "1"), new Shift(1,1,"0.00:00:00", "1.00:00:00"))
                        {
                            CustomParameters = new[] {param1, param2, param3}.ToList()
                        }
                    }
                }
            }.Create();

            var actual = BackendTools.Format(BvScheduleParamAdapter.GetAll());
            string expected = BackendTools.Format(context,@"
 ScheduleID SurveySID ParamID   Name           Description Type Value
      {SS1}         0       1 Param1 Description of Param1    4    11
      {SS1}         0       2 Param2 Description of Param2    4    12
      {SS1}         0       3 Param3 Description of Param3    4    13
      {SS1}      {S1}       1 Param1 Description of Param1    4    11
      {SS1}      {S1}       2 Param2 Description of Param2    4    12
      {SS1}      {S1}       3 Param3 Description of Param3    4    13
      {SS1}      {S2}       1 Param1 Description of Param1    4    11
      {SS1}      {S2}       2 Param2 Description of Param2    4    12
      {SS1}      {S2}       3 Param3 Description of Param3    4    13
      {SS2}         0       1 Param1 Description of Param1    4    11
      {SS2}         0       2 Param2 Description of Param2    4    12
      {SS2}         0       3 Param3 Description of Param3    4    13
      {SS2}      {S3}       1 Param1 Description of Param1    4    11
      {SS2}      {S3}       2 Param2 Description of Param2    4    12
      {SS2}      {S3}       3 Param3 Description of Param3    4    13");
            
            Assert.AreEqual(expected, actual);
        }

        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public void RelaunchScript_NoChanges_ParamsAreCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var param1 = new CustomParameter
            {
                Id = 1,
                Name = "Param1",
                Description = "Description of Param1",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 11
            };

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() {Tag = "S1", SchedulingScript = "SS1"}
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag = "SS1",
                        Script = new TestScript(new Action(Action.Operation.IncrementPriority, "1"), new Shift(1,1,"0.00:00:00", "1.00:00:00"))
                        {
                            CustomParameters = new[] {param1}.ToList()
                        }
                    }
                }
            }.Create();

            var script = context.GetScript("SS1");

            TestScript.Update(script.Id, script.Data.Script);

            var actual = BackendTools.Format(BvScheduleParamAdapter.GetAll());
            string expected = BackendTools.Format(context, @"
 ScheduleID SurveySID ParamID   Name           Description Type Value
      {SS1}         0       1 Param1 Description of Param1    4    11
      {SS1}      {S1}       1 Param1 Description of Param1    4    11");

            Assert.AreEqual(expected, actual);
        }

        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public void RelaunchScript_ChangeWithoutType_ParamValueIsnotUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var param1 = new CustomParameter
            {
                Id = 1,
                Name = "Param1",
                Description = "Description of Param1",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 11
            };

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() {Tag = "S1", SchedulingScript = "SS1"}
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag = "SS1",
                        Script = new TestScript(new Action(Action.Operation.IncrementPriority, "1"), new Shift(1,1,"0.00:00:00", "1.00:00:00"))
                        {
                            CustomParameters = new[] {param1}.ToList()
                        }
                    }
                }
            }.Create();

            var script = context.GetScript("SS1");
            param1.Value = 21;
            param1.Id = 2;
            param1.Description = "New description of Param1";

            TestScript.Update(script.Id, script.Data.Script);

            var actual = BackendTools.Format(BvScheduleParamAdapter.GetAll());
            string expected = BackendTools.Format(context,@"
 ScheduleID SurveySID ParamID   Name               Description Type Value
      {SS1}         0       2 Param1 New description of Param1    4    21
      {SS1}      {S1}       2 Param1 New description of Param1    4    11");

            Assert.AreEqual(expected, actual);
        }

        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public void RelaunchScript_ChangeWithType_ParamValueIsUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var param1 = new CustomParameter
            {
                Id = 1,
                Name = "Param1",
                Description = "Description of Param1",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 11
            };

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() {Tag = "S1", SchedulingScript = "SS1"}
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag = "SS1",
                        Script = new TestScript(new Action(Action.Operation.IncrementPriority, "1"), new Shift(1,1,"0.00:00:00", "1.00:00:00"))
                        {
                            CustomParameters = new[] {param1}.ToList()
                        }
                    }
                }
            }.Create();

            var script = context.GetScript("SS1");
            param1.Value = 21;
            param1.Type = SchedulingParameterType.Integer;
            param1.Id = 2;
            param1.Description = "New description of Param1";

            TestScript.Update(script.Id, script.Data.Script);

            var actual = BackendTools.Format(BvScheduleParamAdapter.GetAll());
            string expected = BackendTools.Format(context, @"
 ScheduleID SurveySID ParamID   Name               Description Type Value
      {SS1}         0       2 Param1 New description of Param1    0    21
      {SS1}      {S1}       2 Param1 New description of Param1    0    21");

            Assert.AreEqual(expected, actual);
        }

        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public void RelaunchScript_AddNewParam_ParamIsAdded(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var param1 = new CustomParameter
            {
                Id = 1,
                Name = "Param1",
                Description = "Description of Param1",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 11
            };

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() {Tag = "S1", SchedulingScript = "SS1"}
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag = "SS1",
                        Script = new TestScript(new Action(Action.Operation.IncrementPriority, "1"), new Shift(1,1,"0.00:00:00", "1.00:00:00"))
                        {
                            CustomParameters = new[] {param1}.ToList()
                        }
                    }
                }
            }.Create();

            var script = context.GetScript("SS1");
            script.Data.Script.CustomParameters.Add( new CustomParameter
            {
                Id = 2,
                Name = "Param2",
                Description = "Description of Param2",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 22
            });

            TestScript.Update(script.Id, script.Data.Script);

            var actual = BackendTools.Format(BvScheduleParamAdapter.GetAll());
            string expected = BackendTools.Format(context, @"
 ScheduleID SurveySID ParamID   Name           Description Type Value
      {SS1}         0       1 Param1 Description of Param1    4    11
      {SS1}         0       2 Param2 Description of Param2    4    22
      {SS1}      {S1}       1 Param1 Description of Param1    4    11
      {SS1}      {S1}       2 Param2 Description of Param2    4    22");

            Assert.AreEqual(expected, actual);
        }

        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public void RelaunchScript_DeleteAction_ParamIsDeleted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var param1 = new CustomParameter
            {
                Id = 1,
                Name = "Param1",
                Description = "Description of Param1",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 11
            };

            var param2 = new CustomParameter
            {
                Id = 2,
                Name = "Param2",
                Description = "Description of Param2",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 12
            };
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() {Tag = "S1", SchedulingScript = "SS1"}
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag = "SS1",
                        Script = new TestScript(new Action(Action.Operation.IncrementPriority, "1"), new Shift(1,1,"0.00:00:00", "1.00:00:00"))
                        {
                            CustomParameters = new[] {param1, param2}.ToList()
                        }
                    }
                }
            }.Create();

            var script = context.GetScript("SS1");

            script.Data.Script.CustomParameters.Remove(param2);

            TestScript.Update(script.Id, script.Data.Script);

            var actual = BackendTools.Format(BvScheduleParamAdapter.GetAll());
            string expected = BackendTools.Format(context, @"
 ScheduleID SurveySID ParamID   Name           Description Type Value
      {SS1}         0       1 Param1 Description of Param1    4    11
      {SS1}      {S1}       1 Param1 Description of Param1    4    11");

            Assert.AreEqual(expected, actual);
        }

        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public void RelaunchScript_ChangeCustomParameterConfiguration_OneIsDeletedOneIsUpdatedOneIsnotChangedOneIsInserted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var param1 = new CustomParameter
            {
                Id = 1,
                Name = "Param1",
                Description = "Description of Param1",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 11
            };

            var param2 = new CustomParameter
            {
                Id = 2,
                Name = "Param2",
                Description = "Description of Param2",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 12
            };

            var param3 = new CustomParameter
            {
                Id = 3,
                Name = "Param3",
                Description = "Description of Param3",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 13
            };

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() {Tag = "S1", SchedulingScript = "SS1"},
                    new SurveyData() {Tag = "S2", SchedulingScript = "SS1"},
                    new SurveyData() {Tag = "S3", SchedulingScript = "SS2"},
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag = "SS1",
                        Script = new TestScript(new Action(Action.Operation.IncrementPriority, "1"), new Shift(1,1,"0.00:00:00", "1.00:00:00"))
                        {
                            CustomParameters = new[] {param1, param2, param3}.ToList()
                        }
                    },
                    new ScriptData()
                    {
                        Tag = "SS2",
                        Script = new TestScript(new Action(Action.Operation.IncrementPriority, "1"), new Shift(1,1,"0.00:00:00", "1.00:00:00"))
                        {
                            CustomParameters = new[] {param1, param2, param3}.ToList()
                        }
                    }
                }
            }.Create();

            var script = context.GetScript("SS1");

            script.Data.Script.CustomParameters.Remove(param1);

            param2.Type = SchedulingParameterType.Integer;
            param2.Id = 4;
            param2.Description = "New description of Param2";
            param2.Value = 24;

            param3.Description = "New description of Param3";
            param3.Id = 5;
            param3.Value = 25;

            script.Data.Script.CustomParameters.Add(new CustomParameter
            {
                Id = 6,
                Name = "Param6",
                Description = "Description of Param6",
                Type = SchedulingParameterType.ExtendedStatus,
                Value = 26
            });
            TestScript.Update(script.Id, script.Data.Script);

            var actual = BackendTools.Format(BvScheduleParamAdapter.GetAll());
            string expected = BackendTools.Format(context, @"
 ScheduleID SurveySID ParamID   Name               Description Type Value
      {SS1}         0       4 Param2 New description of Param2    0    24
      {SS1}         0       5 Param3 New description of Param3    4    25
      {SS1}         0       6 Param6     Description of Param6    4    26
      {SS1}      {S1}       4 Param2 New description of Param2    0    24
      {SS1}      {S1}       5 Param3 New description of Param3    4    13
      {SS1}      {S1}       6 Param6     Description of Param6    4    26
      {SS1}      {S2}       4 Param2 New description of Param2    0    24
      {SS1}      {S2}       5 Param3 New description of Param3    4    13
      {SS1}      {S2}       6 Param6     Description of Param6    4    26
      {SS2}         0       1 Param1     Description of Param1    4    11
      {SS2}         0       2 Param2     Description of Param2    4    12
      {SS2}         0       3 Param3     Description of Param3    4    13
      {SS2}      {S3}       1 Param1     Description of Param1    4    11
      {SS2}      {S3}       2 Param2     Description of Param2    4    12
      {SS2}      {S3}       3 Param3     Description of Param3    4    13");

            Assert.AreEqual(expected, actual);
        }
    }
}
