using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public class TimezoneResolver
    {
        private HashSet<int> activeTzs;
        private HashSet<int> masterTzs;

        public TimezoneResolver()
        {
            var timezoneRepository = ServiceLocator.Resolve<ITimezoneRepository>();
            activeTzs = new HashSet<int>(from timezone in timezoneRepository.GetActiveList() select timezone.ID);
            masterTzs = new HashSet<int>(from timezone in timezoneRepository.GetMasterList() select timezone.ID);
        }

        public bool HasError { get; private set; }

        public int Resolve(int timezoneId)
        {
            if (timezoneId != 0 && !activeTzs.Contains(timezoneId))
            {
                if (!masterTzs.Contains(timezoneId))
                {
                    HasError = true;
                    return 0;
                }
                TimezoneService.Activate(timezoneId);
                activeTzs.Add(timezoneId);
            }

            return timezoneId;
        }
    }
}