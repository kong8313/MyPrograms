using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.MoveCalls
{
    public class Operation : CallsManagementBatchedOperation<Descriptor, Parameters>
    {
        private readonly IAsyncOperationSettings _asyncOperationSettings;
        private readonly ICallsManagementService _callsManagementService;

        public Operation(
            IAsyncOperationSettings asyncOperationSettings,
            ICallsManagementService callsManagementService,
            ICallsManagementBatchedOperationBase batchedOperationBase)
            : base(batchedOperationBase)
        {
            _asyncOperationSettings = asyncOperationSettings;
            _callsManagementService = callsManagementService;
        }

        public override int PortionSize
        {
            get { return _asyncOperationSettings.MovePortionSize; }
        }

        public override BaseAsyncOperationManagementActivityEvent<Parameters> CreateEvent(BvAsyncOperationQueueEntity entity, Parameters parameters)
        {
            var surveyName = SurveyRepository.GetById(parameters.SurveyId).Name;
            switch (parameters.BatchParameters.Type)
            {
                case BatchType.Selected:
                    return new MoveSelectedCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.Filtered:
                    return new MoveFilteredCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                default:
                    throw new NotImplementedException(String.Format("Activity event doesn't specified for Move calls operation with {0} batch type.", parameters.BatchParameters.Type));
            }
        }

        public override void ProcessSubBatch(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, Parameters parameters, BvAsyncOperationQueueEntity entity)
        {
            using (var transactionScope = new DatabaseTransactionScope(Descriptor.Name, DeadlockPriority.Supervisor))
            {

                operation.WriteContextInfo(entity, OperationType.MoveCallsToIts, parameters.StateId);

                _callsManagementService.MoveToIts(parameters.SurveyId, subBatch.Id, parameters.StateId);

                transactionScope.Commit();
            }
        }
    }
}
