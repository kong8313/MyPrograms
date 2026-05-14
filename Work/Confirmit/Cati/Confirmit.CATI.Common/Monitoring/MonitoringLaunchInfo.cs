using System;

namespace Confirmit.CATI.Common.Monitoring
{
    [Serializable]
    public class MonitoringLaunchInfo
    {        
        public int CompanyId { get;  set; }
        public string ServerName { get; set; }
        public string EncryptedData { get;  set; }
        public bool PreventSeparateInstance { get; set; }
    }
}
