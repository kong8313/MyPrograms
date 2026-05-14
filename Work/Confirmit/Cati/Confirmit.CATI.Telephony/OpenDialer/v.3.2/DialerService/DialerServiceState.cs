using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using DialerCommon;

namespace Confirmit.CATI.Telephony.DialerService
{
    [Serializable]
    public class DialerServiceState
    {
        private const string ServiceStateFileName = "DialerServiceState.xml";

        // Data to serialize

        public int companyId;
        public int dialerId;

        public DateTime SaveTime;

        public DialerServiceState()
        {
            Clear();
        }

        public static bool Load(ref DialerServiceState serviceState)
        {
            try
            {
                using (var stream = File.Open(GetServiceStateFileFullPath(), FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    serviceState = (DialerServiceState)formatter.Deserialize(stream);
                }

                LogServiceStateLoadInfo(serviceState);

                return true;
            }
            catch (FileNotFoundException)
            {
                DialerService.Logger.Info(
                    "DialerServiceState.Restore",
                    "Service is started from fresh state, nothing to restore.");
                return false;
            }
            catch (Exception ex)
            {
                DialerService.Logger.Error(
                    "DialerServiceState.Restore",
                    "Unexpected exception: {0}", ex);
                return false;
            }
        }

        internal bool IsExpired()
        {
            var difference = DateTime.UtcNow - SaveTime;
            return difference.TotalSeconds > Settings.Default.ServiceStateExpirationTimeout;
        }

        public void Save()
        {
            SetSaveTimeToNow();

            try
            {
                Stream stream = File.Open(GetServiceStateFileFullPath(), FileMode.Create);
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Close();
            }
            catch (Exception ex)
            {
                DialerService.Logger.Error("DialerServiceState.Save", "Unexpected exception: {0}", ex);
            }
        }

        private void SetSaveTimeToNow()
        {
            SaveTime = DateTime.UtcNow;
        }

        public void Clear()
        {
            companyId = 0;
            dialerId = 0;
        }

        public static string GetServiceStateFileFullPath()
        {
            return DialerServiceAppDataPath.GetServiceAppDataPath() + ServiceStateFileName;
        }

        private static void LogServiceStateLoadInfo(DialerServiceState serviceState)
        {
            DialerService.Logger.Info(
                "DialerServiceState.LogServiceStateLoadInfo",
                "Dialer service state is successfully deserialized from state file: companyId={0}, dialerId={1}", 
                serviceState.companyId, 
                serviceState.dialerId);
        }
    }
}
