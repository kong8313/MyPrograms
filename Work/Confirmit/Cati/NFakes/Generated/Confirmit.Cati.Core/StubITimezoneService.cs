using System;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ConsoleService.Abstract;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubITimezoneService : ITimezoneService 
    {
        private ITimezoneService _inner;

        public StubITimezoneService()
        {
            _inner = null;
        }

        public ITimezoneService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int GetDefaultCallCenterTimezoneIdDelegate();
        public GetDefaultCallCenterTimezoneIdDelegate GetDefaultCallCenterTimezoneId;

        int ITimezoneService.GetDefaultCallCenterTimezoneId()
        {


            if (GetDefaultCallCenterTimezoneId != null)
            {
                return GetDefaultCallCenterTimezoneId();
            } else if (_inner != null)
            {
                return ((ITimezoneService)_inner).GetDefaultCallCenterTimezoneId();
            }

            return default(int);
        }

        public delegate BvTimezoneEntity GetDefaultCallCenterTimezoneDelegate();
        public GetDefaultCallCenterTimezoneDelegate GetDefaultCallCenterTimezone;

        BvTimezoneEntity ITimezoneService.GetDefaultCallCenterTimezone()
        {


            if (GetDefaultCallCenterTimezone != null)
            {
                return GetDefaultCallCenterTimezone();
            } else if (_inner != null)
            {
                return ((ITimezoneService)_inner).GetDefaultCallCenterTimezone();
            }

            return default(BvTimezoneEntity);
        }

        public delegate int GetTimezoneIdOrDefaultCallCenterTimezoneIdNullableOfInt32Delegate(int? timezoneId);
        public GetTimezoneIdOrDefaultCallCenterTimezoneIdNullableOfInt32Delegate GetTimezoneIdOrDefaultCallCenterTimezoneIdNullableOfInt32;

        int ITimezoneService.GetTimezoneIdOrDefaultCallCenterTimezoneId(int? timezoneId)
        {


            if (GetTimezoneIdOrDefaultCallCenterTimezoneIdNullableOfInt32 != null)
            {
                return GetTimezoneIdOrDefaultCallCenterTimezoneIdNullableOfInt32(timezoneId);
            } else if (_inner != null)
            {
                return ((ITimezoneService)_inner).GetTimezoneIdOrDefaultCallCenterTimezoneId(timezoneId);
            }

            return default(int);
        }

        public delegate BvTimezoneEntity GetTimezoneOrDefaultCallCenterTimezoneInt32Delegate(int timezoneId);
        public GetTimezoneOrDefaultCallCenterTimezoneInt32Delegate GetTimezoneOrDefaultCallCenterTimezoneInt32;

        BvTimezoneEntity ITimezoneService.GetTimezoneOrDefaultCallCenterTimezone(int timezoneId)
        {


            if (GetTimezoneOrDefaultCallCenterTimezoneInt32 != null)
            {
                return GetTimezoneOrDefaultCallCenterTimezoneInt32(timezoneId);
            } else if (_inner != null)
            {
                return ((ITimezoneService)_inner).GetTimezoneOrDefaultCallCenterTimezone(timezoneId);
            }

            return default(BvTimezoneEntity);
        }

        public delegate DateTime ConvertTimeFromUtcInt32DateTimeDelegate(int sid, DateTime utcTime);
        public ConvertTimeFromUtcInt32DateTimeDelegate ConvertTimeFromUtcInt32DateTime;

        DateTime ITimezoneService.ConvertTimeFromUtc(int sid, DateTime utcTime)
        {


            if (ConvertTimeFromUtcInt32DateTime != null)
            {
                return ConvertTimeFromUtcInt32DateTime(sid, utcTime);
            } else if (_inner != null)
            {
                return ((ITimezoneService)_inner).ConvertTimeFromUtc(sid, utcTime);
            }

            return default(DateTime);
        }

        public delegate DateTime ConvertTimeToUtcInt32DateTimeDelegate(int sid, DateTime localTime);
        public ConvertTimeToUtcInt32DateTimeDelegate ConvertTimeToUtcInt32DateTime;

        DateTime ITimezoneService.ConvertTimeToUtc(int sid, DateTime localTime)
        {


            if (ConvertTimeToUtcInt32DateTime != null)
            {
                return ConvertTimeToUtcInt32DateTime(sid, localTime);
            } else if (_inner != null)
            {
                return ((ITimezoneService)_inner).ConvertTimeToUtc(sid, localTime);
            }

            return default(DateTime);
        }

        public delegate TimeZoneInfo GetTimezoneInfoInt32Delegate(int sid);
        public GetTimezoneInfoInt32Delegate GetTimezoneInfoInt32;

        TimeZoneInfo ITimezoneService.GetTimezoneInfo(int sid)
        {


            if (GetTimezoneInfoInt32 != null)
            {
                return GetTimezoneInfoInt32(sid);
            } else if (_inner != null)
            {
                return ((ITimezoneService)_inner).GetTimezoneInfo(sid);
            }

            return default(TimeZoneInfo);
        }

        public delegate Timezone GetTimeZoneInt32Delegate(int timezoneId);
        public GetTimeZoneInt32Delegate GetTimeZoneInt32;

        Timezone ITimezoneService.GetTimeZone(int timezoneId)
        {


            if (GetTimeZoneInt32 != null)
            {
                return GetTimeZoneInt32(timezoneId);
            } else if (_inner != null)
            {
                return ((ITimezoneService)_inner).GetTimeZone(timezoneId);
            }

            return default(Timezone);
        }

        public delegate TimeZoneInfo GetMasterTimezoneInfoInt32Delegate(int sid);
        public GetMasterTimezoneInfoInt32Delegate GetMasterTimezoneInfoInt32;

        TimeZoneInfo ITimezoneService.GetMasterTimezoneInfo(int sid)
        {


            if (GetMasterTimezoneInfoInt32 != null)
            {
                return GetMasterTimezoneInfoInt32(sid);
            } else if (_inner != null)
            {
                return ((ITimezoneService)_inner).GetMasterTimezoneInfo(sid);
            }

            return default(TimeZoneInfo);
        }

    }
}