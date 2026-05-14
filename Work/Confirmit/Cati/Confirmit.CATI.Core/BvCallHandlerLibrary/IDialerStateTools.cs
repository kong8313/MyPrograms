using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System;

namespace BvCallHandlerLibrary
{
    public interface IDialerStateTools
    {
        string BvDialerStateToString(int dialerId);
        void SetDialerNotificationExpirationTime(int dialerId, DateTime expirationTime);
        void UpdateDialerStateNotificationTime(int dialerId);
        void UpdateGetStateTime(int dialerId, bool isGetStateSuccessful);
        bool IsGetStateTimeoutElapsed(int dialerId, out DateTime lastSuccessfulGetState);
        bool IsReconnectTimeoutElapsed(BvDialersEntity dialer);
    }
}