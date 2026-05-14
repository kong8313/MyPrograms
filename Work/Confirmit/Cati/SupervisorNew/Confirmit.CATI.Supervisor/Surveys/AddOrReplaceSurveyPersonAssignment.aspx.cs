using System;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes.PageDataProviders.Surveys;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.Persons;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class AddOrReplaceSurveyPersonAssignment : SurveyFormBase
    {
        private readonly ISetDialType _setDialType;
        private readonly IToggleSettings _toggleSettings;

        [StoreInViewState]
        protected int SurveyId;

        [StoreInViewState]
        protected bool ReplaceAssignment;

        private ISurveyPersonAssignmentPageProvider assignmentPageProvider;

        public AddOrReplaceSurveyPersonAssignment()
        {
            _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
            _setDialType = ServiceLocator.Resolve<ISetDialType>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SurveyId = Int32.Parse(Request["ID"]);
                ReplaceAssignment = Convert.ToBoolean(Request["ReplaceAssignment"]);
            }

            dialogControl.OKButton.Text = ReplaceAssignment ? "Replace assignments" : "Add assignments";

            assignmentPageProvider = new SurveyToPersonAssignmentPageProviderFactory().GetProvider(ReplaceAssignment);

            dialogControl.OKButton.OnClientClick = assignmentPageProvider.GetSaveConfirmation();
            userList.HintText = assignmentPageProvider.GetPageHint();
            userList.Data = assignmentPageProvider.GetInterviewersListForAssignment(SurveyId);
            userList.ListName = SurveyService.GetFormattedSurveyName(SurveyId);
        }

        /// <summary>
        /// Handles "OK" button click event.
        /// </summary>
        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (userList.SelectedKeys.Length == 0)
                {
                    CloseOverlay();
                    return;
                }

                using (var transaction = new DatabaseTransactionScope("AssignResourceToSurvey", DeadlockPriority.Supervisor))
                {
                    assignmentPageProvider.PerformAssignment(SurveyId, userList.SelectedInterviewersIDs);

                    if ((_toggleSettings.EnableAgentAssistedDialling || _toggleSettings.EnableTCPA) && userList.DialTypeId != null)
                    {
                        _setDialType.Set(userList.DialTypeId.Value, userList.SelectedInterviewersIDs);
                    }

                    transaction.Commit();
                }

                // Show warning for predictive survey.
                if (SurveyManager.GetDialingMode(SurveyId) == DialingMode.Predictive)
                {
                    string warningText = new PredictiveHelper().GetPredictiveSurveyAssignmentWarning(SurveyId.CreateArray(), userList.AllInterviewersIDs);
                    if (!String.IsNullOrEmpty(warningText))
                    {
                        ShowClientMessage(warningText, true);
                    }
                }

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}