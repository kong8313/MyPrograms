using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Common
{
    public class DialTypeOptions
    {
        public static IEnumerable<DialType> GetAll()
        {
            yield return DialType.Landline;
            yield return DialType.Cellphone;
            yield return DialType.Assisted;
        }
        public static IEnumerable<DialType> GetAllowed()
        {
            yield return DialType.Landline;
            var settings = ServiceLocator.Resolve<IToggleSettings>();
            if (settings.EnableTCPA)
                yield return DialType.Cellphone;
            if (settings.EnableAgentAssistedDialling)
                yield return DialType.Assisted;
        }
    }
}
