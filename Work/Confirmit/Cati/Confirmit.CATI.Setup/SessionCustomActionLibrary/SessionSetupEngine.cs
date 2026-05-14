using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.Security.Crypto.Web;
using Microsoft.Deployment.WindowsInstaller;

namespace SessionCustomAction
{
    public class SessionSetupEngine
    {
        private readonly Session _session;
        public ILogger Logger { get; private set; }

        public SessionSetupEngine(Session session)
            : this(session, false)
        {
        }

        public SessionSetupEngine(Session session, bool useStandardLog)
            : this(session, useStandardLog, new string[0])
        {
        }

        public SessionSetupEngine(Session session, bool useStandardLog, string[] secretLogWords)
        {
            _session = session;
            Logger = new InstallationLogger(session, useStandardLog, secretLogWords);
        }

        /// <summary>
        /// Change information text (after Status word) on Progress dialog
        /// </summary>
        /// <param name="actionName">Custom action name (id)</param>
        /// <param name="description">Displayed text</param>
        public void ChangeProgressStatus(string actionName, string description)
        {
            var record = new Record(2);
            record[1] = actionName;
            record[2] = description;
            _session.Message(InstallMessage.ActionStart, record);
        }

        /// <summary>
        /// Set value for encrypted parameter, if decrypted one exists or 
        /// set value to decrypted parameter, if encrypted one exists
        /// </summary>
        /// <param name="parameterName">Decrypted parameter name</param>
        /// <param name="encryptedParameterName">Encrypted parameter name</param>
        public void DefineEncryptedAndDecryptParameters(string parameterName, string encryptedParameterName)
        {
            if (!string.IsNullOrEmpty(_session[parameterName]) && string.IsNullOrEmpty(_session[encryptedParameterName]))
            {
                _session[encryptedParameterName] = EncryptionUsingMachineKey.Encrypt(DataProtection.All, _session[parameterName]);
            }
            else if (string.IsNullOrEmpty(_session[parameterName]) && !string.IsNullOrEmpty(_session[encryptedParameterName]))
            {
                _session[parameterName] = EncryptionUsingMachineKey.Decrypt(DataProtection.All, _session[encryptedParameterName]);
            }
            else
            {
                Logger.WriteLog("We did nothing in DefineEncryptedAndDecriptParameters method");
            }
        }
    }
}
