using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.EditCalls
{
    public class Operation : CallsManagementBatchedOperation<Descriptor, Parameters>
    {
        private readonly ISystemSettings _systemSettings;
        private readonly ICallsManagementService _callsManagementService;

        public Operation(
            ISystemSettings systemSettings,
            ICallsManagementBatchedOperationBase batchedOperationBase,
            ICallsManagementService callsManagementService)
            : base(batchedOperationBase)
        {
            _systemSettings = systemSettings;
            _callsManagementService = callsManagementService;
        }

        public override int PortionSize => _systemSettings.AsyncOperation.ActivatePortionSize;

        public override BaseAsyncOperationManagementActivityEvent<Parameters> CreateEvent(BvAsyncOperationQueueEntity entity, Parameters parameters)
        {
            var surveyName = SurveyRepository.GetById(parameters.SurveyId).Name;
            switch (parameters.BatchParameters.Type)
            {
                case BatchType.Selected:
                    return new EditSelectedCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.Filtered:
                    return new EditFilteredCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                default:
                    throw new NotImplementedException(
                        $"Activity event doesn't specified for Edit calls operation with {parameters.BatchParameters.Type} batch type.");
            }
        }

        public override void ProcessSubBatch(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, Parameters parameters, BvAsyncOperationQueueEntity entity)
        {
            if (!parameters.TimeToCall.HasValue && !parameters.TimeToExpire.HasValue && !parameters.CallState.HasValue && !parameters.CallPriority.HasValue &&
                !parameters.ShiftType.HasValue && !parameters.ExtendedStatus.HasValue && !parameters.DialingMode.HasValue)
            {
                throw new ArgumentException("At least one parameter should be changed");
            }

            using (var transaction = new DatabaseTransactionScope(Descriptor.Name, DeadlockPriority.Supervisor))
            {
                operation.WriteContextInfo(entity, OperationType.EditCalls, parameters.ExtendedStatus ?? 0, (DialingMode?)parameters.DialingMode ?? 0);
                
                _callsManagementService.Edit(
                     parameters.SurveyId,
                     subBatch.Id,
                     parameters.TimeToCall,
                     parameters.TimeToExpire,
                     parameters.CallState,
                     parameters.CallPriority,
                     parameters.ShiftType,
                     parameters.ExtendedStatus,
                     parameters.DialingMode);

                transaction.Commit();
            }
        }
    }
}
