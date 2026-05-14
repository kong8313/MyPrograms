using System;
using System.Threading;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class DisconnectOrDeactivateDialer : BaseForm
    {
        private readonly ISupervisorServiceClient _supervisorServiceClient;
        private readonly IDialersRepository _dialersRepository;
        private readonly IDialerAvailabilityManager _dialerAvailabilityManager;
        private readonly IAuthoringService _authoringService;
        private readonly ICompanyInfo _companyInfo;

        private int DialerId
        {
            get
            {
                return int.Parse(ViewState["id"].ToString());
            }
            set
            {
                ViewState["id"] = value;
            }
        }

        private bool IsActivated
        {
            get
            {
                return bool.Parse(ViewState["isActivated"].ToString());
            }
            set
            {
                ViewState["isActivated"] = value;
            }
        }

        private bool IsConnected
        {
            get
            {
                return bool.Parse(ViewState["isConnected"].ToString());
            }
            set
            {
                ViewState["isConnected"] = value;
            }
        }
        public DisconnectOrDeactivateDialer()
        {
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _dialerAvailabilityManager = ServiceLocator.Resolve<IDialerAvailabilityManager>();
            _authoringService = ServiceLocator.Resolve<IAuthoringService>();
            _companyInfo = ServiceLocator.Resolve<ICompanyInfo>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!_authoringService.IsCompanyTelephonyEnabled(_companyInfo.CompanyId))
                {
                    DialerHint.Visible = false;
                    DialerState.Visible = false;
                    DisconnectAndDeactivateDialerRow.Visible = false;
                    DeactivateDialerRow.Visible = false;
                    TerminateTasksLine.Visible = false;
                    TerminateTasksRow.Visible = false;
                    dialog.OKButton.Visible = false;
                    lblMessage.Visible = true;
                    lblMessage.Text = Strings.TelephonyIsNotEnabledForCompanyDisconnect;
                }
                else
                {
                    var dialerId = int.Parse(Request.Params["Id"]);
                    DialerId = dialerId;

                    var isDialerInitialized = _supervisorServiceClient.IsDialerOperational(dialerId);
                    IsConnected = isDialerInitialized;

                    var isActivated = _dialersRepository.GetById(dialerId).IsActive;
                    IsActivated = isActivated;

                    if (isDialerInitialized)
                    {
                        if (!isActivated)
                        {
                            DialerState.Visible = true;
                            DeactivateDialerRow.Visible = false;
                            TerminateTasksLine.Visible = false;
                            DisconnectAndDeactivateDialerRow.Visible = false;

                            DialerHint.Text = Strings.DialerConnectedAndDeactivatedHintText;
                            dialog.OKButton.Text = Strings.DisconnectDialer;
                            DialerState.Text = Strings.DialerConnectedAndDeactivatedDialogText;
                            DialerState.HintType = HintType.Warning;
                            RegisterScriptBlock("window.parent.document.querySelector('.modal-dialog__title h3').innerText = '" +
                                                Strings.DisconnectDialer + "';");
                            
                        }
                    }
                    else
                    {
                        DialerHint.Visible = false;
                        DialerState.Visible = true;
                        lblMessage.Visible = true;
                        DeactivateDialerRow.Visible = false;
                        TerminateTasksLine.Visible = false;
                        TerminateTasksRow.Visible = false;
                        DisconnectAndDeactivateDialerRow.Visible = false;

                        dialog.OKButton.Text = Strings.DialerDialogOK;
                        DialerState.Text = Strings.DialerDisconnectedAndDeactivatedDialogText;
                        dialog.OKButton.Visible = false;
                    }
                }
            }
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            var disconnectAndDeactivate = rbDisconnectAndDeactivateDialer.Checked;

            if (IsConnected)
            {
                if (IsActivated)
                {
                    DeactivateDialer();
                }

                if (disconnectAndDeactivate)
                {
                    // Wait until the cache of the BvDialers table is updated after receiving the RabbitMQ message
                    Thread.Sleep(1000);
                    DisableDialer();
                }
            }

            var terminateTasks = cbTerminateTasks.Checked;

            if (terminateTasks)
            {
                TerminateTasks();
            }

            CloseOverlay(true);
        }

        private void TerminateTasks()
        {
            try
            {
                _supervisorServiceClient.TerminateTasksByDialerId(DialerId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("SupervisorServiceClient.TerminateTasksByDialerId(), Failed to terminate tasks by dialer, dialerId = {0}, ex: {1}", DialerId, ex);
            }
        }

        private void DisableDialer()
        {
            try
            {
                _supervisorServiceClient.DisableDialer(DialerId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("SupervisorServiceClient.DisableDialer(), Failed to disable dialer, dialerId = {0}, ex: {1}", DialerId, ex);
            }
        }

        private void DeactivateDialer()
        {
            _dialerAvailabilityManager.DeactivateDialer(DialerId);
        }
    }
}