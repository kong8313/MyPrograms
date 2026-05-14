using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.AssignCalls
{
    public class Operation : CallsManagementBatchedOperation<Descriptor, Parameters>
    {
        private readonly ISystemSettings _systemSettings;
        private readonly IAssignmentService _assignmentService;

        public Operation(
            ISystemSettings systemSettings,
            ICallsManagementBatchedOperationBase batchedOperationBase,
            IAssignmentService assignmentService) : base(batchedOperationBase)
        {
            _systemSettings = systemSettings;
            _assignmentService = assignmentService;
        }

        public override int PortionSize
        {
            get { return _systemSettings.AsyncOperation.MovePortionSize; }
        }
        
        public override BaseAsyncOperationManagementActivityEvent<Parameters> CreateEvent(BvAsyncOperationQueueEntity entity, Parameters parameters)
        {
            var surveyName = SurveyRepository.GetById(parameters.SurveyId).Name;
            switch (parameters.BatchParameters.Type)
            {
                case BatchType.Selected:
                    return new AssignSelectedCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.Filtered:
                    return new AssignFilteredCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                default:
                    throw new NotImplementedException(String.Format("Activity event doesn't specified for Move calls operation with {0} batch type.", parameters.BatchParameters.Type));
            }
        }



        public override void ProcessSubBatch(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, Parameters parameters, BvAsyncOperationQueueEntity entity)
        {
            var assignmentResourceId = _assignmentService.GetAssignmentResourceId(parameters.ResourceIds);

            if( assignmentResourceId == 0)
                throw new InvalidOperationException("Assignment isn't specified.");

            using (var transactionScope = new DatabaseTransactionScope(Descriptor.Name, DeadlockPriority.Supervisor))
            {

                operation.WriteContextInfo(entity, OperationType.AssignCalls);
 
                BvSpAssignment_Insert2Adapter.ExecuteNonQuery(parameters.SurveyId, assignmentResourceId, subBatch.Id);

                transactionScope.Commit();
            }
        }
    }
}
