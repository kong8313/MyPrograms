using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories
{
    public interface IDialerStateRepository
    {
        void CreateOrUpdateWithNotificationExpirationDateTime(string serverName, int dialerId, DateTime notificationExpirationTime);
        List<BvDialerStateEntity> GetByDialerId(int dialerId);
        BvDialerStateEntity GetByDialerIdAndServerName(string serverName, int dialerId);
        DateTime GetDialerNotificationExpirationTime(int dialerId);
        DateTime GetLastSuccessfulGetStateTimestamp(int dialerId);
        DateTime GetLastSuccessfulNotificationTimestamp(int dialerId);
    }
}