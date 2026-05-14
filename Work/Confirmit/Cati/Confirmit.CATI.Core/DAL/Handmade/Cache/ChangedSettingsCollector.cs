using System.Collections.Generic;
using System.Text;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.DAL.Handmade.Cache
{
    internal class ChangedSettingsCollector
    {
        public int TotalChangesCount { get; private set; }

        public Dictionary<string, KeyValuePair<string, string>> ChangedSettings { get; private set; }

        public Dictionary<string, string> AddedSettings { get; private set; }

        public Dictionary<string, string> RemovedSettings { get; private set; }

        public bool IsAccessAllowedIpAddressesChanged { get; private set; }

        public ChangedSettingsCollector()
        {
            TotalChangesCount = 0;
            ChangedSettings = new Dictionary<string, KeyValuePair<string, string>>();
            AddedSettings = new Dictionary<string, string>();
            RemovedSettings = new Dictionary<string, string>();
            IsAccessAllowedIpAddressesChanged = false;
        }

        public void AddInformationAboutChangedSetting(string settingName, string oldValue, string newValue)
        {
            ChangedSettings.Add(settingName, new KeyValuePair<string, string>(oldValue, newValue));
            TotalChangesCount++;
            if (settingName == SystemSettingConstants.Server.AccessAllowedIPAddresses)
            {
                IsAccessAllowedIpAddressesChanged = true;
            }
        }

        public void AddInformationAboutAddedSetting(string settingName, string value)
        {
            AddedSettings.Add(settingName, value);
            TotalChangesCount++;
        }

        public void AddInformationAboutRemovedSetting(string settingName, string value)
        {
            RemovedSettings.Add(settingName, value);
            TotalChangesCount++;
        }

        public string GetMessageAboutChanges()
        {
            if (TotalChangesCount == 0)
            {
                return string.Empty;
            }

            var message = new StringBuilder("Following system settings were changed:\r\n");
            foreach (var addedSetting in AddedSettings)
            {
                message.AppendFormat("'{0}' with value '{1}' was added\r\n", addedSetting.Key, addedSetting.Value);
            }

            foreach (var changedSetting in ChangedSettings)
            {
                message.AppendFormat("'{0}' from '{1}' to '{2}'\r\n", changedSetting.Key, changedSetting.Value.Key, changedSetting.Value.Value);
            }

            foreach (var removedSetting in RemovedSettings)
            {
                message.AppendFormat("'{0}' with value '{1}' was removed\r\n", removedSetting.Key, removedSetting.Value);
            }

            return message.ToString();
        }
    }
}