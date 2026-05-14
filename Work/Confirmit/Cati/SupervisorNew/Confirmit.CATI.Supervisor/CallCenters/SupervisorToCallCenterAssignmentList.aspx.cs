using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallCenters;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;

namespace Confirmit.CATI.Supervisor.CallCenters
{
    public partial class SupervisorToCallCenterAssignmentList : CallCenterAdminForm
    {
        private readonly ISuperToCallCenterAssignmentProvider _dataProvider =
            ServiceLocator.Resolve<ISuperToCallCenterAssignmentProvider>();

        public override string TopTitle
        {
            get
            {
                return Strings.SupervisorAssignments;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                ServiceLocator.Resolve<ICachedConfirmitSupervisorProvider>().ClearCache();
            }

            _assignmentsGrid.GetPage += (out int totalCount) =>
            {
                var list = _dataProvider.GetAllAssignments();

                var args = new PagingArgs(_assignmentsGrid.PageIndex,
                                          _assignmentsGrid.PageSize,
                                          _assignmentsGrid.SortedColumnKey,
                                          _assignmentsGrid.SortIndicatorAsc,
                                          _assignmentsGrid.SearchParameterCollection);

                return BaseMethods.GetPage(list, args, out totalCount);
            };

            FillCallCenterSearchFilter();
        }

        private void FillCallCenterSearchFilter()
        {
            GeneralGridColumn column = _assignmentsGrid.Columns.FromKey("CallCenterName") as GeneralGridColumn;

            CallCenterRepository.GetAll().ForEach(
                callCenter => column.Items.Add(new ListItem(callCenter.Name, callCenter.ID.ToString(CultureInfo.InvariantCulture)))
            );
        }
    }
}