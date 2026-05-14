using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IDialersRepository
    {
        BvDialersEntity GetById(int id);
        void Update(BvDialersEntity dialerEntity, bool useNotification = true);
        List<BvDialersEntity> GetAll();
        bool IsAnyDialerConfigured();
        int? GetNextAvailableDialer(int surveyId, DialType dialType, int callCenterId = 0);
        BvDialersEntity AddDialer(BvDialersEntity dialer);
        void Delete(int dialerId);
    }
}
