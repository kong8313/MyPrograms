using System;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.Survey.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class SurveyDataBuilder : BaseObjectBuilder<SurveyData>
    {
        private SurveyController _controller;

        private readonly ISurveyRepository _surveyRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IQuotaClusteringConfigurationService _quotaClusteringConfigurationService;

        public ISurveyDatabaseBuilder Database { get; private set; }

        public SurveyDataBuilder(TestDataContext context, SurveyData data, DataGenerator dataGenerator)
            : base(context, data, dataGenerator)
        {
            _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
            _interviewRepository = ServiceLocator.Resolve<IInterviewRepository>();
            _quotaClusteringConfigurationService = ServiceLocator.Resolve<IQuotaClusteringConfigurationService>();
        }

        public override void Create()
        {
            CheckAndInitData();

            int surveyId;

            if (Data.IsUseDb)
            {
                Database = SurveyDatabaseBuilder.Create(Data.ProjectId, Data.Forms);

                Database.CreateRespondentTable(Data.Forms.Where(x => x.TableName == "respondent"));

                foreach (var tableColumns in Data.Forms.GroupBy(x => x.TableName).Where(x => x.Key != "respondent"))
                {
                    Database.CreateResponseTable(tableColumns.Key, tableColumns);
                }

                Database.CreateQuotaTables();

                Database.CreateFormAndFieldTable(Data.Forms);

                LaunchSurvey(Database, Data, true);
                surveyId = _surveyRepository.GetByName(Database.ProjectId).SID;
                
                ServiceLocator.Register<IRespondentVariablesService, RespondentVariablesService>();
                ServiceLocator.Register<ISurveyDatabaseService, SurveyDatabaseService>();
            }
            else
            {
                surveyId = new BackendTools(IntegrationTestingFramework.Instance).CreateSurvey(Data.ProjectId, true);

                SurveyService.SetDialingMode(surveyId, Data.DialMode);

                var survey = SurveyRepository.GetById(surveyId);
                survey.IsTelephoneBlacklistSupported = true;
                survey.InterviewScreenRecording = Data.ScreenRecording;
                SurveyRepository.Update(survey);

                Database = DummySurveyDatabaseBuilder.Create(surveyId);
                IntegrationTestingFramework.Instance.RegistryStub<ISurveyDatabaseService, StubISurveyDatabaseService>();
            }

            if (Data.Assigns.Length > 0)
            {
                var ids = Data.Assigns.Select(x => Context.GetResource(x).Id);
                AssignmentService.AssignResourcesToSurvey(surveyId, ids, 1);
            }

            _controller = new SurveyController(Data.Tag, surveyId, Context, Data, this);
            Context.Surveys.Add(_controller);
        }

        public static void LaunchSurvey(ISurveyDatabaseBuilder sdbBuilder, SurveyData data, bool first)
        {
            if (!first)
            {
                sdbBuilder.ClearQuotaTables();
            }

            foreach (var quota in data.Quotas)
            {
                sdbBuilder.CreateQuota(quota, data.Forms);
            }

            TableInfo[] tableInfo = data.Forms.Where(x => x.IsReplicated).GroupBy(f => f.TableName)
                .Select(table => new TableInfo
                {
                    Name = table.Key,
                    PrimaryKeyColumns = new[]
                    {
                        new ColumnInfo
                        {
                            DataType = SqlDataType.Int,
                            Name = table.Key == "respondent" ? "respid" : "responseid"
                        }
                    },
                    ReplicationColumns = table.Select(x => new ReplicationColumnInfo
                    {
                        Id = x.ColumnId,
                        DataType = x.SqlType,
                        Name = x.Name,
                        QuotaIds = data.Quotas.Where(q => q.Fields.Contains(x.Name)).Select(y => y.Id).ToArray(),
                        MaxLength = x.SqlType == SqlDataType.NVarChar ? 255 : 0
                    }).ToArray()
                })
                .ToArray();

            if (first)
            {
                sdbBuilder.EnableChangeTracking(tableInfo);

            }

            new BackendTools(IntegrationTestingFramework.Instance).LaunchSurvey(
                sdbBuilder.ProjectId,
                new LaunchSurveyParameters
                {
                    PermittedUsers = new[] { "user1" },
                    RemoveData = false,
                    ReplicatedTables = tableInfo,
                    SurveyProperties = new SurveyProperties
                    {
                        CreatedUserName = "MaximL",
                        CfSqlServerConnectionString = sdbBuilder.ConnectionString,
                        DialingMode = (int)data.DialMode,
                        EnforceHttps = false,
                        NotificationEmail = "a@firmsw.no",
                        OpenEndReview = data.OpenEndReview,
                        ProjectName = "Survey name",
                        ReplicationStatus = true,
                        ScreenRecording = data.ScreenRecording,
                        SupportBlacklist = data.IsSupportBlackList,
                        VoiceRecording = false,
                    }
                });
        }

        private void CheckAndInitData()
        {
            if (Data.ProjectId == null)
            {
                Data.ProjectId = DataGenerator.NewProjectId();
            }
            var cntOfReplicatedColumns = Data.Forms.Count(x => x.IsReplicated);

            Data.Forms = Data.Forms.Union(new[]
            {
                new FormData { Name = "CallAttemptCount",SqlType = SqlDataType.Int, TableName = "respondent", ColumnId = cntOfReplicatedColumns++ },
                new FormData { Name = "TimeZoneId", SqlType = SqlDataType.Int, TableName = "respondent", ColumnId = cntOfReplicatedColumns++ },
                new FormData { Name = "TelephoneNumber", SqlType = SqlDataType.NVarChar, TableName = "respondent", ColumnId = cntOfReplicatedColumns++ },
                new FormData { Name = "RespondentName", SqlType = SqlDataType.NVarChar,TableName = "respondent", ColumnId = cntOfReplicatedColumns++ },
                new FormData { Name = "ExtensionNumber", SqlType = SqlDataType.NVarChar,TableName = "respondent", ColumnId = cntOfReplicatedColumns++ },
                new FormData { Name = "DialType", SqlType = SqlDataType.Int, TableName = "respondent", ColumnId = cntOfReplicatedColumns },

            }).ToArray();

            var form = Data.Forms.Where(q => q.Name != null).GroupBy(x => x.Name).FirstOrDefault(g => g.Count() > 1);
            if (form != null)
            {
                Assert.Fail("Survey configuration contains forms with the same names '{0}'.", form.Key);
            }

            var groupId = Data.Quotas.Where(q => q.Id != 0).GroupBy(x => x.Id).FirstOrDefault(g => g.Count() > 1);
            if (groupId != null)
            {
                Assert.Fail("Survey configuration contains quotas with same ids '{0}'.", groupId.Key);
            }

            var groupName = Data.Quotas.Where(q => !String.IsNullOrEmpty(q.Name)).GroupBy(x => x.Name).FirstOrDefault(g => g.Count() > 1);
            if (groupName != null)
            {
                Assert.Fail("Survey configuration contains quotas with the same names '{0}'.", groupName.Key);
            }

            var field = Data.Quotas.SelectMany(x => x.Fields).FirstOrDefault(fieldName => Data.Forms.All(x => x.Name != fieldName));
            if (field != null)
            {
                Assert.Fail("Quota contains not declared field '{0}'.", field);
            }

            foreach (var quota in Data.Quotas)
            {
                if (quota.Id == 0)
                {
                    quota.Id = Enumerable.Range(1, 10000)
                        .First(id => Data.Quotas.All(q => q.Id != id));
                }
                if (String.IsNullOrEmpty(quota.Name))
                {
                    quota.Name = Enumerable.Range(1, 10000)
                        .Select(id => String.Format("quota{0}", id))
                        .First(name => Data.Quotas.All(q => q.Name != name));
                }
            }
        }

        public override void Setup()
        {
            int totalInterviews = 0;
            if (!String.IsNullOrEmpty(Data.ClusterQuota))
            {
                _quotaClusteringConfigurationService.Configure(_controller.Id, new QuotaClusteringConfiguration { LiveThreshod = Data.ClusterQuotaThreshold, QuotaName = Data.ClusterQuota });
                BackendTools.ExecuteAllAsyncOperations();
            }

            SetupSurveyProperties();

            var batchId = Database.GetNewBatchId();
            using (var sampleStorage = ServiceLocator.Resolve<ISampleDataStorageRepository>().Create(batchId, new BvSurveyEntity { SID = _controller.Id }, 1, false))
            {
                foreach (var interviewData in Data.Interviews)
                {
                    var pepsonOrGroupIds = Context.GetResources(Data.Assigns).Select(x => x.Id).ToArray();
                    AssignmentService.AssignResourcesToSurvey(_controller.Id, pepsonOrGroupIds, 1);

                    foreach (var callCenter in Context.CallCenters)
                    {
                        AssignmentService.AssignResourcesToSurvey(_controller.Id, pepsonOrGroupIds, callCenter.Id);
                    }

                    for (int index = 0; index < interviewData.Count; index++)
                    {
                        totalInterviews++;
                        var respId = Database.AddInterview(batchId, null, interviewData);
                        int? lastCallPersonSid = null;

                        if (!string.IsNullOrEmpty(interviewData.LastCallPerson))
                            lastCallPersonSid = Context.GetPerson(interviewData.LastCallPerson).Id;

                        var interview = new BvInterviewEntity
                        {
                            ID = respId,
                            SurveySID = _controller.Id,
                            TransientState = (int)interviewData.ITS,
                            BatchID = batchId,
                            TelephoneNumber = interviewData.TelephoneNumber,
                            ExtensionNumber = interviewData.ExtensionNumber,
                            RespondentName = String.IsNullOrEmpty(interviewData.RespondentName) ?
                                String.Format("respondent{0}", respId) :
                                interviewData.RespondentName,
                            LastCallPersonSID = lastCallPersonSid,
                            ReviewStatus = (byte)interviewData.ReviewStatus,
                            DialTypeId = (byte)interviewData.DialType,
                            DialingMode = (byte)int.Parse(interviewData.DialMode ?? "0"),
                            LastChannelID = (byte)int.Parse(interviewData.LastChannelId ?? "0")

                        };

                        int tzId;
                        if (int.TryParse(interviewData.TimeZoneId, out tzId))
                            interview.TimezoneID = tzId;

                        
                        _interviewRepository.Insert(new BvInterviewWithOriginEntity(interview), new SchedulingScriptExecutionOptions() { IsExecuteSchedulingScript = false, IsLogToHistory = false, BatchID = batchId });

                        RegistryInterviewInContext(respId, interviewData);

                        if (interviewData.Call != null)
                        {
                            interviewData.Call.Model.InterviewID = interview.ID;
                            interviewData.Call.Model.SurveySID = interview.SurveySID;
                            interviewData.Call.Model.DialTypeId = interview.DialTypeId;

                            if (!String.IsNullOrEmpty(interviewData.Call.Resource))
                            {
                                var resources = interviewData.Call.Resource.Split(',')
                                    .Select(r => Context.GetResource(r).Id)
                                    .ToArray();
                                int id = ServiceLocator.Resolve<IAssignmentService>().GetAssignmentResourceId(resources);

                                interviewData.Call.Model.Resource = id;
                            }

                            if (Data.IsCallGroupEnabled)
                                interviewData.Call.Model.ConditionValue = interview.TransientState;

                            CallQueueService.AddCall(interviewData.Call.Model, batchId, interview.TransientState, SchedulingScriptExecutionReason.AddedBySample);
                        }

                        sampleStorage.SaveCurrentRecord();

                        if (interviewData.CallHistory != null)
                        {
                            foreach (var callHistoryData in interviewData.CallHistory)
                            {
                                var person = Context.GetPerson(callHistoryData.Person);
                                BvHistoryAdapter.Insert(new BvHistoryEntity
                                {
                                    FiredTime = callHistoryData.FiredTime,
                                    ITS = (byte)callHistoryData.ITS,
                                    TelephoneNumber = callHistoryData.TelephoneNumber,
                                    Duration = callHistoryData.Duration,
                                    WaitingTime = callHistoryData.WaitingTime,
                                    CallCenterID = callHistoryData.CallCenterId,
                                    InterviewId = interview.ID,
                                    SurveyId = _controller.Id,
                                    PersonSID = person.Id,
                                    RoleID = 2
                                });
                            }
                        }

                        if (interviewData.ExtendedCallHistory != null)
                        {
                            foreach (var data in interviewData.ExtendedCallHistory)
                            {
                                BvCallHistoryExAdapter.Insert(new BvCallHistoryExEntity
                                {
                                    FiredTime = data.FiredTime,
                                    ITS = (byte)data.ITS,
                                    InterviewID = interview.ID,
                                    SurveyId = _controller.Id,
                                    OperationType = (byte)data.OperationType,
                                    OperationId = data.OperationId,
                                    DialTypeId = interview.DialTypeId
                                });
                            }
                        }

                        if (interviewData.History != null)
                        {
                            foreach (var history in interviewData.History)
                            {
                                BvHistoryAdapter.Insert(new BvHistoryEntity()
                                {
                                    SurveyId = _controller.Id,
                                    InterviewId = interview.ID,
                                    AppointmentID = 0,
                                    BatchId = 0,
                                    CallCenterID = GetCallCenterIdByPerson(history.Person),
                                    ConfirmitDuration = 0,
                                    Duration = history.Duration,
                                    FiredTime = DateTime.Parse(history.Time),
                                    ITS = (short)history.ITS,
                                    RoleID = (byte)history.Role,
                                    PersonSID = history.Person == null ? 0 : Context.GetPerson(history.Person).Id,
                                    TelephoneNumber = history.TelephoneNumber
                                });
                            }
                        }
                    }
                }

                sampleStorage.Commit(new DummyEventDetails());
            }

            var survey = _surveyRepository.GetByName(Database.ProjectId);
            BvAsyncOperationQueueAdapter.Insert(new BvAsyncOperationQueueEntity
            {
                SurveySid = _controller.Id,
                State = 2,
                Text = "",
                StartedDate= DateTime.UtcNow,
                FinishedDate = DateTime.UtcNow,
                TotalItemsCount = totalInterviews,
                ProcessedItemsCount = totalInterviews,
                FailedItemsCount = 0,
                IsInitiatedBySystem = false,
                Type = (int)OperationTypes.SampleUpload,
                Title = $"Process sample for '{survey.ProjectId}' ({survey.Description})",
                Parameters = $@"<Parameters xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
<SurveyId>{survey.SID}</SurveyId>
<ProjectId>{survey.ProjectId}</ProjectId>
<BatchId>{batchId}</BatchId>
<ProcessSampleMode>Add</ProcessSampleMode>
<SchedulingMode>Simple</SchedulingMode>
</Parameters>",
                Priority = 30,
                QueuedDate = DateTime.UtcNow,
                HeartBeat = DateTime.UtcNow,
                CreatedBySupervisorName = "administrator",
                Server = Environment.MachineName,
                Error = string.Empty,
                CallCenterId = 0
            });

            BvSamplesAdapter.Insert(new BvSamplesEntity
            {
                BatchID = batchId,
                SurveySID = _controller.Id,
                State = 2,
                StateDescription = "",
                StartedTime = DateTime.UtcNow,
                FinishedTime = DateTime.UtcNow,
                CountInterviews = totalInterviews
            });
            
            var replicationService = ServiceLocator.Resolve<IReplicationService>();
            replicationService.UploadSampleDataToReplicatedTable(survey.SID, batchId, CancellationToken.None);

            if (Data.IsOpen && !Data.IsSoftDeleted)
                ServiceLocator.Resolve<ISurveyStateService>().Open(_controller.Id);            

            if (Data.Assigns.Length > 0)
            {
                var ids = Data.Assigns.Select(x => Context.GetResource(x).Id);
                AssignmentService.AssignResourcesToSurvey(_controller.Id, ids, 1);
            }

            if (Data.CallCenters != null)
            {
                var callCentersService = ServiceLocator.Resolve<ICallCenterService>();
                var callCenterIds = Context.GetCallCenters(Data.CallCenters).Select(x => x.Id);
                callCentersService.AssignSurveys(callCenterIds, new[] { _controller.Id });
            }

            if (Data.InboundTelephoneNumbers != null)
            {
                foreach (var inboundCall in Data.InboundTelephoneNumbers)
                {
                    var dialer = Context.GetDialer(inboundCall.Dialer);
                    BvInboundTelephoneNumberAdapter.Insert(new BvInboundTelephoneNumberEntity
                    {
                        SurveyId = _controller.Id,
                        TelephoneNumber = inboundCall.TelephoneNumber,
                        DialerId = dialer.Id
                    });
                }
            }
        }

        private int GetCallCenterIdByPerson(string personTag)
        {
            if (personTag == null)
                return 0;
            
            var person = Context.GetPerson(personTag).Data;
            if (person.CallCenter == null)
                return 0;
            
            var callCenter = Context.GetCallCenter(person.CallCenter);
            return callCenter.Id;
        }
        
        private void SetupSurveyProperties()
        {
            var model = _controller.Model;

            if (!String.IsNullOrEmpty(Data.SchedulingScript))
            {
                model.ScheduleID = Context.GetScript(Data.SchedulingScript).Model.ScheduleID;
            }

            model.SurveySchedulingMode = Data.IsCallGroupEnabled
                ? (short)SurveySchedulingMode.CallGroup
                : (short)SurveySchedulingMode.Normal;

           // model.IsQuotaInCatiDb = Data.IsQuotaInCatiDb;
            model.InboundBehavior = Data.InboundBehavior;

            _surveyRepository.Update(model);

            if (Data.IsSoftDeleted)
            {
                model.State = (int)SurveyState.SoftDeleted;
                BvSurveyAdapter.Update(model);
            }
        }

        public void RegistryInterviewInContext(int interviewId, InterviewData interviewData)
        {
            var interview = new InterviewController(interviewData.Tag, Context, _controller, interviewId, Database, interviewData);
            Context.Interviews.Add(interview);
            Context.Calls.Add(new CallRef(interview));
        }
    }
}