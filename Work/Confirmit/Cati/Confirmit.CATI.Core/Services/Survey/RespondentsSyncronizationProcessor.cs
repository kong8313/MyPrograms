using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using ConfirmitDialerInterface;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Confirmit.CATI.Core.Services.Survey
{
    public class RespondentsRespondentsSynchronizationProcessor : IRespondentsSynchronizationProcessor
    {
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IReplicationService _replicationService;
        private readonly ISampleDataStorageRepository _sampleDataStorageRepository;
        private readonly IRespondentBatchObtainer _respondentBatchObtainer;
        private readonly IInterviewQuotaCellService _interviewQuotaCellService;
        private readonly IInterviewService _interviewService;
        
        public RespondentRecord[] Records { get; private set; }

        public RespondentsRespondentsSynchronizationProcessor(
            ISampleDataStorageRepository sampleDataStorageRepository,
            IReplicationService replicationService,
            IDatabaseLockTimeouts databaseLockTimeouts,
            IInterviewRepository interviewRepository,
            IRespondentBatchObtainer respondentBatchObtainer,
            IInterviewQuotaCellService interviewQuotaCellService,
            IInterviewService interviewService)
        {
            _sampleDataStorageRepository = sampleDataStorageRepository;
            _replicationService = replicationService;
            _databaseLockTimeouts = databaseLockTimeouts;
            _interviewRepository = interviewRepository;
            _respondentBatchObtainer = respondentBatchObtainer;
            _interviewQuotaCellService = interviewQuotaCellService;
            _interviewService = interviewService;
        }

        public void SynchronizeRespondents(RespondentsSynchronizationContext context, CancellationToken cancellationToken)
        {
            using (new EventDetailsScope(context.EventDetails))
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Process(context);

                    if (Records == null || Records.Length < context.PartitionSize)
                    {
                        break;
                    }
                }

                _replicationService.RereadSurveyReplicatedData(context.Survey.SID, "Respondents synchronization", cancellationToken);

                var respondentIds = _interviewService.GetInterviewIdsWithoutRespondents(context.Survey.SID);
                if (respondentIds != null && respondentIds.Length > 0)
                {
                    EventDetailsScope.Current.AddTiming("DeleteRespondents.Begin");
                    using (
                        var dbLock = ExclusiveDatabaseLock.CreateLock(DatabaseLockTimeoutsAndRecourceNames.GetFcdResourceName(context.Survey.SID), "SynchronizationProc", _databaseLockTimeouts.DefaultLockTimeoutInMs))
                    {
                        dbLock.EnterLock();
                        _interviewService.DeleteRespondents(context.Survey.SID, respondentIds, cancellationToken);

                        context.DeletedRecords += respondentIds.Length;
                    }
                }

                _interviewQuotaCellService.Populate(context.Survey.SID, cancellationToken);
            }
        }

        private void Process(RespondentsSynchronizationContext context)
        {
            using (
                var dbLock = ExclusiveDatabaseLock.CreateLock(DatabaseLockTimeoutsAndRecourceNames.GetFcdResourceName(context.Survey.SID), "SynchronizationProc", _databaseLockTimeouts.DefaultLockTimeoutInMs))
            {
                dbLock.EnterLock();

                Records = _respondentBatchObtainer.GetRespondentsForSynchronization(context.Survey, context.PartitionSize);

                if (Records.Length <= 0)
                    return;

                ProcessRecords(context, Records);

                context.AddedRecords += Records.Length;
            }
        }

        internal void ProcessRecords(RespondentsSynchronizationContext context, IEnumerable<RespondentRecord> batchPartition)
        {
            using (var storage = _sampleDataStorageRepository.Create(context.Survey, context.OperationId))
            {
                foreach (var record in batchPartition)
                {
                    record.TimeZoneId = context.TimeZoneReolver.Resolve(record.TimeZoneId);

                    var interview = InterviewService.GetInterviewFromRespondentRecord(context.Survey.SID, 0, record);

                    ProcessRecord(context, record, interview, storage);

                    storage.SaveCurrentRecord();
                }

                storage.Commit(context.EventDetails);
            }
        }

        internal void ProcessRecord(RespondentsSynchronizationContext context, RespondentRecord record, BvInterviewWithOriginEntity interview, ISampleDataStorage sampleStorage = null)
        {
            SetTransientState(context, record, interview);

            var options = new SchedulingScriptExecutionOptions { IsExecuteSchedulingScript = false, IsLogToHistory = false };

            _interviewRepository.Insert(interview, options, sampleStorage);
        }

        private void SetTransientState(RespondentsSynchronizationContext context, RespondentRecord record, BvInterviewWithOriginEntity interview)
        {
            interview.TransientState = context.ExtendedStatus;

            if (record.IsTelephoneInBlackList)
            {
                interview.TransientState = (int)CallOutcome.Blacklist;
            }
        }
    }
}