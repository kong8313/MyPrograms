using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ChangeDialModeOfInterviews
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
                    return new ChangeDialModeOfSelectedInterviewsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.Filtered:
                    return new ChangeDialModeOfFilteredInterviewsEvent(parameters.SurveyId, surveyName, parameters, entity);
                default:
                    throw new NotImplementedException(String.Format("Activity event doesn't specified for Change dial mode of interviews operation with {0} batch type.", parameters.BatchParameters.Type));
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
                new DatabaseEngine().ExecuteNonQuery(
                      @"DECLARE @its TABLE ( interviewid INT, DialingMode TINYINT, its TINYINT)
                        UPDATE [BvInterview] 
                        SET [DialingMode] = @DialingMode 
                        OUTPUT inserted.Id, inserted.DialingMode, inserted.TransientState INTO @its
                        FROM [BvTransferArrays] ta 
                        WHERE  BvInterview.ID = ta.ItemID AND ta.BatchID = @BatchID AND BvInterview.SurveySID = @SurveyId 

                        INSERT INTO BvCallhistoryEx
                        SELECT GETUTCDATE(), c.ApptID, c.ShiftTypeID, i.interviewId, @SurveyID, i.its, i.Dialingmode, c.CallState, c.[Priority], c.TimeInShift, c.ExpireTime, c.ExplicitSid, c.ExplicitType, c.CellId, 
                                @OperationId, @OperationType, @CallCenterId, c.DialTypeId
                        FROM @its i
                        LEFT JOIN BvSvySchedule c ON c.InterviewID = i.InterviewID AND c.SurveySID = @SurveyId ",
                      CommandType.Text,
                      new SqlParameter("@DialingMode", parameters.DialingMode.HasValue ? (int)parameters.DialingMode.Value : 0),
                      new SqlParameter("@SurveyId", parameters.SurveyId),
                      new SqlParameter("@BatchID", subBatch.Id),
                      new SqlParameter("@OperationId", entity.Id),
                      new SqlParameter("@OperationType", OperationType.ChangeDiallingMode),
                      new SqlParameter("@CallCenterId", entity.CallCenterId)
                      );

                transactionScope.Commit();
            }
        }
    }
}
