using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IActiveDialRepository
    {
        BvActiveDialEntity TryGetById(long? dialId);
        BvActiveDialEntity TryGetByCallId(long? callId);
        BvActiveDialEntity TryGetByTransferId(string transferId);
        BvActiveDialEntity TryGetByInboundCallId(string inboundCallId);
        BvActiveDialEntity TryGetBySurveyAndInterviewId(int surveyId, int interviewId);
        BvActiveDialEntity GetByCallIdWithCheck(long callId);
        BvActiveDialEntity GetByTransferIdWithCheck(string transferId);

        BvActiveDialEntity Insert(BvActiveDialEntity dial);
        void Update(BvActiveDialEntity entity);
        void Delete(long id, CallCompleteStatus callCompleteStatus);
    }
}
