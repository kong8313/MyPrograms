using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confirmit.CATI.Core.InstanceRegistrator
{
    public class InstanceManagementLock
    {
        /// <summary>
        /// <see cref="InstanceManagementService.RegisterSchedulingServiceInstance"/>
        /// </summary>
        public static object lockObject = new object();
    }
}
