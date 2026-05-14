using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ActivateCalls
{
    public class Operation : CallsManagementBatchedOperation<Descriptor, Parameters>
    {
        private readonly ISystemSettings _systemSettings;
        private readonly ITimezoneService _timezoneService;
        private readonly IAssignmentService _assignmentService;
        private readonly ICallsManagementService _callsManagementService;

        public Operation(
            ISystemSettings systemSettings,
            ICallsManagementBatchedOperationBase batchedOperationBase,
            ITimezoneService timezoneService,
            IAssignmentService assignmentService,
            ICallsManagementService callsManagementService)
            : base(batchedOperationBase)
        {
            _systemSettings = systemSettings;
            _timezoneService = timezoneService;
            _assignmentService = assignmentService;
            _callsManagementService = callsManagementService;
        }

        public override int PortionSize => _systemSettings.AsyncOperation.ActivatePortionSize;

        public override BaseAsyncOperationManagementActivityEvent<Parameters> CreateEvent(BvAsyncOperationQueueEntity entity, Parameters parameters)
        {
            var surveyName = SurveyRepository.GetById(parameters.SurveyId).Name;
            switch (parameters.BatchParameters.Type)
            {
                case BatchType.Selected:
                    return new ActivateSelectedCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.Filtered:
                    return new ActivateFilteredCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.FilteredByCells:
                case BatchType.FilteredByMultipleCells:
                    return new ActivateFilteredByCellsCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                default:
                    throw new NotImplementedException(
                        $"Activity event doesn't specified for Activate calls operation with {parameters.BatchParameters.Type} batch type.");
            }
        }

        public override void ProcessSubBatch(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, Parameters parameters, BvAsyncOperationQueueEntity entity)
        {
            var assignmentResourceId = _assignmentService.GetAssignmentResourceId(parameters.ResourceIds);

            using (var transaction = new DatabaseTransactionScope(Descriptor.Name, DeadlockPriority.Supervisor))
            {
                operation.WriteContextInfo(entity, OperationType.ActivateCalls, parameters.ITS ?? 0);

                _callsManagementService.Activate(
                     parameters.SurveyId,
                     (int)GetModeForActivate(parameters.CallState),
                     subBatch.Id,
                     parameters.Priority,
                     assignmentResourceId,
                     parameters.ShiftTypeId,
                     parameters.TimeToCall,
                     parameters.EnableDisabledCalls,
                     _timezoneService.GetDefaultCallCenterTimezoneId(),
                     parameters.ITS);

                transaction.Commit();
            }
        }

        private FilterGenerateMode GetModeForActivate(CallStates callState)
        {
            switch (callState)
            {
                case CallStates.Scheduled:
                case CallStates.HighPriority:
                    return FilterGenerateMode.ScheduledInterviewIds;
                case CallStates.Suspended:
                    return FilterGenerateMode.SuspendedInterviewIds;
                case CallStates.All:
                    return FilterGenerateMode.AllInterviewIds;
            }

            throw new ArgumentException($"Unexpected call state: {callState}");
        }

    }
}
