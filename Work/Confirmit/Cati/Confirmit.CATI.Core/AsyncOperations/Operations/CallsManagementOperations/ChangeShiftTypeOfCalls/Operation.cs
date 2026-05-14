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
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ChangeShiftTypeOfCalls
{
    public class Operation : ICallsManagementBatchedOperation
    {
        private readonly ISystemSettings _systemSettings;
        private readonly ICallsManagementBatchedOperationBase _batchedOperationBase;
        private readonly ITimezoneService _timezoneService;

        public Operation(
            ISystemSettings systemSettings,
            ICallsManagementBatchedOperationBase batchedOperationBase,
            ITimezoneService timezoneService)
        {
            _systemSettings = systemSettings;
            _batchedOperationBase = batchedOperationBase;
            _timezoneService = timezoneService;
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
                    return new ChangeShiftTypeOfSelectedCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.Filtered:
                    return new ChangeShiftTypeOfFilteredCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                default:
                    throw new NotImplementedException(String.Format("Activity event doesn't specified for change shift type of calls operation with {0} batch type.", parameters.BatchParameters.Type));
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

                operation.WriteContextInfo(entity, OperationType.ChangeShiftTypesOfCall);

                BvSpCall_ChangeShiftTypeAdapter.ExecuteNonQuery(parameters.SurveyId, parameters.ShiftTypeID, subBatch.Id, _timezoneService.GetDefaultCallCenterTimezoneId());

                transactionScope.Commit();
            }
        }
    }
}
