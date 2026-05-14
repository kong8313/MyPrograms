using System.Collections.Generic;

namespace BuildServerRegistrator
{
    public class ConfigParameters
    {
        public string SqlServerName { get; set; }

        public string SqlLoginName { get; set; }

        public string SqlPassword { get; set; }

        public List<CfgServerConfigValue> CfgServerConfigValues { get; set; }
    }

    public class CfgServerConfigValue
    {
        public string ConfigId { get; set; }

        public string EncryptedConfigValue { get; set; }
    }
}
