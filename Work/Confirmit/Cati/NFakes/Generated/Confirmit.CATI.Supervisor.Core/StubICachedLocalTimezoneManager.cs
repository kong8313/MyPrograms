using System;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Core.Timezone.Fakes
{
    public class StubICachedLocalTimezoneManager : ICachedLocalTimezoneManager 
    {
        private ICachedLocalTimezoneManager _inner;

        public StubICachedLocalTimezoneManager()
        {
            _inner = null;
        }

        public ICachedLocalTimezoneManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int GetLocalTimezoneIdDelegate();
        public GetLocalTimezoneIdDelegate GetLocalTimezoneId;

        int ICachedLocalTimezoneManager.GetLocalTimezoneId()
        {


            if (GetLocalTimezoneId != null)
            {
                return GetLocalTimezoneId();
            } else if (_inner != null)
            {
                return ((ICachedLocalTimezoneManager)_inner).GetLocalTimezoneId();
            }

            return default(int);
        }

        public delegate BvTimezoneEntity GetLocalTimezoneDelegate();
        public GetLocalTimezoneDelegate GetLocalTimezone;

        BvTimezoneEntity ICachedLocalTimezoneManager.GetLocalTimezone()
        {


            if (GetLocalTimezone != null)
            {
                return GetLocalTimezone();
            } else if (_inner != null)
            {
                return ((ICachedLocalTimezoneManager)_inner).GetLocalTimezone();
            }

            return default(BvTimezoneEntity);
        }

        public delegate void ChangeLocalInt32Delegate(int timezoneId);
        public ChangeLocalInt32Delegate ChangeLocalInt32;

        void ICachedLocalTimezoneManager.ChangeLocal(int timezoneId)
        {

            if (ChangeLocalInt32 != null)
            {
                ChangeLocalInt32(timezoneId);
            } else if (_inner != null)
            {
                ((ICachedLocalTimezoneManager)_inner).ChangeLocal(timezoneId);
            }
        }

        public delegate DateTime GetCurrentLocalTimeDelegate();
        public GetCurrentLocalTimeDelegate GetCurrentLocalTime;

        DateTime ICachedLocalTimezoneManager.GetCurrentLocalTime()
        {


            if (GetCurrentLocalTime != null)
            {
                return GetCurrentLocalTime();
            } else if (_inner != null)
            {
                return ((ICachedLocalTimezoneManager)_inner).GetCurrentLocalTime();
            }

            return default(DateTime);
        }

        public delegate DateTime ConvertToLocalTimeDateTimeDelegate(DateTime utc);
        public ConvertToLocalTimeDateTimeDelegate ConvertToLocalTimeDateTime;

        DateTime ICachedLocalTimezoneManager.ConvertToLocalTime(DateTime utc)
        {


            if (ConvertToLocalTimeDateTime != null)
            {
                return ConvertToLocalTimeDateTime(utc);
            } else if (_inner != null)
            {
                return ((ICachedLocalTimezoneManager)_inner).ConvertToLocalTime(utc);
            }

            return default(DateTime);
        }

        public delegate DateTime ConvertToUtcDateTimeDelegate(DateTime localTime);
        public ConvertToUtcDateTimeDelegate ConvertToUtcDateTime;

        DateTime ICachedLocalTimezoneManager.ConvertToUtc(DateTime localTime)
        {


            if (ConvertToUtcDateTime != null)
            {
                return ConvertToUtcDateTime(localTime);
            } else if (_inner != null)
            {
                return ((ICachedLocalTimezoneManager)_inner).ConvertToUtc(localTime);
            }

            return default(DateTime);
        }

    }
}