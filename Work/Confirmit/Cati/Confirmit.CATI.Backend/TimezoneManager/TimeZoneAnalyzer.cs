using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Backend.TimezoneManager
{
    internal class TimeZoneAnalyzer
    {
        public void GenerateIdForSystemTimezones(List<BvTimezoneEntity> systemTimezones, List<BvTimezoneEntity> masterTimezones)
        {
            var nameToId = masterTimezones.ToDictionary(x => x.StandardName, y => (int)y.ID);
            var maxId = masterTimezones.Select(y => y.ID).DefaultIfEmpty(0).Max();

            foreach (var systemTimezone in systemTimezones)
            {
                if (nameToId.TryGetValue(systemTimezone.StandardName, out var id))
                {
                    systemTimezone.ID = id;
                }

                systemTimezone.Name = systemTimezone.Name.Replace("(UTC", "(GMT");
            }

            foreach (var sys in systemTimezones.Where(x => x.ID == 0).OrderBy(y => y.Name))
            {
                sys.ID = ++maxId;
            }
        }

        public List<BvTimezoneEntity> GetUpdatedTimezones(List<BvTimezoneEntity> systemTimezones, List<BvTimezoneEntity> timezones)
        {
            var nameToId = timezones.ToDictionary(x => x.StandardName);

            return systemTimezones.Where(x => nameToId.ContainsKey(x.StandardName) && !nameToId[x.StandardName].Equals(x)).ToList();
        }

        public List<BvTimezoneEntity> GetNewTimezones(List<BvTimezoneEntity> systemTimezones, List<BvTimezoneEntity> timezones)
        {
            var nameToId = timezones.ToDictionary(x => x.StandardName);

            return systemTimezones.Where(x => !nameToId.ContainsKey(x.StandardName)).ToList();
        }
    }
}