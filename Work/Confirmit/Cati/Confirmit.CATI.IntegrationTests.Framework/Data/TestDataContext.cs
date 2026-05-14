using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data.Builders;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.DAL.Framework;

namespace Confirmit.CATI.IntegrationTests.Framework.Data
{
    public class TestDataContext
    {
        public List<SurveyController> Surveys = new List<SurveyController>();
        public List<InterviewController> Interviews = new List<InterviewController>();
        public List<CallRef> Calls = new List<CallRef>();
        public List<PersonController> Persons = new List<PersonController>();
        public List<PersonGroupController> PersonGroups = new List<PersonGroupController>();
        public List<DialerController> Dialers = new List<DialerController>();
        public List<ScriptController> Scripts = new List<ScriptController>();
        public List<CallCenterController> CallCenters = new List<CallCenterController>();
        public List<CallGroupController> CallGroups = new List<CallGroupController>();
        public List<SupervisorController> Supervisors = new List<SupervisorController>();
        public List<InboundCallHistoryController> InboundCallHistories = new List<InboundCallHistoryController>();
        public List<FilterController> Filters = new List<FilterController>();
        public List<ExternalNumberController> ExternalNumbers = new List<ExternalNumberController>();

        public IEnumerable<IRef> GetAll()
        {
            return Surveys.Cast<IRef>()
                .Union(Interviews)
                .Union(Calls)
                .Union(Persons)
                .Union(PersonGroups)
                .Union(Dialers)
                .Union(Scripts)
                .Union(CallCenters)
                .Union(CallGroups)
                .Union(Supervisors)
                .Union(InboundCallHistories)
                .Union(Filters)
                .Union(ExternalNumbers);
        }

        public SurveyController GetSurvey(string tag)
        {
            return Surveys.Single(x => x.Tag == tag);
        }

        public ArrayController<SurveyController, BvSurveyEntity> GetSurveys(params string[] tags)
        {
            return new ArrayController<SurveyController, BvSurveyEntity>(tags.Select(GetSurvey));
        }

        public InterviewController GetInterview(string tag)
        {
            return Interviews.Single(x => x.Tag == tag);
        }

        public IArrayController<InterviewController, BvInterviewWithOriginEntity> GetInterviews(params string[] tags)
        {
            return new ArrayController<InterviewController, BvInterviewWithOriginEntity>(Interviews.Where(x => tags.Contains(x.Tag)));
        }

        public IArrayController<InterviewController, BvInterviewWithOriginEntity> GetInterviewsInOrder(params string[] tags)
        {
            return new ArrayController<InterviewController, BvInterviewWithOriginEntity>(tags.Select(x => Interviews.SingleOrDefault(y => y.Tag == x)).Where(z => z != null));
        }

        public CallRef GetCall(string tag)
        {
            return Calls.Single(x => x.Tag == tag);
        }

        public IArrayController<CallRef, BvCallEntity> GetCalls(params string[] tag)
        {
            return new ArrayController<CallRef, BvCallEntity>(Calls.Where(x => tag.Contains(x.Tag)));
        }

        public IResourceController GetResource(string tag)
        {
            return GetResources(tag).Single();
        }

        public IResourceController[] GetResources(params string[] tags)
        {
            return Persons.Concat(PersonGroups.Cast<IResourceController>())
                .Where(x => tags.Contains(x.Tag)).ToArray();
        }

        public PersonGroupController GetPersonGroup(string tag)
        {
            return PersonGroups.Single(x => x.Tag == tag);
        }

        public PersonController[] GetPersons(params string[] tags)
        {
            return Persons.Where(x => tags.Contains(x.Tag)).ToArray();
        }

        public PersonController GetPerson(string tag)
        {
            return Persons.Single(x => x.Tag == tag);
        }

        public DialerController GetDialer(string tag)
        {
            return Dialers.Single(x => x.Tag == tag);
        }

        public ScriptController GetScript(string tag)
        {
            return Scripts.Single(x => x.Tag == tag);
        }

        public CallCenterController GetCallCenter(string tag)
        {
            return CallCenters.Single(x => x.Tag == tag);
        }

        public CallCenterController[] GetCallCenters(params string[] tags)
        {
            return CallCenters.Where(x => tags.Contains(x.Tag)).ToArray();
        }

        public CallGroupController GetCallGroup(string tag)
        {
            return CallGroups.Single(x => x.Tag == tag);
        }

        public SupervisorController GetSupervisor(string tag)
        {
            return Supervisors.Single(x => x.Tag == tag);
        }

        public InboundCallHistoryController GetInboundHistory(string tag)
        {
            return InboundCallHistories.Single(x => x.Tag == tag);
        }

        public FilterController GetFilter(string tag)
        {
            return Filters.Single(x => x.Tag == tag);
        }
    }

    public class CallRef : Ref<BvCallEntity>, IAssert<BvCallEntity>
    {
        public InterviewController Interview { get; }

        public CallRef(InterviewController interview)
            : base(interview.Tag, interview.Id, interview.Context)
        {
            Interview = interview;
            Assert = new CallAsserter(this);
        }

        public override BvCallEntity Model => CallQueueService.GetCallAndNoLock(Interview.Survey.Id, Interview.Id);

        IAsserter<BvCallEntity> IAssert<BvCallEntity>.Assert => this.Assert;

        public CallAsserter Assert { get; set; }
    }

    public class CallCenterController : Ref<BvCallCenterEntity>
    {
        public CallCenterController(string tag, int id, TestDataContext context)
            : base(tag, id, context)
        {
        }

        public override BvCallCenterEntity Model => ServiceLocator.Resolve<ICallCenterRepository>().Get(Id);
    }

    public class InterviewController : Ref<BvInterviewWithOriginEntity>, IAssert<BvInterviewWithOriginEntity>
    {
        public SurveyController Survey { get; }
        public InterviewData Data { get; }

        private readonly ISurveyDatabase _sdb;

        public InterviewController(string tag, TestDataContext context, SurveyController survey, int id, ISurveyDatabase sdb, InterviewData data)
            : base(tag, id, context)
        {
            Survey = survey;
            Data = data;
            _sdb = sdb;
            Assert = new SingleAsserter<BvInterviewWithOriginEntity>(this);
        }

        public override BvInterviewWithOriginEntity Model => InterviewRepository.GetById(Survey.Id, Id);

        public void SetData(string data)
        {
            _sdb.SetInterviewData(Id, data);
        }

        public string GetData(string input)
        {
            return _sdb.GetInterviewData(Id, input);
        }

        public IAsserter<BvInterviewWithOriginEntity> Assert { get; set; }
    }

    public class StateGroupController : Ref<BvStateGroupEntity>
    {
        public StateGroupController(string tag, int id, TestDataContext context)
            : base(tag, id, context)
        {
        }

        public override BvStateGroupEntity Model => StateGroupRepository.GetById(Id);

        public void ChangeState(int its, Action<BvStateEntity> changer)
        {
            var state = StateRepository.GetById(Id, its);
            changer(state);
            StateRepository.Update(state);
        }
    }

    public class SurveyController : Ref<BvSurveyEntity>, IAssert<BvSurveyEntity>
    {
        public ISurveyDatabase Database { get; }

        private SurveyDataBuilder Builder { get; }

        public SurveyData Data { get; set; }

        public SurveyController(string tag, int id, TestDataContext context, SurveyData data, SurveyDataBuilder builder)
            : base(tag, id, context)
        {
            Database = builder.Database;
            Data = data;
            Builder = builder;
            Assert = new SingleAsserter<BvSurveyEntity>(this);
        }

        public void Launch()
        {
            SurveyDataBuilder.LaunchSurvey(Builder.Database, Data, false);
        }

        public override BvSurveyEntity Model => SurveyRepository.GetById(Id);

        public QuotaController GetQuota(string quotaName)
        {
            return new QuotaController(this, Data.Quotas.Single(x => x.Name == quotaName));
        }

        public void AddSample(SchedulingMode schedulingMode, params InterviewData[] interviews)
        {
            int batchId = Builder.Database.GetNewBatchId();
            foreach (var interview in interviews)
            {
                for (int index = 0; index < interview.Count; index++)
                {
                    int respId = Builder.Database.AddInterview(batchId, ((int)interview.ITS).ToString(), interview);
                    Builder.RegistryInterviewInContext(respId, interview);
                }
            }

            new BackendTools(IntegrationTestingFramework.Instance).AddSample(Builder.Database.ProjectId, batchId, (int)schedulingMode);
        }

        public void SetRespondentTableColumnValue(int[] respondentIds, string column, string value)
        {
            Builder.Database.SetRespondentTableColumnValue(respondentIds, column, value);
        }

        public void ProcessSample(SchedulingMode schedulingMode, SampleMode sampleMode, params InterviewData[] interviews)
        {
            int batchId = Builder.Database.GetNewBatchId();
            ProcessSample(schedulingMode, sampleMode, batchId, interviews);
        }

        public void ProcessSample(SchedulingMode schedulingMode, SampleMode sampleMode, int batchId, params InterviewData[] interviews)
        {
            var updateBatchid = batchId;

            if (sampleMode == SampleMode.Update)
            {
                updateBatchid = updateBatchid + 1;
            }

            foreach (var interview in interviews)
            {
                var dbInterviews = Builder.Context.GetInterviews(interview.Tag);
                foreach (var dbInterview in dbInterviews)
                {
                    Builder.Database.SetInterviewData(dbInterview.Id, interview.Data);
                    Builder.Database.SetBatchId(dbInterview.Id, batchId, updateBatchid);
                }
            }

            if (sampleMode == SampleMode.Add)
            {
                new BackendTools(IntegrationTestingFramework.Instance).ProcessSample(Builder.Database.ProjectId, batchId, (int)sampleMode, (int)schedulingMode);
            }
            else
            {
                new BackendTools(IntegrationTestingFramework.Instance).ProcessSample(Builder.Database.ProjectId, updateBatchid, (int)sampleMode, (int)schedulingMode);
            }
        }

        public IAsserter<BvSurveyEntity> Assert { get; }
    }

    public class QuotaController
    {
        public SurveyController Survey { get; }
        public QuotaData Data { get; }

        public QuotaController(SurveyController survey, QuotaData data)
        {
            Survey = survey;
            Data = data;
        }

        public void CloseCellById(int cellId)
        {
            Survey.Database.CloseCell(Data.Id, cellId);
            new ManagementService().OnQuotaCellsChanged(
                Survey.Database.ProjectId, Data.Id, new int[] { }, new[] { cellId }, new int[] { });
            BackendTools.ExecuteAllAsyncOperations();
        }

        public void CloseCellByIdOptimistically(int cellId)
        {
            Survey.Database.CloseCellOptimistically(Data.Id, cellId);
            new ManagementService().OnQuotaCellsChanged(
                Survey.Database.ProjectId, Data.Id, new int[] { }, new int[] { }, new[] { cellId });
            BackendTools.ExecuteAllAsyncOperations();
        }

        public void OpenCellById(int cellId)
        {
            OpenCellsById(cellId);
        }

        public void OpenCellsById(params int[] cellIds)
        {
            foreach (var cellId in cellIds)
            {
                Survey.Database.OpenCell(Data.Id, cellId);
            }

            new ManagementService().OnQuotaCellsChanged(Survey.Database.ProjectId, Data.Id, cellIds, new int[] { }, new int[] { });
            BackendTools.ExecuteAllAsyncOperations();
        }

        public void OnQuotaChanged()
        {
            new ManagementService().OnQuotaChanged(Survey.Database.ProjectId, Data.Id);
            BackendTools.ExecuteAllAsyncOperations();
        }

        public CellController GetCell(int cellId)
        {
            return new CellController(Survey, this, Data.Cells.Single(x => x.Id == cellId));
        }

        public void ChangeQuotaCellsStates(string projectId, int quotaId, int[] optimisticallyOpenedCellsIds = null, int[] optimisticallyClosedCellsIds = null,
            int[] pessimisticallyOpenedCellsIds = null, int[] pessimisticallyClosedCellsIds = null)
        {
            var quotaCellsCountersStates = new List<CatiQuotaCellCountersState>();
            if (optimisticallyClosedCellsIds != null)
            {
                quotaCellsCountersStates.AddRange(optimisticallyClosedCellsIds.Select(GetOptimisticallyClosedCellState));
            }
            if (optimisticallyOpenedCellsIds != null)
            {
                quotaCellsCountersStates.AddRange(optimisticallyOpenedCellsIds.Select(GetOptimisticallyOpenedCellState));
            }
            if (pessimisticallyOpenedCellsIds != null)
            {
                quotaCellsCountersStates.AddRange(pessimisticallyOpenedCellsIds.Select(GetPessimisticallyOpenedCellState));
            }
            if (pessimisticallyClosedCellsIds != null)
            {
                quotaCellsCountersStates.AddRange(pessimisticallyClosedCellsIds.Select(GetPessimisticallyClosedCellState));
            }

            UpdateQuotaCellsInDatabase(projectId, quotaId, quotaCellsCountersStates);

            new ManagementService().OnQuotaCellsStateChanged(projectId, quotaId,
                quotaCellsCountersStates);
            BackendTools.ExecuteAllAsyncOperations();
        }

        private void UpdateQuotaCellsInDatabase(string projectId, int quotaId, List<CatiQuotaCellCountersState> quotaCells)
        {
            var connection = IntegrationTestingFramework.Instance.GetConfirmitSqlServerConnectionString("survey_" + projectId);
            var db = new DatabaseEngine(connection);

            foreach (var cell in quotaCells)
            {
                db.ExecuteNonQuery($@"
                                    UPDATE quota_{quotaId} SET 
                                    counter = {cell.ActualCounters.Counter}, 
                                    limit = {cell.ActualCounters.Limit}, 
                                    live_counter = {cell.ActualCounters.LiveCounter}, 
                                    live_limit = {cell.ActualCounters.LiveLimit} 
                                    WHERE quotaid = {cell.CellId}");
            }
        }

        private static CatiQuotaCellCountersState GetOptimisticallyClosedCellState(int cellId)
        {
            return new CatiQuotaCellCountersState
            {
                CellId = cellId,
                ActualCounters = new CatiQuotaCellCounters
                {
                    Disabled = false,
                    IsOptimistic = true,
                    Counter = 1,
                    Limit = 3,
                    LiveCounter = 5,
                    LiveLimit = 5

                },
                OldCounters = new CatiQuotaCellCounters()
            };
        }

        private static CatiQuotaCellCountersState GetOptimisticallyOpenedCellState(int cellId)
        {
            return new CatiQuotaCellCountersState
            {
                CellId = cellId,
                ActualCounters = new CatiQuotaCellCounters
                {
                    Disabled = false,
                    IsOptimistic = true,
                    Counter = 1,
                    Limit = 5,
                    LiveCounter = 5,
                    LiveLimit = 7
                },
                OldCounters = new CatiQuotaCellCounters()
            };
        }

        private static CatiQuotaCellCountersState GetPessimisticallyOpenedCellState(int cellId)
        {
            return new CatiQuotaCellCountersState
            {
                CellId = cellId,
                ActualCounters = new CatiQuotaCellCounters
                {
                    Disabled = false,
                    IsOptimistic = true,
                    Counter = 1,
                    Limit = 5,
                    LiveCounter = 1,
                    LiveLimit = 5
                },
                OldCounters = new CatiQuotaCellCounters()
            };
        }

        private static CatiQuotaCellCountersState GetPessimisticallyClosedCellState(int cellId)
        {
            return new CatiQuotaCellCountersState
            {
                CellId = cellId,
                ActualCounters = new CatiQuotaCellCounters
                {
                    Disabled = false,
                    IsOptimistic = true,
                    Counter = 3,
                    Limit = 3,
                    LiveCounter = 0,
                    LiveLimit = 5
                },
                OldCounters = new CatiQuotaCellCounters()
            };
        }
    }

    public class CellController
    {
        public SurveyController Survey { get; }
        public QuotaController Quota { get; }
        public CellData Data { get; }

        public CellController(SurveyController survey, QuotaController quota, CellData data)
        {
            Survey = survey;
            Quota = quota;
            Data = data;
        }

        public void ChangeState(QuotaCellState state)
        {
            switch (state)
            {
                case QuotaCellState.PessimisticallyOpened:
                    Quota.OpenCellById(Data.Id);
                    break;
                case QuotaCellState.OptimisticallyClosed:
                    Quota.CloseCellByIdOptimistically(Data.Id);
                    break;
                case QuotaCellState.PessimisticallyClosed:
                    Quota.CloseCellById(Data.Id);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }

    public class PersonController : Ref<BvPersonEntity>, IResourceController
    {
        public PersonData Data { get; set; }

        public PersonController(string tag, int id, TestDataContext context, PersonData data)
            : base(tag, id, context)
        {
            Data = data;
        }

        public override BvPersonEntity Model => PersonRepository.GetById(Id);

        private ConsoleController _console;
        public ConsoleController Console => _console ?? (_console = ConsoleController.Create(this));
    }

    public class PersonGroupController : Ref<BvPersonGroupEntity>, IResourceController
    {
        public PersonGroupData Data { get; set; }

        public PersonGroupController(string tag, int id, TestDataContext context, PersonGroupData data)
            : base(tag, id, context)
        {
            Data = data;
        }

        public override BvPersonGroupEntity Model => PersonGroupRepository.GetById(Id);
    }

    public class CallGroupController : Ref<BvCallGroupEntity>
    {
        public CallGroupController(string tag, int id, TestDataContext context)
            : base(tag, id, context)
        {
        }

        public override BvCallGroupEntity Model => ServiceLocator.Resolve<ICallGroupRepository>().Get(Id);
    }

    public class FilterController : Ref<BvFiltersEntity>
    {
        public FilterController(string tag, int id, TestDataContext context)
            : base(tag, id, context)
        {
        }

        public override BvFiltersEntity Model => ServiceLocator.Resolve<IFilterRepository>().GetById(Id);
    }

    public class InboundCallHistoryController : Ref<BvInboundCallsHistoryEntity>, IResourceController
    {
        public InboundCallHistoryData Data { get; set; }

        public InboundCallHistoryController(string tag, int id, TestDataContext context, InboundCallHistoryData data)
            : base(tag, id, context)
        {
            Data = data;
        }

        public override BvInboundCallsHistoryEntity Model => ServiceLocator.Resolve<IInboundCallsHistoryRepository>().GetById(Id);
    }

    public class ExternalNumberController : Ref<BvExternalTransferTelephoneNumberEntity>, IResourceController
    {
        public ExternalNumberData Data { get; set; }

        public ExternalNumberController(string tag, int id, TestDataContext context, ExternalNumberData data)
            : base(tag, id, context)
        {
            Data = data;
        }

        public override BvExternalTransferTelephoneNumberEntity Model => ServiceLocator.Resolve<IExternalTransferTelephoneNumberRepository>().TryGetById(Id);
    }

    public interface IResourceController
    {
        int Id { get; }
        string Tag { get; }
    }

    public class ScriptController : Ref<BvScheduleEntity>
    {
        public ScriptData Data { get; }

        public ScriptController(string tag, int id, TestDataContext context, ScriptData data)
            : base(tag, id, context)
        {
            Data = data;
        }

        public override BvScheduleEntity Model => ScheduleRepository.GetById(Id);
    }

    public class SupervisorController : Ref<string>
    {
        public string Name { get; }
        public SupervisorController(string tag, string name, TestDataContext context)
            : base(tag, 0, context)
        {
            Name = name;
        }

        public override string Model => Name;
    }

    public abstract class Ref<TModel> : IRef, IModelProvider<TModel>
    {
        protected Ref(string tag, int id, TestDataContext context)
        {
            Tag = tag;
            Id = id;
            Context = context;
        }
        public int Id { get; protected set; }
        public string Tag { get; protected set; }
        public TestDataContext Context { get; }
        public abstract TModel Model { get; }

        object IRef.Model => Model;

        public override string ToString()
        {
            return $"{Tag}:Id={Id}";
        }
    }

    public interface IModelProvider<out TModel>
    {
        TModel Model { get; }
    }

    public interface IRef
    {
        int Id { get; }
        string Tag { get; }
        object Model { get; }
    }
}
