using System;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.SurveysInterviewersSelection
{
    public partial class GroupsInterviewersSelectionPage : BaseForm
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                doubleGrid.SelectedIds = SessionVariables.TaskListSelectedInterviewersIds;
            }
        }

        protected void Save(object sender, EventArgs e)
        {
            SessionVariables.TaskListSelectedInterviewersIds = doubleGrid.SelectedIds;

            CloseOverlay(true);
        }
    }
}