using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly ISurveyDatabaseService _surveyDatabaseService;

        public HistoryRepository(ISurveyDatabaseService surveyDatabaseService)
        {
            _surveyDatabaseService = surveyDatabaseService;
        }

        public int Insert(BvHistoryEntity history)
        {
            if (history.InterviewId != null && history.InterviewId != 0 && history.CallAttemptNumber == null)
            {
                history.CallAttemptNumber = _surveyDatabaseService.GetCallAttemptCount(history.SurveyId, history.InterviewId.Value);
            }

            return BvHistoryAdapter.InsertWithReturnIdentityValue(history);
        }

        public void Delete(int id)
        {
            var entity = BvHistoryAdapter.GetByCondition("[ID]=@id\r\n", new SqlParameter("@id", id)).FirstOrDefault();

            if (entity == null)
            {
                return;
            }

            var evt = new CallHistoryDeleteEvent(entity);

            BvHistoryAdapter.DeleteByCondition(
                "[ID]=@id\r\n",
                new SqlParameter("@id", id));

            BvCallHistoryExAdapter.Insert(new BvCallHistoryExEntity {
                OperationType = (byte)OperationType.DeleteCallHistory,
                InterviewID = entity.InterviewId ?? 0,
                SurveyId = entity.SurveyId,
                FiredTime = DateTime.UtcNow,
                ITS = entity.ITS,
                DialingMode = 0,
                CallCenterId = entity.CallCenterID
            });

            evt.Finish();
        }

        public BvHistoryEntity GetById(int id)
        {
            return BvHistoryAdapter.GetByCondition("[ID]=@id\r\n", new SqlParameter("@id", id)).FirstOrDefault();
        }

        public void Update(BvHistoryEntity entity)
        {
            var evt = new CallHistoryUpdateEvent(entity);

            BvHistoryAdapter.UpdateByCondition(entity, "[ID]=@entityId\r\n", new SqlParameter("@entityId", entity.ID));

            BvCallHistoryExAdapter.Insert(new BvCallHistoryExEntity {
                OperationType = (byte)OperationType.EditCallHistory,
                InterviewID = entity.InterviewId ?? 0,
                SurveyId = entity.SurveyId,
                FiredTime = DateTime.UtcNow,
                ITS = entity.ITS,
                DialingMode = 0,
                CallCenterId = entity.CallCenterID
            });

            evt.Finish();
        }

        public DataTable GetCallAttemptsForInterview(int surveyId, int interviewId)
        {
            var query = @"
                select 
                    isnull(h.CallAttemptNumber, 0) as AttemptNumber,
                    isnull(h.TelephoneNumber, ''),
                    h.FiredTime as EndTimeUtc,
                    h.PersonSID as InterviewerId,
                    isnull(CAST(h.ITS AS INT), -1) as ExtendedStatus,
                    isnull(st.AaporCode, '') as AaporCode,
                    isnull(h.Duration, 0) as Duration,
                    isnull(h.WaitingTime, 0) as WaitingTime,
                    isnull(h.PreviewTime, 0) as PreviewTime,
                    isnull(h.OpenEndReviewDuration, 0) as OpenEndReviewDuration,
                    isnull(h.WrapTime, 0) as WrapTime,
                    isnull(h.ConnectedTime, 0) as ConnectedTime,
                    h.CallCenterID,
                    h.TelephoneNumber
                from BvHistory  h
                left join BvSurvey sv on sv.SID = h.SurveyId
                left join BvStateGroup sg on sg.ID = sv.StateGroupId
                left join BvState st on st.StateGroupID = sg.ID and st.StateID = h.ITS
                where SurveyId = @surveyId and InterviewId = @interviewId
                order by h.FiredTime desc
            ";
            var dbEngine = new DatabaseEngine();
            var dataTable = dbEngine.ExecuteDataTableInNewConnection<DataTable>(query, CommandType.Text,
                new SqlParameter("surveyId", surveyId),
                new SqlParameter("interviewId", interviewId));
            
            return dataTable;
        }
    }
}