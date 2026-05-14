using System.IO;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.Security.Pgp;

namespace Confirmit.CATI.Core.Misc
{
    public class PgpEncryptionService : IPgpEncryptionService
    {
        private readonly ISecuritySettings _securitySettings;
        private readonly IConfirmitEncryptionSettingProvider _confirmitEncryptionSettingProvider;

        public PgpEncryptionService(ISecuritySettings securitySettings, IConfirmitEncryptionSettingProvider confirmitEncryptionSettingProvider)
        {
            _securitySettings = securitySettings;
            _confirmitEncryptionSettingProvider = confirmitEncryptionSettingProvider;
        }

        private const string PgpExtension = ".pgp";

        private string GetTemporaryDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        public MemoryStream EncryptIfNeeded(string filePath, ref string fileName, bool useEncryption)
        {
            var tempDir = "";
            try
            {
                if (!useEncryption)
                {
                    return new MemoryStream(File.ReadAllBytes(filePath));
                }

                if (_securitySettings.AlwaysEncryptFiles && IsConfirmitSettingEnabled())
                {
                    tempDir = GetTemporaryDirectory();
                    var fileWithProperName = Path.Combine(tempDir, fileName);
                    File.Move(filePath, fileWithProperName);

                    var outputFile = fileWithProperName + PgpExtension;
                    fileName += PgpExtension;

                    var principal = Thread.CurrentPrincipal is SupervisorPrincipal
                        ? null
                        : _securitySettings.UserForEncryption;

                    new PgpUtility().EncryptFile(fileWithProperName, outputFile, principal);

                    return new MemoryStream(File.ReadAllBytes(outputFile));
                }

                if (_securitySettings.AlwaysEncryptFiles)
                {
                    DisableCatiAlwaysEncryptFilesSetting();
                }

                return new MemoryStream(File.ReadAllBytes(filePath));
            }
            catch (PgpException ex)
            {
                throw new UserMessageException(ex.Message);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                if (!string.IsNullOrEmpty(tempDir) && Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        public bool IsConfirmitSettingEnabled()
        {
            return _confirmitEncryptionSettingProvider.GetAlwaysUseEncryptedFileTransferSetting();
        }

        public void DisableCatiAlwaysEncryptFilesSetting()
        {
            using (
                    var transactionScope = new DatabaseTransactionScope("DisableAlwaysEncryptFiles",
                        DeadlockPriority.Supervisor))
            {
                _securitySettings.AlwaysEncryptFiles = false;

                transactionScope.Commit();
            }
        }
    }
}