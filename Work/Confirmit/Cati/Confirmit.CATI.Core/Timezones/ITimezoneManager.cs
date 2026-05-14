using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Timezones
{
    public interface ITimezoneManager
    {
        BvTimezoneEntityCollection TimezonesList { get; }

        List<TimezoneEntity> GetTimezones();
        List<string> GetSystemTimezoneNames();
        TimeZoneInfo GetMasterTimezoneInfo(int timezoneId);
        List<BvTimezoneEntity> GetCustomTimezones(int parentTimezoneId);
        int AddCustomTimezone(string name, int parentTimezoneId);
        BvTimezoneEntity GetActiveTimezone(int timezoneId);
        void UpdateCustomTimezone(int customTimezoneId, string name, int parentId);
    }
}