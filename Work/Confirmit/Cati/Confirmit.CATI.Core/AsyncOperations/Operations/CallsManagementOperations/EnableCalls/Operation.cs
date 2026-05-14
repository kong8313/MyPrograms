using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.EnableCalls
{
    public class Operation : CallsManagementBatchedOperation<Descriptor, Parameters>
    {
        private readonly ISystemSettings _systemSettings;
        private readonly ICallsManagementService _callsManagementService;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IDialerOperation _dialerOperation;

        public Operation(
            ISystemSettings systemSettings,
            ICallsManagementService callsManagementService,
            ICallsManagementBatchedOperationBase batchedOperationBase,
            ISurveyRepository surveyRepository,
            IDialerOperation dialerOperation)
            : base(batchedOperationBase)
        {
            _systemSettings = systemSettings;
            _callsManagementService = callsManagementService;
            _surveyRepository = surveyRepository;
            _dialerOperation = dialerOperation;
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
                    return parameters.EnablingState ? (BaseAsyncOperationManagementActivityEvent<Parameters>)new EnableSelectedCallsEvent(parameters.SurveyId, surveyName, parameters, entity) :
                                                        new DisableSelectedCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.Filtered:
                    return parameters.EnablingState ? (BaseAsyncOperationManagementActivityEvent<Parameters>)new EnableFilteredCallsEvent(parameters.SurveyId, surveyName, parameters, entity) :
                                                        new DisableFilteredCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.FilteredByCells:
                case BatchType.FilteredByClosedQuotaCell:
                case BatchType.FilteredByOpenedQuotaCell:
                    return parameters.EnablingState ? (BaseAsyncOperationManagementActivityEvent<Parameters>)new EnableFilteredByCellsCallsEvent(parameters.SurveyId, surveyName, parameters, entity) :
                                                        new DisableFilteredByCellsCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                default:
                    throw new NotImplementedException(String.Format("Activity event doesn't specified for Move calls operation with {0} batch type.", parameters.BatchParameters.Type));
            }

        }

        public override void ProcessSubBatch(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, Parameters parameters, BvAsyncOperationQueueEntity entity)
        {
            using (var transactionScope = new DatabaseTransactionScope(
                Descriptor.Name,
                DeadlockPriority.Supervisor))
            {
                if (parameters.EnablingState)
                {
                    operation.WriteContextInfo(entity, OperationType.EnableCalls);
                    EnableCalls(subBatch, parameters);
                }
                else
                {
                    operation.WriteContextInfo(entity, (parameters.IsFcdOperation) ? OperationType.DisableByFcd : OperationType.DisableCalls);
                    DisableCalls(subBatch, parameters);
                }

                transactionScope.Commit();
            }
        }

        private void DisableCalls(IDatabaseBatch subBatch, Parameters parameters)
        {
            List<CallInfo> callsToFlush = null;
            var survey = _surveyRepository.GetById(parameters.SurveyId);
            // Note: currenly we support the only dialer in predictieve, so we get that only dialer and flush call on it
            // Or either we could FlushNumbers on all dialers?
            // Or... we currently can't define which dialer actually owns the calls
            var dialerEntity = BvDialersAdapter.GetAll().FirstOrDefault();

            //TODO CODI changes: for now we get the isRecording flag from the survey properties, later we would add isRecording property to each call?
            var isRecording = survey.RecWholeInt > 0;

            if (dialerEntity != null)
            {
                callsToFlush = _callsManagementService.GetCallsToFlushOnDialer(survey.SID, subBatch.Id, isRecording);
            }

            BvSpCall_EnableAdapter.ExecuteNonQuery(parameters.SurveyId, subBatch.Id, parameters.IsFcdOperation, false);

            _dialerOperation.FlushCallsIfNeeded(survey, callsToFlush);
        }

        private void EnableCalls(IDatabaseBatch subBatch, Parameters parameters)
        {
            BvSpCall_EnableAdapter.ExecuteNonQuery(parameters.SurveyId, subBatch.Id, parameters.IsFcdOperation, true);
        }
    }
}
