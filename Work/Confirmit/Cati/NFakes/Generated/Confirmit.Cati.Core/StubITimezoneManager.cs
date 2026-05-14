using System;
using Confirmit.CATI.Core.Timezones;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Timezones.Fakes
{
    public class StubITimezoneManager : ITimezoneManager 
    {
        private ITimezoneManager _inner;

        public StubITimezoneManager()
        {
            _inner = null;
        }

        public ITimezoneManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<TimezoneEntity> GetTimezonesDelegate();
        public GetTimezonesDelegate GetTimezones;

        List<TimezoneEntity> ITimezoneManager.GetTimezones()
        {


            if (GetTimezones != null)
            {
                return GetTimezones();
            } else if (_inner != null)
            {
                return ((ITimezoneManager)_inner).GetTimezones();
            }

            return default(List<TimezoneEntity>);
        }

        public delegate List<string> GetSystemTimezoneNamesDelegate();
        public GetSystemTimezoneNamesDelegate GetSystemTimezoneNames;

        List<string> ITimezoneManager.GetSystemTimezoneNames()
        {


            if (GetSystemTimezoneNames != null)
            {
                return GetSystemTimezoneNames();
            } else if (_inner != null)
            {
                return ((ITimezoneManager)_inner).GetSystemTimezoneNames();
            }

            return default(List<string>);
        }

        public delegate TimeZoneInfo GetMasterTimezoneInfoInt32Delegate(int timezoneId);
        public GetMasterTimezoneInfoInt32Delegate GetMasterTimezoneInfoInt32;

        TimeZoneInfo ITimezoneManager.GetMasterTimezoneInfo(int timezoneId)
        {


            if (GetMasterTimezoneInfoInt32 != null)
            {
                return GetMasterTimezoneInfoInt32(timezoneId);
            } else if (_inner != null)
            {
                return ((ITimezoneManager)_inner).GetMasterTimezoneInfo(timezoneId);
            }

            return default(TimeZoneInfo);
        }

        public delegate List<BvTimezoneEntity> GetCustomTimezonesInt32Delegate(int parentTimezoneId);
        public GetCustomTimezonesInt32Delegate GetCustomTimezonesInt32;

        List<BvTimezoneEntity> ITimezoneManager.GetCustomTimezones(int parentTimezoneId)
        {


            if (GetCustomTimezonesInt32 != null)
            {
                return GetCustomTimezonesInt32(parentTimezoneId);
            } else if (_inner != null)
            {
                return ((ITimezoneManager)_inner).GetCustomTimezones(parentTimezoneId);
            }

            return default(List<BvTimezoneEntity>);
        }

        public delegate int AddCustomTimezoneStringInt32Delegate(string name, int parentTimezoneId);
        public AddCustomTimezoneStringInt32Delegate AddCustomTimezoneStringInt32;

        int ITimezoneManager.AddCustomTimezone(string name, int parentTimezoneId)
        {


            if (AddCustomTimezoneStringInt32 != null)
            {
                return AddCustomTimezoneStringInt32(name, parentTimezoneId);
            } else if (_inner != null)
            {
                return ((ITimezoneManager)_inner).AddCustomTimezone(name, parentTimezoneId);
            }

            return default(int);
        }

        public delegate BvTimezoneEntity GetActiveTimezoneInt32Delegate(int timezoneId);
        public GetActiveTimezoneInt32Delegate GetActiveTimezoneInt32;

        BvTimezoneEntity ITimezoneManager.GetActiveTimezone(int timezoneId)
        {


            if (GetActiveTimezoneInt32 != null)
            {
                return GetActiveTimezoneInt32(timezoneId);
            } else if (_inner != null)
            {
                return ((ITimezoneManager)_inner).GetActiveTimezone(timezoneId);
            }

            return default(BvTimezoneEntity);
        }

        public delegate void UpdateCustomTimezoneInt32StringInt32Delegate(int customTimezoneId, string name, int parentId);
        public UpdateCustomTimezoneInt32StringInt32Delegate UpdateCustomTimezoneInt32StringInt32;

        void ITimezoneManager.UpdateCustomTimezone(int customTimezoneId, string name, int parentId)
        {

            if (UpdateCustomTimezoneInt32StringInt32 != null)
            {
                UpdateCustomTimezoneInt32StringInt32(customTimezoneId, name, parentId);
            } else if (_inner != null)
            {
                ((ITimezoneManager)_inner).UpdateCustomTimezone(customTimezoneId, name, parentId);
            }
        }

        private BvTimezoneEntityCollection _TimezonesList;
        public Func<BvTimezoneEntityCollection> TimezonesListGet;
        public Action<BvTimezoneEntityCollection> TimezonesListSetBvTimezoneEntityCollection;

        BvTimezoneEntityCollection ITimezoneManager.TimezonesList
        {
            get
            {
                if (TimezonesListGet != null)
                {
                    return TimezonesListGet();
                } else if (_inner != null)
                {
                    return ((ITimezoneManager)_inner).TimezonesList;
                }

                if (TimezonesListSetBvTimezoneEntityCollection == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TimezonesList;
                }

                return default(BvTimezoneEntityCollection);
            }

        }

    }
}