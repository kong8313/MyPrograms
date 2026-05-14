using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.UpdateFcdStatusOfCalls
{
    public class Operation : CallsManagementBatchedOperation<Descriptor, Parameters>
    {
        private readonly ISystemSettings _systemSettings;
        private readonly ISurveyRepository _surveyRepository;
        private readonly ICallsManagementService _callsManagementService;
        private readonly IDialerOperation _dialerOperation;

        public Operation(ISystemSettings systemSettings,
            ISurveyRepository surveyRepository,
            ICallsManagementService callsManagementService,
            ICallsManagementBatchedOperationBase batchedOperationBase,
            IDialerOperation dialerOperation)
            : base(batchedOperationBase)
        {
            _systemSettings = systemSettings;
            _surveyRepository = surveyRepository;
            _callsManagementService = callsManagementService;
            _dialerOperation = dialerOperation;
        }

        public override int PortionSize
        {
            get { return _systemSettings.AsyncOperation.MovePortionSize; }
        }

        public override BaseAsyncOperationManagementActivityEvent<Parameters> CreateEvent(BvAsyncOperationQueueEntity entity, Parameters parameters)
        {
            var surveyName = _surveyRepository.GetById(parameters.SurveyId).Name;
            return new UpdateFcdStatusOfCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
        }

        public override void ProcessSubBatch(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, Parameters parameters, BvAsyncOperationQueueEntity entity)
        {
            var survey = _surveyRepository.GetById(parameters.SurveyId);
            var isRecording = survey.RecWholeInt > 0;

            var contextInfoSql = ContextInfoService.GetContextInfoSql(entity.Id, OperationType.SynchronizeEnableDisableCallState, entity.CallCenterId);

            string query =
            $@"{contextInfoSql}
            CREATE TABLE #ClosedInterviews
            (
                Id INT
            );
            INSERT INTO #ClosedInterviews
                SELECT DISTINCT sched.InterviewId AS Id
                FROM BvSvySchedule AS sched
                INNER JOIN BvTransferArrays AS arr
                    ON arr.BatchId = @BatchId AND sched.SurveySID = @SurveyId AND sched.InterviewId = arr.ItemId
                INNER JOIN BvInterviewQuotaCell AS icell 
                    ON icell.SurveyId = sched.SurveySID AND icell.InterviewId = sched.InterviewId
                INNER JOIN BvSurveyQuotaCell AS qcell
                    ON icell.SurveyID = qcell.SurveyID AND icell.QuotaID = qcell.QuotaID AND icell.CellID = qcell.CellID AND qcell.IsOpen = 0
           

            CREATE TABLE #ChangedCallStates
            (
                CallId INT,
                ExplicitSID INT,
                SurveySID INT,
                DiallingMode TINYINT,
                InterviewID INT,
                TelephoneNumber NVARCHAR(256),
                ExtensionNumber NVARCHAR(256),
                TimeInShift DATETIME,
                OldState INT,
                NewState INT
            )
            ;WITH data as 
            (
                SELECT sched.Id as CallId, sched.ExplicitSID, sched.SurveySID, sched.CallState, interview.DialingMode, sched.InterviewId, interview.TelephoneNumber, interview.ExtensionNumber, sched.TimeInShift, 
                CASE WHEN iclosed.Id is not NULL AND state.FcdAction = 0 THEN 1 ELSE 2 END as newCallState
                FROM BvSvySchedule sched
                INNER JOIN BvTransferArrays arr
                    ON arr.BatchId = @BatchId AND sched.SurveySID = @SurveyId AND sched.InterviewId = arr.ItemId 
                INNER JOIN BvInterview interview 
                    ON sched.SurveySID = interview.SurveySID AND sched.InterviewId = interview.ID
                LEFT JOIN BvState state 
                    ON interview.TransientState = state.StateId AND state.StateGroupId = @StateGroupId
                LEFT JOIN #ClosedInterviews AS iclosed 
                    ON iclosed.Id = sched.InterviewId
                WHERE CallState IN ( -2, 1, 2)
            )
            UPDATE data SET CallState = newCallState 
                OUTPUT deleted.CallId, deleted.ExplicitSID, deleted.SurveySID, deleted.DialingMode, 
						deleted.InterviewId, deleted.TelephoneNumber, deleted.ExtensionNumber,
                    deleted.TimeInShift, deleted.CallState, inserted.CallState 
                    INTO #ChangedCallStates
                WHERE CASE WHEN CallState = -2 THEN 2 ELSE CallState END <> newCallState

            SELECT CallId as ID, ExplicitSID, SurveySID, DiallingMode, InterviewID, TelephoneNumber, ExtensionNumber, TimeInShift, 0 as GroupId 
				FROM #ChangedCallStates 
					WHERE OldState = -2 AND NewState = 1";
            
            List<CallInfo> callsToFlush;

            using (var reader = new DatabaseEngine().ExecuteReaderInNewConnection(query, CommandType.Text,
                new SqlParameter("@BatchId", subBatch.Id),
                new SqlParameter("@SurveyId", parameters.SurveyId),
                new SqlParameter("@StateGroupId", survey.StateGroupID)))
            {
                callsToFlush = _callsManagementService.ReadFlushedCallInfos(isRecording, reader);
            }

            var dialerEntity = BvDialersAdapter.GetAll().FirstOrDefault();

            if (dialerEntity != null)
            {
                _dialerOperation.FlushCallsIfNeeded(survey, callsToFlush);
            }
        }
    }
}