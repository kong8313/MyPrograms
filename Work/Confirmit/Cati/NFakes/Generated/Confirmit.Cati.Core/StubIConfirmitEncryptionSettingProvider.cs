using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIConfirmitEncryptionSettingProvider : IConfirmitEncryptionSettingProvider 
    {
        private IConfirmitEncryptionSettingProvider _inner;

        public StubIConfirmitEncryptionSettingProvider()
        {
            _inner = null;
        }

        public IConfirmitEncryptionSettingProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool GetAlwaysUseEncryptedFileTransferSettingDelegate();
        public GetAlwaysUseEncryptedFileTransferSettingDelegate GetAlwaysUseEncryptedFileTransferSetting;

        bool IConfirmitEncryptionSettingProvider.GetAlwaysUseEncryptedFileTransferSetting()
        {


            if (GetAlwaysUseEncryptedFileTransferSetting != null)
            {
                return GetAlwaysUseEncryptedFileTransferSetting();
            } else if (_inner != null)
            {
                return ((IConfirmitEncryptionSettingProvider)_inner).GetAlwaysUseEncryptedFileTransferSetting();
            }

            return default(bool);
        }

    }
}