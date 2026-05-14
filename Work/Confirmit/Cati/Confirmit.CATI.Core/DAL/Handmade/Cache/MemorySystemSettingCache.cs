using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.DAL.Handmade.Cache
{
    public class MemorySystemSettingCache : ISystemSettingCache
    {
        private Dictionary<string, string> _systemNameToValue; 
        
        public MemorySystemSettingCache()
        {
            Reset();
        }
        
        #region ISystemSettingCache Members

        public string Get(string settingSystemName)
        {
            return _systemNameToValue[settingSystemName];
        }

        public void Set(string settingSystemName, string value)
        {
            _systemNameToValue[settingSystemName] = value;
        }

        public void Set<T>(string settingSystemName, T value)
        {
            string val = value == null ? null : value.ToString();

            Set(settingSystemName, val);
        }

        public void Reset()
        {
            _systemNameToValue = SystemSettingConstants.SystemNameToDefaultValue.ToDictionary(k => k.Key, v => v.Value);
        }

        #endregion
    }
}
