using System.IO;

namespace Confirmit.CATI.Core.Misc
{
    public interface IPgpEncryptionService
    {
        MemoryStream EncryptIfNeeded(string filePath, ref string fileName, bool useEncryption);
        bool IsConfirmitSettingEnabled();
        void DisableCatiAlwaysEncryptFilesSetting();
    }
}