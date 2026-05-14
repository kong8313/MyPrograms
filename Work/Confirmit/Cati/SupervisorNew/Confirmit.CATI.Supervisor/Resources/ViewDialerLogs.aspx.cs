using System;
using System.IO;
using System.Linq;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class ViewDialerLogs : BaseForm
    {
        protected internal const string DefaultZipArchiveFileName = "log.zip";

        [StoreInViewState]
        protected int DialerId;

        private ISupervisorServiceClient _supervisorServiceClient;

        protected void Page_Load(object sender, EventArgs e)
        {
            dialogControl.OKButton.Visible = false;
            dialogControl.CancelButton.InnerText = "Close";
            if (!IsPostBack)
            {
                DialerId = int.Parse(Request["Id"]);
            }
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            var availFunctionality = _supervisorServiceClient.GetAvailableExtendedFunctionality(DialerId);
            if (availFunctionality.IsLogGetterSupported)
            {
                grid.GetPage = GetPage;
            }
            else
            {
                grid.Visible = false;
                dialerLogsNotAvailableHint.Visible = true;
                RegisterScriptBlock("window.parent.document.querySelector('body > div').style.height = '160px'");
                RegisterScriptBlock(
                    "window.parent.document.querySelector('body > div > div > iframe').style.height = '130px'");
            }
        }

        /// <summary>
        /// Returns page of information to show in grid.
        /// </summary>
        protected object GetPage(out int totalCount)
        {
            var list = _supervisorServiceClient.GetLogFiles(DialerId);
            return BaseMethods.GetPage(list, grid.PageArguments, out totalCount);
        }

        protected void DownloadLog(object sender, EventArgs e)
        {
            var filename = grid.SelectedKeys.First();
            if (string.IsNullOrWhiteSpace(filename))
            {
                AddUserMessage(Strings.NothingToExport);
                return;
            }

            var buffer = _supervisorServiceClient.GetLogFileBodyZipped(DialerId, filename);
            if (buffer == null)
            {
                AddUserMessage(Strings.NothingToExport);
                return;
            }

            FileToClientSender.SendBuffer(buffer, GenerateZipArchiveFileName(filename));
        }

        private static string GenerateZipArchiveFileName(string filename)
        {
            return Path.ChangeExtension(filename, ".zip") ?? DefaultZipArchiveFileName;
        }
    }
}