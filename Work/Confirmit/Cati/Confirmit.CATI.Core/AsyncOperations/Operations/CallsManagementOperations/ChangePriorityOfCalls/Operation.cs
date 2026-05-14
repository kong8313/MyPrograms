using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ChangePriorityOfCalls
{
    public class Operation : ICallsManagementBatchedOperation
    {
        private readonly ISystemSettings _systemSettings;
        private readonly ICallsManagementBatchedOperationBase _batchedOperationBase;

        public Operation(
            ISystemSettings systemSettings,
            ICallsManagementBatchedOperationBase batchedOperationBase)
        {
            _systemSettings = systemSettings;
            _batchedOperationBase = batchedOperationBase;
        }

        public IOperationDescriptor Descriptor
        {
            get { return new Descriptor(); }
        }

        private Parameters DeserializeParameters(string parameters)
        {
            var serializer = new XmlSerializer(typeof(Parameters));

            using (var reader = new StringReader(parameters))
            {
                return (Parameters)serializer.Deserialize(reader);
            }
        }

        private BaseAsyncOperationManagementActivityEvent<Parameters> CreateEvent(BvAsyncOperationQueueEntity entity, Parameters parameters)
        {
            var surveyName = SurveyRepository.GetById(parameters.SurveyId).Name;
            switch (parameters.BatchParameters.Type)
            {
                case BatchType.Selected:
                    return new ChangePriorityOfSelectedCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.Filtered:
                    return new ChangePriorityOfFilteredCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.FilteredByCells:
                case BatchType.FilteredByMultipleCells:
                    return new ChangePriorityOfFilteredByCellsCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                default:
                    throw new NotImplementedException(String.Format("Activity event doesn't specified for change priority of calls operation with {0} batch type.", parameters.BatchParameters.Type));
            }
        }

        public AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity, string serializedParameters, IAsyncOperationProgressLogger progressLogger, CancellationToken cancellationToken)
        {
            var parameters = DeserializeParameters(serializedParameters);

            var evt = CreateEvent(entity, parameters);

            var result = _batchedOperationBase.Execute(
                this,
                parameters.BatchParameters,
                progressLogger,
                entity,
                parameters.SurveyId,
                _systemSettings.AsyncOperation.MovePortionSize,
                parameters, cancellationToken);

            if (evt.Details != null && result != null)
            {
                evt.Details.Result = result.ToString();
            }
            
            evt.Save();

            return result;
        }

        public void ProcessSubBatch(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, object state, BvAsyncOperationQueueEntity entity)
        {
            var parameters = (Parameters)state;

            using (var transactionScope = new DatabaseTransactionScope(Descriptor.Name, DeadlockPriority.Supervisor))
            {
                operation.WriteContextInfo(entity, OperationType.ChangePriorityOfCalls);

                BvSpCall_ChangePriorityAdapter.ExecuteNonQuery(parameters.SurveyId, parameters.Priority, subBatch.Id);

                transactionScope.Commit();
            }
        }
    }
}
