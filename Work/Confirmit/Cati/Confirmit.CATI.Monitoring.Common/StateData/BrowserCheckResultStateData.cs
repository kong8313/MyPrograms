using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Contains data for BrowserCheckResult event
    /// </summary>
    [Serializable]
    public class BrowserCheckResultStateData : BaseStateData
    {
        public List<string> WarningMessages { get; set; }
    }
}
