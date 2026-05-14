using System;
using Confirmit.CATI.Core;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Classes.CallManagement;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    public partial class ChangePriority : BaseActionForm
    {
        private int Priority
        {
            get
            {
                return wnePriority.ValueInt;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                if (SelectionType == CallSelectionType.Selected)
                {
                    wnePriority.Value = Calculator.Calculate(SurveyID, IDS, entity => entity.Priority, 1);
                }
            }
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                LegacySupervisorMetrics.OnCallManagementAction("ChangePriority");
                var operationEntity = CallManager.ChangeCallsPriority(SurveyID, Priority, BatchParameters);

                Redirect(operationEntity);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

    }
}
