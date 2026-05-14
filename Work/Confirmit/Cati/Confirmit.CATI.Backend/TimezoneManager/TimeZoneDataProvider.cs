using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Backend.TimezoneManager
{
    public class TimeZoneDataProvider
    {
        public List<BvTimezoneEntity> GetSystemTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(new TimezoneConverter().TryConvertToTimezoneEntity)
                .Where(x => x != null).OrderBy(x => x.StandardName).ToList();
        }
    }
}