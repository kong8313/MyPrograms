using System;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.CallCenters.Controls
{
    public partial class CallCentersList : BaseWUC
    {
        private ICallCenterRepository _callCenterRepository;
        private IChangeCallCenter _callCenterChanger;
        private ICallCenterProvider _callCenterProvider;
        private int _currentCallCenterId;

        public bool EnableMultiSelection
        {
            get { return !_callCenters.HideSelectedColumn; }
            set { _callCenters.HideSelectedColumn = !value; }
        }

        public RightToolbarButtonsConfiguration RightToolbarButtons
        {
            get { return _callCenters.RightToolbarButtons; }
            set { _callCenters.RightToolbarButtons = value; }
        }

        public int[] SelectedCallCenterIds
        {
            get { return _callCenters.SelectedKeys.Select(Int32.Parse).ToArray(); }
        }

        public string HintText
        {
            get { return _callCenters.HintText; }
            set { _callCenters.HintText = value; }
        }

        public bool ReadOnly { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
            _callCenterChanger = ServiceLocator.Resolve<IChangeCallCenter>();
            _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();

            _callCenters.GetPage += (out int totalCount) =>
            {
                _currentCallCenterId = _callCenterProvider.GetCurrentId();
                var list = _callCenterRepository.GetAllWithDialerIds();

                var args = new PagingArgs(_callCenters.PageIndex,
                                          _callCenters.PageSize,
                                          _callCenters.SortedColumnKey,
                                          _callCenters.SortIndicatorAsc,
                                          _callCenters.SearchParameterCollection);

                return BaseMethods.GetPage(list, args, out totalCount);
            };
            _callCenters.InitializeRow += InitializeCallCenterRow;

            if (ReadOnly)
            {
                _callCenters.HideCommand("New");
                _callCenters.HideCommand("Edit");
                _callCenters.HideCommand("SetDefault");
                _callCenters.HideCommand("Delete");
            }

            if (!SupervisorPrincipal.Current.IsCatiAdministratorOrPros && !SupervisorPrincipal.Current.IsSystemProjectAdministrator)
            {
                _callCenters.HideCommand("New");
                _callCenters.HideCommand("Edit");
                _callCenters.HideCommand("Delete");
                _callCenters.OnDblClickCommand = "";
            }
        }

        private void InitializeCallCenterRow(object sender, Infragistics.Web.UI.GridControls.RowEventArgs e)
        {
            var callCenter = (BvCallCenterEntity)e.Row.DataItem;
            if (callCenter.IsDefault)
            {
                e.Row.CssClass += " DefaultCallCenter";
            }

            e.Row.Items.FindItemByKey("IsCurrent").Text = (callCenter.ID == _currentCallCenterId ? Strings.Yes : "");
        }

        protected void SetDefault(object sender, EventArgs e)
        {
            try
            {
                _callCenterChanger.Change(_callCenters.SelectedKeysInt[0]);

                _callCenters.ClearSelectedKeys();
                _callCenters.RefreshColumns();

                Page.RegisterStartupScript("refreshCallCenterInfo();");
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}