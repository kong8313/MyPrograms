using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    internal class FullSchedulingSampleRecordProcessor : ISampleRecordProcessor
    {
        private readonly IInterviewRepository _interviewRepository;

        public SampleContext Context { get; set; }
        public IFCDSettings FcdSettings { get; set; }

        public FullSchedulingSampleRecordProcessor(SampleContext context, IFCDSettings fcdSettings)
        {
            Context = context;
            FcdSettings = fcdSettings;
            _interviewRepository = ServiceLocator.Resolve<IInterviewRepository>();
        }

        public void Process(ISampleRecordStorage storage, SampleProcessingStateContainer stateContainer, RespondentRecord record, BvInterviewWithOriginEntity interview, ProcessSampleMode processSampleMode)
        {
            bool runSchedulingScript = true;

            if (record.IsTelephoneInBlackList)
            {
                interview.TransientState = (int)CallOutcome.Blacklist;
                runSchedulingScript = false;
            }
            else if (IsInterviewFilteredByFcd(record, interview) && FcdSettings.AlgorithmType == FcdAlgorithmType.DeleteCalls)
            {
                interview.TransientState = (int)CallOutcome.FilteredByCallDelivery;
                runSchedulingScript = false;
            }

            var options = new SchedulingScriptExecutionOptions()
            {
                ExecutionReason = SchedulingScriptExecutionReason.AddedBySample,
                BatchID = Context.BatchId,
                ProcessSampleMode = processSampleMode,
                SchedulingScriptNotificatorExceptions = Context.SchedulingScriptNotificatorExceptions,
                IsExecuteSchedulingScript = runSchedulingScript,
                IsLogToHistory = false,
                opType = OperationType.SampleAddFullScheduling
            };

            _interviewRepository.Insert(interview, options);
        }

        public void OnCompleted()
        {
        }

        private bool IsInterviewFilteredByFcd(RespondentRecord record, BvInterviewEntity interview)
        {
            return record.IsClosedCell && !Context.IgnoredItsByFcd.Contains(interview.TransientState);
        }
    }
}
