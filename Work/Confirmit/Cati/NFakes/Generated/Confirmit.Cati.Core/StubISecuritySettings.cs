using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubISecuritySettings : ISecuritySettings 
    {
        private ISecuritySettings _inner;

        public StubISecuritySettings()
        {
            _inner = null;
        }

        public ISecuritySettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private bool _AlwaysEncryptFiles;
        public Func<bool> AlwaysEncryptFilesGet;
        public Action<bool> AlwaysEncryptFilesSetBoolean;

        bool ISecuritySettings.AlwaysEncryptFiles
        {
            get
            {
                if (AlwaysEncryptFilesGet != null)
                {
                    return AlwaysEncryptFilesGet();
                } else if (_inner != null)
                {
                    return ((ISecuritySettings)_inner).AlwaysEncryptFiles;
                }

                if (AlwaysEncryptFilesSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AlwaysEncryptFiles;
                }

                return default(bool);
            }

            set
            {
                if (AlwaysEncryptFilesSetBoolean != null)
                {
                    AlwaysEncryptFilesSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISecuritySettings)_inner).AlwaysEncryptFiles = value;
                    return;
                }

                if (AlwaysEncryptFilesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AlwaysEncryptFiles = value;
                }

            }
        }

        private string _UserForEncryption;
        public Func<string> UserForEncryptionGet;
        public Action<string> UserForEncryptionSetString;

        string ISecuritySettings.UserForEncryption
        {
            get
            {
                if (UserForEncryptionGet != null)
                {
                    return UserForEncryptionGet();
                } else if (_inner != null)
                {
                    return ((ISecuritySettings)_inner).UserForEncryption;
                }

                if (UserForEncryptionSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _UserForEncryption;
                }

                return default(string);
            }

            set
            {
                if (UserForEncryptionSetString != null)
                {
                    UserForEncryptionSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISecuritySettings)_inner).UserForEncryption = value;
                    return;
                }

                if (UserForEncryptionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _UserForEncryption = value;
                }

            }
        }

    }
}