using System;
using Confirmit.CATI.Core.Repositories;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Fakes
{
    public class StubIDialerStateRepository : IDialerStateRepository 
    {
        private IDialerStateRepository _inner;

        public StubIDialerStateRepository()
        {
            _inner = null;
        }

        public IDialerStateRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CreateOrUpdateWithNotificationExpirationDateTimeStringInt32DateTimeDelegate(string serverName, int dialerId, DateTime notificationExpirationTime);
        public CreateOrUpdateWithNotificationExpirationDateTimeStringInt32DateTimeDelegate CreateOrUpdateWithNotificationExpirationDateTimeStringInt32DateTime;

        void IDialerStateRepository.CreateOrUpdateWithNotificationExpirationDateTime(string serverName, int dialerId, DateTime notificationExpirationTime)
        {

            if (CreateOrUpdateWithNotificationExpirationDateTimeStringInt32DateTime != null)
            {
                CreateOrUpdateWithNotificationExpirationDateTimeStringInt32DateTime(serverName, dialerId, notificationExpirationTime);
            } else if (_inner != null)
            {
                ((IDialerStateRepository)_inner).CreateOrUpdateWithNotificationExpirationDateTime(serverName, dialerId, notificationExpirationTime);
            }
        }

        public delegate List<BvDialerStateEntity> GetByDialerIdInt32Delegate(int dialerId);
        public GetByDialerIdInt32Delegate GetByDialerIdInt32;

        List<BvDialerStateEntity> IDialerStateRepository.GetByDialerId(int dialerId)
        {


            if (GetByDialerIdInt32 != null)
            {
                return GetByDialerIdInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerStateRepository)_inner).GetByDialerId(dialerId);
            }

            return default(List<BvDialerStateEntity>);
        }

        public delegate BvDialerStateEntity GetByDialerIdAndServerNameStringInt32Delegate(string serverName, int dialerId);
        public GetByDialerIdAndServerNameStringInt32Delegate GetByDialerIdAndServerNameStringInt32;

        BvDialerStateEntity IDialerStateRepository.GetByDialerIdAndServerName(string serverName, int dialerId)
        {


            if (GetByDialerIdAndServerNameStringInt32 != null)
            {
                return GetByDialerIdAndServerNameStringInt32(serverName, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerStateRepository)_inner).GetByDialerIdAndServerName(serverName, dialerId);
            }

            return default(BvDialerStateEntity);
        }

        public delegate DateTime GetDialerNotificationExpirationTimeInt32Delegate(int dialerId);
        public GetDialerNotificationExpirationTimeInt32Delegate GetDialerNotificationExpirationTimeInt32;

        DateTime IDialerStateRepository.GetDialerNotificationExpirationTime(int dialerId)
        {


            if (GetDialerNotificationExpirationTimeInt32 != null)
            {
                return GetDialerNotificationExpirationTimeInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerStateRepository)_inner).GetDialerNotificationExpirationTime(dialerId);
            }

            return default(DateTime);
        }

        public delegate DateTime GetLastSuccessfulGetStateTimestampInt32Delegate(int dialerId);
        public GetLastSuccessfulGetStateTimestampInt32Delegate GetLastSuccessfulGetStateTimestampInt32;

        DateTime IDialerStateRepository.GetLastSuccessfulGetStateTimestamp(int dialerId)
        {


            if (GetLastSuccessfulGetStateTimestampInt32 != null)
            {
                return GetLastSuccessfulGetStateTimestampInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerStateRepository)_inner).GetLastSuccessfulGetStateTimestamp(dialerId);
            }

            return default(DateTime);
        }

        public delegate DateTime GetLastSuccessfulNotificationTimestampInt32Delegate(int dialerId);
        public GetLastSuccessfulNotificationTimestampInt32Delegate GetLastSuccessfulNotificationTimestampInt32;

        DateTime IDialerStateRepository.GetLastSuccessfulNotificationTimestamp(int dialerId)
        {


            if (GetLastSuccessfulNotificationTimestampInt32 != null)
            {
                return GetLastSuccessfulNotificationTimestampInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerStateRepository)_inner).GetLastSuccessfulNotificationTimestamp(dialerId);
            }

            return default(DateTime);
        }

    }
}