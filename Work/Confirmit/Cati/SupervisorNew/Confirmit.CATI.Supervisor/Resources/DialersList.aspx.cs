using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Resources
{
    /// <summary>
    /// Dialers list page
    /// </summary>
    public partial class DialersList : BaseForm
    {
        private bool _needUpdate;
        private IDialerStatusProvider _dialerStatusProvider;
        private IDialersRepository _dialersRepository;
        private IDialerSettings _dialerSettings;
        private IDialerService _dialerService;

        private ISupervisorServiceClient _supervisorServiceClient;

        public override string TopTitle => Strings.Dialers;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (_needUpdate)
            {
                grid.RefreshData();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
            _dialerStatusProvider = ServiceLocator.Resolve<IDialerStatusProvider>();
            _dialerSettings = ServiceLocator.Resolve<IDialerSettings>();
            _dialerService = ServiceLocator.Resolve<IDialerService>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();

            grid.GetPage += GetDialersListPage;
            grid.InitializeRow += InitializeRow;

            if (_dialersRepository.IsAnyDialerConfigured())
            {
                RegisterStartupScript("openDialerSettingsFrame()");
            }

            InitializeSearchingToolbar();

            if (!SupervisorPrincipal.Current.IsCatiDialerAdministrator)
            {
                grid.HideCommand("DialerFeaturesView");
                grid.HideCommand("ViewDialerLogs");
                grid.HideCommand("AddDialer");
                grid.HideCommand("EditDialer");
                grid.HideCommand("Delete");
                grid.OnDblClickCommand = string.Empty;
            }
        }

        /// <summary>
        /// Used to fill row's cells by some values.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Row event arguments</param>
        protected void InitializeRow(object sender, RowEventArgs e)
        {
            SetState("DialerActualState", e);
        }

        private void SetState(string key, RowEventArgs e)
        {
            var stateTextCell = e.Row.Items.FindItemByKey($"{key}Text");
            var stateCell = e.Row.Items.FindItemByKey(key);
            var state = (DialerStatus)Enum.Parse(typeof(DialerStatus), stateCell.Text);
            stateTextCell.CssClass = state.ToString();
            stateTextCell.Text = StringHelper.GetStringFromEnum(state);
        }

        private object GetDialersListPage(out int totalCount)
        {
            var dialers = _dialersRepository.GetAll();

            var list = from dialer in dialers
                       let actualState = (int)_dialerStatusProvider.GetDialerActualStatus(dialer.Id, dialer.IsActive, dialer.ReconnectionDuration != null, dialer.ExpectedState)
                       let dialType = ConvertToDialTypeDescription(dialer.DialTypeId)
                       let dialerConfigurationType = GetDialerConfigurationType(dialer.DialerConfigurationTypeId)
                       let dialerVersion = GetDialerVersion(dialer.Id)
                       select new
                       {
                           Id = dialer.Id,
                           Name = dialer.Name,
                           DialerActualState = actualState.ToString(),
                           DialType = dialType,
                           DialerConfigurationType = dialerConfigurationType,
                           DialerVersion = dialerVersion
                       };

            return BaseMethods.GetPage(list, grid.PageArguments, out totalCount);
        }

        private string GetDialerVersion(int dialerId)
        {
            try
            {
                return _supervisorServiceClient.GetDialerVersion(dialerId);
            }
            catch
            {
                return null;
            }
        }

        private string GetDialerConfigurationType(int? dialerConfigurationTypeId)
        {
            if (dialerConfigurationTypeId == null)
            {
                return string.Empty;
            }

            return ((DialerConfigurationType)dialerConfigurationTypeId.Value).ToString();
        }

        private string ConvertToDialTypeDescription(byte dialTypeId)
        {
            if (Enum.IsDefined(typeof(DialType), (int)dialTypeId))
            {
                switch ((DialType)dialTypeId)
                {
                    case DialType.Landline:
                        return "Automatic";
                    case DialType.Cellphone:
                        return "Manual";
                    case DialType.Assisted:
                        return "Assisted";
                }
            }
            return "Undefined";
        }

        private void InitializeSearchingToolbar()
        {
            InitStateColumnSearch("DialerActualState");
        }

        private void InitStateColumnSearch(string stateName)
        {
            var column = grid.Columns.FromKey($"{stateName}Text") as GeneralGridColumn;

            foreach (int value in Enum.GetValues(typeof(DialerStatus)))
            {
                var name = StringHelper.GetStringFromEnum((DialerStatus)value);
                column.Items.Add(new ListItem(name, value.ToString()));
            }
        }

        protected void DoUpdate(object sender, EventArgs e)
        {
            _needUpdate = true;
        }

        protected void DeleteDialer(object sender, EventArgs e)
        {
            var dialerId = grid.SelectedKeysInt.First();
            var dialer = _dialersRepository.GetById(dialerId);

            var dialerStatus = _dialerStatusProvider.GetDialerStatus(dialerId, dialer.IsActive);
            if (dialerStatus != DialerStatus.DisconnectedAndDeactivated)
            {
                AddUserMessage(Strings.DialerDeleteEditWarning);
                return;
            }

            var evt = new DeleteDialerEvent(dialer);

            _dialerService.DeleteDialerWithFeatures(dialerId);

            if (_dialersRepository.GetAll().Count == 0)
            {
                _dialerSettings.DialerType = DiallerType.NoDialler.ToString();
            }

            evt.Finish();
        }
    }
}
