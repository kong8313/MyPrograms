using System;
using Confirmit.CATI.Core.Misc;
using System.IO;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIPgpEncryptionService : IPgpEncryptionService 
    {
        private IPgpEncryptionService _inner;

        public StubIPgpEncryptionService()
        {
            _inner = null;
        }

        public IPgpEncryptionService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate MemoryStream EncryptIfNeededStringStringRefBooleanDelegate(string filePath, ref string fileName, bool useEncryption);
        public EncryptIfNeededStringStringRefBooleanDelegate EncryptIfNeededStringStringRefBoolean;

        MemoryStream IPgpEncryptionService.EncryptIfNeeded(string filePath, ref string fileName, bool useEncryption)
        {


            if (EncryptIfNeededStringStringRefBoolean != null)
            {
                return EncryptIfNeededStringStringRefBoolean(filePath, ref fileName, useEncryption);
            } else if (_inner != null)
            {
                return ((IPgpEncryptionService)_inner).EncryptIfNeeded(filePath, ref fileName, useEncryption);
            }

            return default(MemoryStream);
        }

        public delegate bool IsConfirmitSettingEnabledDelegate();
        public IsConfirmitSettingEnabledDelegate IsConfirmitSettingEnabled;

        bool IPgpEncryptionService.IsConfirmitSettingEnabled()
        {


            if (IsConfirmitSettingEnabled != null)
            {
                return IsConfirmitSettingEnabled();
            } else if (_inner != null)
            {
                return ((IPgpEncryptionService)_inner).IsConfirmitSettingEnabled();
            }

            return default(bool);
        }

        public delegate void DisableCatiAlwaysEncryptFilesSettingDelegate();
        public DisableCatiAlwaysEncryptFilesSettingDelegate DisableCatiAlwaysEncryptFilesSetting;

        void IPgpEncryptionService.DisableCatiAlwaysEncryptFilesSetting()
        {

            if (DisableCatiAlwaysEncryptFilesSetting != null)
            {
                DisableCatiAlwaysEncryptFilesSetting();
            } else if (_inner != null)
            {
                ((IPgpEncryptionService)_inner).DisableCatiAlwaysEncryptFilesSetting();
            }
        }

    }
}