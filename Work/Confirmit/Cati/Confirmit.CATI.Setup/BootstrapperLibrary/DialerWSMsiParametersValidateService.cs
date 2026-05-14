using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BootstrapperLibrary.Interfaces;
using BootstrapperLibrary.Properties;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Microsoft.Web.Administration;

namespace BootstrapperLibrary
{
    public class DialerWSMsiParametersValidateService : IParametersValidateService
    {
        private readonly DialerWSMsiParameters _parameters;

        protected readonly ILogger _logger;
        protected readonly IBootstrapperEngine _bootstrapperEngine;
        protected readonly IConfirmitCATIValidator _confirmitCATIValidator;
        protected readonly IPrereqChecker _prereqChecker;

        public DialerWSMsiParametersValidateService(DialerWSMsiParameters parameters, ILogger logger, IBootstrapperEngine bootstrapperEngine, IObjectFactory objectFactory)
        {
            _parameters = parameters;
            _logger = logger;
            _bootstrapperEngine = bootstrapperEngine;

            _confirmitCATIValidator = objectFactory.CreateConfirmitCATIValidatorObject();
            _prereqChecker = objectFactory.CreatePrereqCheckerObject();
        }

        public virtual void ValidateParameters()
        {
            // Verify, that all general parameters are defined            
            _bootstrapperEngine.VerifyInstallLocation(_parameters.InstallLocation);

            // Verify, that some parameters have correct value
            var errInfo = new StringBuilder();
            
            errInfo.AppendLine(_bootstrapperEngine.VerifyParameterValue("IS_FILE_LOGGING_ENABLED", _parameters.IsFileLoggingEnabled, new[] { "1", string.Empty }));
            errInfo.AppendLine(_bootstrapperEngine.VerifyParameterValue("IS_SET_RECYCLING_VALUE_TO_ZERO", _parameters.IsSetRecyclingValueToZero, new[] { "1", string.Empty }));
            errInfo.AppendLine(_bootstrapperEngine.VerifyParameterValue("MACHINE_CONFIG_CHANGING", _parameters.MachineConfigChanging, new[] { "DoNotChange", "SetAutoConfigTrue", "SetCustomSettings" }));
            errInfo.AppendLine(_bootstrapperEngine.VerifyParameterValue("DUMP_CREATION_OPTIONS", _parameters.DumpCreationOptions, new[] { "DoNotModifyCurrentOptions", "DoNotCreateDump", "CreateDump" }));

            if (errInfo.ToString().Replace("\r\n", string.Empty).Length > 0)
            {
                throw new MessageException(errInfo.ToString(), TraceEventType.Warning);
            }

            VerifySiteInfo(_parameters.WebSiteId, _parameters.WebSiteName);

            if (_parameters.DumpCreationOptions == "CreateDump")
            {
                VerifyDumpOptions(_parameters.ProcdumpFilePath, _parameters.ProcdumpLogFolderPath);
            }
        }

        public virtual void CheckPrerequisites()
        {
        }

        private void VerifyDumpOptions(string procDumpFilePath, string procDumpLogFolderPath)
        {
            if (string.IsNullOrEmpty(procDumpFilePath) || string.IsNullOrEmpty(procDumpLogFolderPath))
            {
                throw new MessageException(Resources.YouShouldSpecifyBothProcdumpParameters, TraceEventType.Warning);
            }

            string procDumpFileName = Path.GetFileName(procDumpFilePath);

            if (procDumpFileName == null || procDumpFileName.ToLower() != "procdump.exe" || !File.Exists(procDumpFilePath))
            {
                throw new MessageException(Resources.YouMustSpecifyAPathToProcDumpExeFile, TraceEventType.Warning);
            }

            if (!Directory.Exists(procDumpLogFolderPath))
            {
                throw new MessageException(Resources.NotExistedFolderInProcdumpLogFolderPathParameter, TraceEventType.Warning);
            }
        }

        private void VerifySiteInfo(string webSiteId, string webSiteName)
        {
            using (var sm = new ServerManager())
            {
                Site site = sm.Sites.FirstOrDefault(s => s.Name == webSiteName);
                if (site == null)
                {
                    throw new MessageException(string.Format(Resources.NotFoundWebSite, webSiteName), TraceEventType.Warning);
                }

                if (site.Id.ToString() != webSiteId)
                {
                    throw new MessageException(string.Format(Resources.DifferentIdForWebSite, webSiteName, site.Id), TraceEventType.Warning);
                }
            }
        }
    }
}
