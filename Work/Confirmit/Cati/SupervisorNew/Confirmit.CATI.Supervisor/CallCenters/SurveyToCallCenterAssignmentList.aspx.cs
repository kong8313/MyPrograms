using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes.CallCenters;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;

namespace Confirmit.CATI.Supervisor.CallCenters
{
    public partial class SurveyToCallCenterAssignmentList : CallCenterBaseForm
    {
        private readonly ISurveyToCallCenterAssignmentProvider _assignmentProvider =
            ServiceLocator.Resolve<ISurveyToCallCenterAssignmentProvider>();
 
        public override string TopTitle
        {
            get { return Strings.SurveysAssignmentsInCallCenters; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _assignmentsGrid.GetPage += (out int totalCount) => _assignmentProvider.GetPage(User.Name, _assignmentsGrid.PageArguments, out totalCount);

            FillCallCenterSearchFilter();
        }

        private void FillCallCenterSearchFilter()
        {
            var column = _assignmentsGrid.Columns.FromKey("CallCenters") as GeneralGridColumn;

            CallCenterRepository.GetAll().ForEach(
                callCenter => column.Items.Add(new ListItem(callCenter.Name, callCenter.ID.ToString(CultureInfo.InvariantCulture)))
            );
        }
    }
}