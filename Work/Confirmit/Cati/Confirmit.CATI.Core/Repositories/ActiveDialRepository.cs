using System;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Repositories
{
    public class ActiveDialRepository : IActiveDialRepository
    {

        [CanBeNull]
        public BvActiveDialEntity TryGetById(long? dialId)
        {
            if (dialId == null)
                return null;
            return BvActiveDialAdapter.GetByCondition("Id = @dialId", new SqlParameter("@dialId", dialId)).SingleOrDefault();

        }
        [CanBeNull]
        public BvActiveDialEntity TryGetByCallId(long? callId)
        {
            if (callId == null)
                return null;
            return BvActiveDialAdapter.GetByCondition("CallId = @callId", new SqlParameter("@callId", callId)).SingleOrDefault();
        }

        [CanBeNull]
        public BvActiveDialEntity TryGetByTransferId(string transferId)
        {
            if (transferId == null)
                return null;
            return BvActiveDialAdapter.GetByCondition("TransferId = @transferId", new SqlParameter("@transferId", transferId)).SingleOrDefault();
        }


        [CanBeNull]
        public BvActiveDialEntity TryGetByInboundCallId(string inboundCallId)
        {
            return BvActiveDialAdapter.GetByCondition("inboundCallId = @inboundCallId", new SqlParameter("@inboundCallId", inboundCallId)).SingleOrDefault();
        }

        [CanBeNull]
        public BvActiveDialEntity TryGetBySurveyAndInterviewId(int surveyId, int interviewId)
        {
            return BvActiveDialAdapter.GetByCondition("SurveyId = @surveyId AND InterviewId = @interviewId", 
                new SqlParameter("@surveyId", surveyId),
                new SqlParameter("@interviewId", interviewId)).SingleOrDefault();
        }

        public BvActiveDialEntity Insert(BvActiveDialEntity dial)
        {
            return BvActiveDialAdapter.ReadList(
                BvSpActiveDial_InsertAdapter.ExecuteReader(
                    dial.Type, 
                    dial.DialerId, 
                    dial.DialerTelephoneNumber, 
                    dial.RespondentTelephoneNumber, 
                    dial.State, 
                    dial.InboundCallId, 
                    dial.InitialSurveyId,
                    dial.SurveyId,
                    dial.CampaignId,
                    dial.InterviewId,
                    dial.CallId,
                    dial.MainPersonId,
                    dial.JsonCallOutcomeMetadata,
                    dial.RingTime,
                    dial.DialerCallerId,
                    dial.DialerCallOutcome,
                    UpdateActiveDialInBvSvySchedule))
                .Single();
        }
        
        public void Update(BvActiveDialEntity entity)
        {
            BvSpActiveDial_UpdateAdapter.ExecuteNonQuery(
                entity.Id,
                entity.Type,
                entity.State,
                entity.AnswerTime,
                entity.TransferId,
                entity.SurveyId,
                entity.CampaignId,
                entity.InterviewId,
                entity.CallId,
                entity.MainPersonId,
                entity.JsonTransferState,
                entity.TransferType,
                entity.JsonCallOutcomeMetadata,
                entity.RingTime,
                entity.DialerCallerId,
                entity.DialerCallOutcome,
                UpdateActiveDialInBvSvySchedule);
        }


        public void Delete(long id, CallCompleteStatus callCompleteStatus)
        {
            BvSpActiveDial_DeleteSingleAdapter.ExecuteNonQuery(id, (byte)callCompleteStatus, null, null, null, null, UpdateActiveDialInBvSvySchedule);
        }

        public BvActiveDialEntity GetByCallIdWithCheck(long callId)
        {
            var dial = TryGetByCallId(callId);
            if (dial == null)
            {
                throw new Exception($"Active dial for call Id = {callId} not found");
            }

            return dial;
        }

        public BvActiveDialEntity GetByTransferIdWithCheck(string transferId)
        {
            var dial = TryGetByTransferId(transferId);
            if (dial == null)
            {
                throw new Exception($"Active dial for transfer Id = {transferId} not found");
            }

            return dial;
        }

        private bool UpdateActiveDialInBvSvySchedule => !ServiceLocator.Resolve<IToggleSettings>().BvSvyScheduleDeadlockReduction;
    }
}
