using System.Collections.Generic;
using Confirmit.CATI.Core.CallCenters;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ICallCenterRepository
    {
        BvCallCenterEntity Get(int id);
        BvCallCenterEntityWithDialerIds GetCallCenterWithDialers(int id);
        
        BvCallCenterEntity Default { get; }

        List<BvCallCenterEntity> GetAssignedToSurvey(int surveyId);
        List<BvCallCenterEntity> GetAll();
        List<BvCallCenterEntityWithDialerIds> GetAllWithDialerIds();

        void Insert(BvCallCenterEntity entity);
        void Insert(BvCallCenterEntityWithDialerIds entity);
        void Update(BvCallCenterEntity entity);
        void Update(BvCallCenterEntityWithDialerIds entity, int[] newDialerIds, int[] oldDialerIds);
        void Delete(int id, int moveToCallCenterId, InterviewerActionOnCallCenterDelete interviewerAction);
    }
}
