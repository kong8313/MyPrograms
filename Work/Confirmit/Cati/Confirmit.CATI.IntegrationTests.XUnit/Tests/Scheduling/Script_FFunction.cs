using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes;
using Confirmit.CATI.Core.SurveyDataService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ScriptFFunctionTest : BaseMockedIntegrationTest
    {
        private string ProjectId { get; set; } = BackendTools.GenerateSurveyName();
        
        private const int InitIts = (int)CallOutcome.FreshSample;
        private const int NewIts = (int)CallOutcome.Completed;

        private static void MockGetFormInfo(IEnumerable<FormBase> forms)
        {
            var authoringStub = new StubIAuthoringService
            {
                GetFormInfosStringIEnumerableOfStringSchemaSourceType = (id, names, type) =>
                {
                    var formNames = names;
                    return forms.Where(x => formNames.Contains(x.Name)).ToArray();
                }
            };

            ServiceLocator.RegisterInstance<IAuthoringService>(authoringStub);
        }

        private static StubISurveyDataService MockFormValues(Dictionary<string, string> formsValues)
        {
            var stubISurveyDataService = new StubISurveyDataService
            {
                GetDataTransferDefBaseResponseToken = (transfer, token) =>
                {
                    var result = new TransferResult { ResponseToken = token, Result = new DataSet() };

                    var formName = ((TransferDef)transfer).Levels[0].Forms[0].Name;
                    var table = new DataTable("responseid");

                    table.Columns.Add(formName, typeof(string));
                    result.Result.Tables.Add(table);

                    if (formsValues.TryGetValue(formName, out var value))
                    {
                        table.Rows.Add(value);
                    }

                    return result;
                },
                UpdateDataTransferDefDataSetBooleanBooleanInt32 =
                    (transferDef, dataSet, applyRules, inTransaction, transactionKey) =>
                    {
                        foreach (DataColumn column in dataSet.Tables[0].Columns)
                        {
                            if (formsValues.ContainsKey(column.ColumnName))
                            {
                                formsValues[column.ColumnName] = (string)dataSet.Tables[0].Rows[0].ItemArray[column.Ordinal];
                            }
                        }

                        return new ErrorMessage[] { };
                    }
            };
            ServiceLocator.RegisterInstance<ISurveyDataService>(stubISurveyDataService);

            var stub = IntegrationTestingFramework.Instance.RegistryStub<ISurveyDatabaseInfoProvider, StubISurveyDatabaseInfoProvider>();
            stub.GetRespondentFieldsInfoInt32 = (s) => new SurveyDatabaseFieldInfo[] { };
            stub.GetFormInfoInt32String = (s, n) => new SurveyDatabaseFormInfo
            {
                Name = n,
                Fields = new[] { new SurveyDatabaseFieldInfo() { FieldName = n, TableName = "response0" } },
                LoopPath = new[] { "responseid" }
            };

            ServiceLocator.Resolve<ISystemSettings>().SchedulingScript.UseDirectDbAccess = false;
            return stubISurveyDataService;
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Q1Exists_ReadQ1_ReadSuccessedAndSchedulingIsCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            MockGetFormInfo(
                new FormBase[]
                    {
                        new SingleForm
                            {
                                Name = "q1", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new Answer { Precode = "2" } } }
                            }
                    });

            var formsValues = new Dictionary<string, string> { { "q1", "1" }, };

            MockFormValues(formsValues);

            var script =
                new TestScript(
                    new Action(Action.Operation.SetNewITS, NewIts.ToString(CultureInfo.InvariantCulture), "f('q1').get() == 1"),
                    @"Scheduling2007\Schedule.xml");


            int surveySid = BackendToolsObject.CreateSurvey(script, ProjectId);


            var interview = BackendTools.NewInterview(surveySid);
            interview.TransientState = InitIts;
            BackendTools.CreateInterview(interview);

            //move and reschedule
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            interview.TransientState = NewIts;

            BackendTools.CheckInterview(interview);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Q1Exists_ReadQ1_ReadSuccessedAndSchedulingIsFiltered(SecurityMode mode)
        {
            SetSecurityMode(mode);

            MockGetFormInfo(
                new FormBase[]
                    {
                        new SingleForm
                            {
                                Name = "q1", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new Answer { Precode = "2" } } }
                            }
                    });

            var formsValues = new Dictionary<string, string> { { "q1", "2" }, };

            MockFormValues(formsValues);

            var script =
                new TestScript(
                    new Action(Action.Operation.SetNewITS, NewIts.ToString(CultureInfo.InvariantCulture), "f('q1').get() == 1"),
                    @"Scheduling2007\Schedule.xml");


            int surveySid = BackendToolsObject.CreateSurvey(script, ProjectId);


            var interview = BackendTools.NewInterview(surveySid);
            interview.TransientState = InitIts;
            BackendTools.CreateInterview(interview);

            //move and reschedule
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            BackendTools.CheckInterview(interview);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Q1Q2Exists_ReadQ1AndWriteQ2_ReadWriteSuccessedAndSchedulingIsCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            MockGetFormInfo(
                new FormBase[]
                    {
                        new SingleForm
                            {
                                Name = "q1", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new Answer { Precode = "2" } } }
                            },
                        new SingleForm
                            {
                                Name = "q2", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new Answer { Precode = "2" } } }
                            }
                    });

            var script =
                new TestScript(
                    new Action(Action.Operation.RunCustomScript, "customFunc", "f('q1').get() == 1"),
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                {
                    CustomScript = @"function customFunc()
                                    {
                                       f('q2').setValue(2);
                                    }"
                };

            var formsValues = new Dictionary<string, string> { { "q1", "1" }, { "q2", "1" } };

            MockFormValues(formsValues);

            int surveySid = BackendToolsObject.CreateSurvey(script, ProjectId);

            var interview = BackendTools.NewInterview(surveySid);
            interview.TransientState = InitIts;
            BackendTools.CreateInterview(interview);

            //move and reschedule
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            BackendTools.CheckInterview(interview);
            Assert.AreEqual("2", formsValues.Single(x => x.Key == "q2").Value);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Q1WithHeaderCategories_ReadQ1_ReadSuccessedAndSchedulingIsCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            MockGetFormInfo(
                new FormBase[]
                    {
                        new SingleForm
                            {
                                Name = "q1", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new HeaderAnswer(), new Answer { Precode = "2" }, new HeaderAnswerEnd() }
                                                           }
                            }
                    });

            var formsValues = new Dictionary<string, string> { { "q1", "1" }, };

            MockFormValues(formsValues);

            var script =
                new TestScript(
                    new Action(Action.Operation.SetNewITS, NewIts.ToString(CultureInfo.InvariantCulture), "f('q1').get() == 1"),
                    @"Scheduling2007\Schedule.xml");


            int surveySid = BackendToolsObject.CreateSurvey(script, ProjectId);


            var interview = BackendTools.NewInterview(surveySid);
            interview.TransientState = InitIts;
            BackendTools.CreateInterview(interview);

            //move and reschedule
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            interview.TransientState = NewIts;
            BackendTools.CheckInterview(interview);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Q1Q2WithEmptyPrecodesAndGroups_ReadQ1AndWriteQ2_ReadWriteSuccessedAndSchedulingIsCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            MockGetFormInfo(
                new FormBase[]
                    {
                        new SingleForm
                            {
                                Name = "q1", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new HeaderAnswer(), new Answer { Precode = "2" },
                                    new Answer(), new Answer(), new HeaderAnswerEnd(), new Answer { Precode = "3" }, new Answer() } }
                            },
                        new SingleForm
                            {
                                Name = "q2", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new HeaderAnswer(), new Answer { Precode = "2" },
                                    new Answer(), new Answer(), new HeaderAnswerEnd(), new Answer { Precode = "3" }, new Answer() } }
                            }
                    });

            var script =
                new TestScript(
                    new Action(Action.Operation.RunCustomScript, "customFunc", "f('q1').get() == 1"),
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                {
                    CustomScript = @"function customFunc()
                        {
                           f('q2').setValue(2);
                        }"
                };

            var formsValues = new Dictionary<string, string> { { "q1", "1" }, { "q2", "1" } };

            MockFormValues(formsValues);

            int surveySid = BackendToolsObject.CreateSurvey(script, ProjectId);


            var interview = BackendTools.NewInterview(surveySid);
            interview.TransientState = InitIts;
            BackendTools.CreateInterview(interview);

            //move and reschedule
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            BackendTools.CheckInterview(interview);
            Assert.AreEqual("2", formsValues.Single(x => x.Key == "q2").Value);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void f_ReadQ1_FilterIsFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new FormData[]
                        {
                            new SingleFormData(){Name="q1", Precodes = new[]{"1","2","3"}}
                        },
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1=2"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.SetNewITS, NewIts.ToString(CultureInfo.InvariantCulture), "f('q1').get() == 1"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISchedulingScriptSettings>().UseDirectDbAccess = true;

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            interview.Assert.IsTrue(x => x.TransientState == InitIts);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void f_ReadQ1_FilterIsTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new FormData[]
                        {
                            new SingleFormData(){Name="q1", Precodes = new[]{"1","2","3"}}
                        },
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1=1"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.SetNewITS, NewIts.ToString(CultureInfo.InvariantCulture), "f('q1').get() == 1"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISchedulingScriptSettings>().UseDirectDbAccess = true;

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            interview.Assert.IsTrue(x => x.TransientState == NewIts);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void f_WriteQ1_ValueAreUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new FormData[]
                        {
                            new SingleFormData(){Name="q1", Precodes = new[]{"1","2","3"}}
                        },
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1=2"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "func"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @"function func(){f('q1').setValue(1);}"
                        }
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISchedulingScriptSettings>().UseDirectDbAccess = true;

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var q1 = ServiceLocator.Resolve<ISurveyDatabaseEngine>().ExecuteScalar<int>(
                survey.Id,
                @"SELECT q1 FROM <Schema>.response0 WHERE respid = @respId",
            new SqlParameter("@respId", interview.Id));

            TestAssert.AreEqual(1, q1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void f_ReadQ1WithNull_FilterIsTrueAndResultOfFFunctionIsEmptyString(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new FormData[]
                        {
                            new FormData(){Name="q1",}
                        },
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.SetNewITS, NewIts.ToString(CultureInfo.InvariantCulture), "f('q1').get() == ''"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISchedulingScriptSettings>().UseDirectDbAccess = true;

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            interview.Assert.IsTrue(x => x.TransientState == NewIts);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void f_ReadQ1AfterUpdate_FilterIsTrueAndResultOfFFunctionIsEmptyString(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new FormData[]
                        {
                            new FormData(){Name="q1"}
                        },
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1=1"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "custom_function"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @"function custom_function(){
            var q1 : int = f('q1').get(); 
            f('q1').setValue('2'); 
            var q1_new : int = f('q1').get();
            q1 = q1 + q1_new;
            CallShouldBeCreated();
            Scheduling.NewCall.Priority = q1; }"
                        }
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISchedulingScriptSettings>().UseDirectDbAccess = true;

            var interview = context.GetInterview("S1.I1");
            var call = context.GetCall("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            call.Assert.IsTrue(x => x.Priority == 3);


        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void f_WriteQ1WithNullValue_ValueAreUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new FormData[]
                        {
                            new FormData(){Name="q1"}
                        },
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1=2"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "func"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @"function func(){f('q1').setValue(null);}"
                        }
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISchedulingScriptSettings>().UseDirectDbAccess = true;

            var interview = context.GetInterview("S1.I1");
            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            Assert.AreEqual("NULL", InterviewResultParam.ParseFromString(interview.GetData("q1"))[0].ParamValue);
        }

        [Theory, Owner(@"FIRM\OlegM")]
        [ClassData(typeof(TestDataGenerator))]
        public void f_ReadMultiQuestion_ValueIsReturned(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new FormData[]
                        {
                            new MultiFormData{Name="q1",  IsNumeric = true, Precodes = new[]{"1", "2"}}
                        },
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1_1=10,q1_2=20"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "func"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @"function func(){ Scheduling.Interview.TransientState = 10 + f('q1')['2'].toInt();}"
                        }
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISchedulingScriptSettings>().UseDirectDbAccess = true;

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            interview.Assert.IsTrue(x => x.TransientState == 30);
        }

        [Theory, Owner(@"FIRM\OlegM")]
        [ClassData(typeof(TestDataGenerator))]
        public void f_WriteMultiQuestion_ValueIsUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new FormData[]
                        {
                            new MultiFormData{Name="q1",  IsNumeric = true, Precodes = new[]{"1", "2"}}
                        },
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1_1=10,q1_2=20"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "func"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @"function func(){ f('q1')['2'].setValue('15');}"
                        }
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISchedulingScriptSettings>().UseDirectDbAccess = true;

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            var result = InterviewResultParam.ParseFromString(interview.GetData("q1_1,q1_2"));

            Assert.AreEqual("10", result.Single(x => x.ParamId == "q1_1").ParamValue);
            Assert.AreEqual("15", result.Single(x => x.ParamId == "q1_2").ParamValue);
        }

        [Theory, Owner(@"FIRM\OlegZ")]
        [ClassData(typeof(TestDataGenerator))]
        public void Q1Q2Exists_ReadQ1AndWriteQ2_RaiseCommunicationExeption_ReadWriteSuccessedAndSchedulingIsCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            MockGetFormInfo(
                new FormBase[]
                    {
                        new SingleForm
                            {
                                Name = "q1", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new Answer { Precode = "2" } } }
                            },
                        new SingleForm
                            {
                                Name = "q2", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new Answer { Precode = "2" } } }
                            }
                    });

            var script =
                new TestScript(
                    new Action(Action.Operation.RunCustomScript, "customFunc", "f('q1').get() == 1"),
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                {
                    CustomScript = @"function customFunc()
                                    {
                                       f('q2').setValue(2);
                                    }"
                };

            var formsValues = new Dictionary<string, string> { { "q1", "1" }, { "q2", "1" } };

            var stubISurveyDataService = MockFormValues(formsValues);
            var getDataInternalBehavior = stubISurveyDataService.GetDataTransferDefBaseResponseToken;
            var updateDataInternalBehavior = stubISurveyDataService.UpdateDataTransferDefDataSetBooleanBooleanInt32;
            var gettingRunCounter = 0;
            var updatingRunCounter = 0;
            stubISurveyDataService.GetDataTransferDefBaseResponseToken =
                (transferDef, token) =>
                {
                    if (gettingRunCounter > 0) return getDataInternalBehavior(transferDef, token);

                    gettingRunCounter++;
                    throw new CommunicationException("Test CommunicationException");
                };
            stubISurveyDataService.UpdateDataTransferDefDataSetBooleanBooleanInt32 =
                (transferDef, dataSet, applyRules, inTransaction, transactionKey) =>
                {
                    if (updatingRunCounter > 0) return updateDataInternalBehavior(transferDef, dataSet, applyRules, inTransaction, transactionKey);

                    updatingRunCounter++;
                    throw new CommunicationException("Test CommunicationException");
                };

            int surveySid = BackendToolsObject.CreateSurvey(script, ProjectId);

            var interview = BackendTools.NewInterview(surveySid);
            interview.TransientState = InitIts;
            BackendTools.CreateInterview(interview);

            //move and reschedule
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            BackendTools.CheckInterview(interview);
            Assert.AreEqual("2", formsValues.Single(x => x.Key == "q2").Value);
            Assert.IsFalse(gettingRunCounter == 0);
            Assert.IsFalse(updatingRunCounter == 0);
        }

        [Theory, Owner(@"FIRM\OlegZ")]
        [ClassData(typeof(TestDataGenerator))]
        public void Q1Q2Exists_ReadQ1AndWriteQ2_GetDataTransferRaiseCommunicationExeptionAndNullReference_ReadNotSuccessedAndSchedulingNotProcessed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            MockGetFormInfo(
                new FormBase[]
                    {
                        new SingleForm
                            {
                                Name = "q1", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new Answer { Precode = "2" } } }
                            },
                        new SingleForm
                            {
                                Name = "q2", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new Answer { Precode = "2" } } }
                            }
                    });

            var script =
                new TestScript(
                    new Action(Action.Operation.RunCustomScript, "customFunc", "f('q1').get() == 1"),
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                {
                    CustomScript = @"function customFunc()
                                    {
                                       f('q2').setValue(2);
                                    }"
                };

            var formsValues = new Dictionary<string, string> { { "q1", "1" }, { "q2", "1" } };

            var stubISurveyDataService = MockFormValues(formsValues);
            var getDataInternalBehavior = stubISurveyDataService.GetDataTransferDefBaseResponseToken;
            var gettingRunCounter = 0;
            stubISurveyDataService.GetDataTransferDefBaseResponseToken =
                (transferDef, token) =>
                {
                    if (gettingRunCounter == 1) throw new NullReferenceException("Test NullReferenceException");
                    if (gettingRunCounter > 0) return getDataInternalBehavior(transferDef, token);

                    gettingRunCounter++;
                    throw new CommunicationException("Test CommunicationException");
                };

            int surveySid = BackendToolsObject.CreateSurvey(script, ProjectId);

            var interview = BackendTools.NewInterview(surveySid);
            interview.TransientState = InitIts;
            BackendTools.CreateInterview(interview);

            //move and reschedule
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            BackendTools.CheckInterview(interview);
            Assert.AreEqual("1", formsValues.Single(x => x.Key == "q2").Value);
            Assert.IsTrue(gettingRunCounter > 0);
        }

        [Theory, Owner(@"FIRM\OlegZ")]
        [ClassData(typeof(TestDataGenerator))]
        public void Q1Q2Exists_ReadQ1AndWriteQ2_UpdateDataTransferRaiseCommunicationExeptionAndNullReference_WriteNotSuccessedAndSchedulingNotProcessed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            MockGetFormInfo(
                new FormBase[]
                    {
                        new SingleForm
                            {
                                Name = "q1", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new Answer { Precode = "2" } } }
                            },
                        new SingleForm
                            {
                                Name = "q2", FormTexts = new FormText[]{},
                                SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { new Answer { Precode = "1" }, new Answer { Precode = "2" } } }
                            }
                    });

            var script =
                new TestScript(
                    new Action(Action.Operation.RunCustomScript, "customFunc", "f('q1').get() == 1"),
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                {
                    CustomScript = @"function customFunc()
                                    {
                                       f('q2').setValue(2);
                                    }"
                };

            var formsValues = new Dictionary<string, string> { { "q1", "1" }, { "q2", "1" } };

            var stubISurveyDataService = MockFormValues(formsValues);
            var updateDataInternalBehavior = stubISurveyDataService.UpdateDataTransferDefDataSetBooleanBooleanInt32;
            var updatingRunCounter = 0;
            stubISurveyDataService.UpdateDataTransferDefDataSetBooleanBooleanInt32 =
                (transferDef, dataSet, applyRules, inTransaction, transactionKey) =>
                {
                    if (updatingRunCounter == 1) throw new NullReferenceException("Test NullReferenceException");
                    if (updatingRunCounter > 0) return updateDataInternalBehavior(transferDef, dataSet, applyRules, inTransaction, transactionKey);

                    updatingRunCounter++;
                    throw new CommunicationException("Test CommunicationException");
                };

            int surveySid = BackendToolsObject.CreateSurvey(script, ProjectId);

            var interview = BackendTools.NewInterview(surveySid);
            interview.TransientState = InitIts;
            BackendTools.CreateInterview(interview);

            //move and reschedule
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            BackendTools.CheckInterview(interview);
            Assert.AreEqual("1", formsValues.Single(x => x.Key == "q2").Value);
            Assert.IsTrue(updatingRunCounter > 0);
        }

    }

    internal class InterviewResultParam
    {
        public string ParamId { get; private set; }

        public string ParamValue { get; private set; }

        public static InterviewResultParam[] ParseFromString(string input)
        {
            return input.Split(',').Select(x =>
            {
                var pair = x.Split('=');
                return new InterviewResultParam { ParamId = pair[0], ParamValue = pair[1] };
            }).ToArray();
        }
    }
}
