using System;
using System.Threading;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class ConnectOrActivateDialer : BaseForm
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

        public ConnectOrActivateDialer()
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
                    ConnectAndActivateDialerRow.Visible = false;
                    ConnectDialerRow.Visible = false;
                    dialog.OKButton.Visible = false;
                    lblMessage.Visible = true;
                    lblMessage.Text = Strings.TelephonyIsNotEnabledForCompanyConnect;
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
                        DialerHint.Visible = false;
                        DialerState.Visible = true;
                        ConnectAndActivateDialerRow.Visible = false;
                        ConnectDialerRow.Visible = false;

                        if (isActivated)
                        {
                            dialog.OKButton.Text = Strings.DialerDialogOK;
                            DialerState.Text = Strings.DialerConnectedAndActivatedDialogText;
                            DialerState.CssClass = "attention--success";
                            dialog.OKButton.Visible = false;
                            lblMessage.Visible = true;
                        }
                        else
                        {
                            dialog.OKButton.Text = Strings.ActivateDialer;
                            DialerState.HintType = HintType.Warning;
                            DialerState.Text = Strings.DialerConnectedAndDeactivatedDialogText;
                            RegisterScriptBlock("window.parent.document.querySelector('.modal-dialog__title h3').innerText = '" +
                                                Strings.ActivateDialer + "';");
                            dialog.Title = Strings.ActivateDialer;
                        }
                    }
                    else
                    {
                        DialerState.HintType = HintType.Warning;
                        DialerState.Text = Strings.DialerDisconnectedAndDeactivated;
                    }
                }
            }

        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            bool success;
            
            if (IsConnected)
            {
                success = IsActivated || ActivateDialer();
            }
            else
            {
                success = EnableDialer();

                if (!success)
                {
                    return;
                }
                
                if (!rbConnectAndActivateDialer.Checked)
                {
                    CloseOverlay(true);
                }
                else
                {
                    // Wait until the cache of the BvDialers table is updated after receiving the RabbitMQ message
                    Thread.Sleep(1000);
                    success = ActivateDialer();
                }
            }

            if (success)
            {
                CloseOverlay(true);                
            }
        }

        private bool EnableDialer()
        {
            var result = false;

            try
            {
                result = _supervisorServiceClient.EnableDialer(DialerId);
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex, $"Failed to enable dialer, Id = {DialerId}");
                ShowClientMessage(ex.Message);
            }

            return result;
        }

        private bool ActivateDialer()
        {
            var result = _dialerAvailabilityManager.ActivateDialer(DialerId);

            if (!result)
            {
                ShowClientMessage(String.Format(Strings.DialerActivationFailedMessage, _dialersRepository.GetById(DialerId).Name, DialerId), true);
            }

            return result;
        }
    }
}