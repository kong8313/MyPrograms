using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.DeleteCalls
{
    public class Operation : CallsManagementBatchedOperation<Descriptor, Parameters>
    {
        private readonly ISystemSettings _systemSettings;
        private readonly ICallsManagementService _callsManagementService;
        private readonly IDialerOperation _dialerOperation;

        public Operation(
            ISystemSettings systemSettings,
            ICallsManagementService callsManagementService,
            ICallsManagementBatchedOperationBase batchedOperationBase,
            IDialerOperation dialerOperation)
            : base(batchedOperationBase)
        {
            _systemSettings = systemSettings;
            _callsManagementService = callsManagementService;
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
                    return new DeleteSelectedCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.Filtered:
                    return new DeleteFilteredCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.FilteredByClosedQuotaCell:
                    return new DeleteFilteredByClosedQuotaCellEvent(parameters.SurveyId, surveyName, parameters, entity);
                default:
                    throw new NotImplementedException(String.Format("Activity event doesn't specified for Delete calls operation with {0} batch type.", parameters.BatchParameters.Type));
            }

        }

        private OperationType GetOperationTypeByIts(int? newIts)
        {
            if (newIts == (int) CallOutcome.FilteredByCallDelivery)
                return OperationType.DeleteCallsByFcd;
            
            if (newIts == (int) CallOutcome.Blacklist)
                return OperationType.DeleteCallByBlacklistInAddSample;
            
            return OperationType.DeleteCalls;
        }

        public override void ProcessSubBatch(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, Parameters parameters, BvAsyncOperationQueueEntity entity)
        {
            using (var transactionScope = new DatabaseTransactionScope(
                Descriptor.Name,
                DeadlockPriority.Supervisor))
            {

                // We shouldn't use [linked server] prefix in queries, if both CF and BE databases are placed on same SQL server.
                // If CF database is placed on 'local' SQL server, then CfDbSchemaPath is like [DbName].[SchemaName],
                // otherwise CfDbSchemaPath is like [LinkedServerName].[DbName].[SchemaName]
                // Note: we use '[' marker for check CfDbSchemaPath instead of '.' marker, because using '.' marker isn't safe.

                // Note: we update response_control table through remote call of sp_excutesql for
                // excution of update query as 'local' on remote server. Otherwise we have huge duration of query excution, because  
                // remote response_control table has trigger which also update response_control

                var survey = SurveyRepository.GetById(parameters.SurveyId);

                List<CallInfo> callsToFlush = null;
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

                operation.WriteContextInfo(entity,
                    GetOperationTypeByIts(parameters.NewITS),
                    parameters.NewITS ?? 0);

                _callsManagementService.RemoveFilteredCalls(survey.SID, subBatch.Id, parameters.NewITS);

                _dialerOperation.FlushCallsIfNeeded(survey, callsToFlush);

                transactionScope.Commit();
            }
        }
    }
}
