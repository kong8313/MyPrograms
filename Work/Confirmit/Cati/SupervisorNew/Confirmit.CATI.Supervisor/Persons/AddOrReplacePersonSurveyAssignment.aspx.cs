using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.PageDataProviders.Persons;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.Persons;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class AddOrReplacePersonSurveyAssignment : BaseForm
    {
        [StoreInViewState]
        protected bool IsGroup;

        [StoreInViewState]
        protected bool ReplaceAssignment;

        private List<int> _interviewerOrGroupIds;
        
        /// <summary>
        /// Selected interviewer ids
        /// </summary>
        protected List<int> InterviewerOrGroupIds
        {
            get
            {
                if (_interviewerOrGroupIds == null)
                {
                    string requestIDS = (string)ViewState["IDS"];
                    string[] ids = requestIDS.Split(',');
                    _interviewerOrGroupIds = ids.Select(int.Parse).ToList();
                }
                return _interviewerOrGroupIds;
            }
        }
        
        private IPersonSurveyAssignmentPageProvider _assignmentPageProvider;

        private readonly IAssignmentManager _assignmentManager;
        private readonly ICallCenterProvider _callCenterProvider;
        private readonly ISetDialType _setDialType;
        private readonly IToggleSettings _toggleSettings;
        
        public AddOrReplacePersonSurveyAssignment()
        { 
            _assignmentManager = ServiceLocator.Resolve<IAssignmentManager>();
            _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
            _setDialType = ServiceLocator.Resolve<ISetDialType>();
            _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["IDS"] = Request.Params["IDS"] ?? Request["ObjectSid"];
                IsGroup = Boolean.Parse(Request["IsGroup"]);
                ReplaceAssignment = Convert.ToBoolean(Request["ReplaceAssignment"]);
            }

            dialogControl.OKButton.Text = ReplaceAssignment ? "Replace assignments" : "Add assignments";

            _assignmentPageProvider = new PersonToSurveyAssignmentPageProviderFactory().GetProvider(ReplaceAssignment);

            surveyListGrid.HintText = _assignmentPageProvider.GetPageHint();

            surveyListGrid.GetPage = GetPage;
            if (InterviewerOrGroupIds.Count == 1)
            {
                surveyListGrid.GridName = IsGroup
                    ? PersonGroupRepository.GetById(InterviewerOrGroupIds[0]).Name
                    : PersonRepository.GetById(InterviewerOrGroupIds[0]).Name;
            }
            else
            {
                surveyListGrid.GridName = IsGroup
                    ? $"{InterviewerOrGroupIds.Count} selected groups"
                    : $"{InterviewerOrGroupIds.Count} selected interviewers";
            }

            DialTypeTable.Visible = _toggleSettings.ShowDialType;

            dialogControl.OKButton.OnClientClick = _assignmentPageProvider.GetSaveConfirmation();

        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                List<int> selectedSurveys = surveyListGrid.SelectedKeysInt;
                if (selectedSurveys.Count == 0)
                {
                    CloseOverlay();
                    return;
                }

                using (var transaction = new DatabaseTransactionScope("AssignResourcesToSurvey", DeadlockPriority.Supervisor))
                {
                    _assignmentPageProvider.PerformAssignment(InterviewerOrGroupIds, IsGroup, selectedSurveys, User.Name);

                    if (_toggleSettings.ShowDialType && ddlDialType.SelectedDialType != null)
                    {
                        _setDialType.Set(ddlDialType.SelectedDialType.Value, InterviewerOrGroupIds);
                        dialogControl.RefreshListFrame();
                    }

                    transaction.Commit();
                }

                // Show warning for predictive surveys.
                List<int> interviewerSids = IsGroup ? PersonManager.GetAllPersons(InterviewerOrGroupIds, _callCenterProvider.GetCurrentId()).Select(x => x.Id).ToList() : InterviewerOrGroupIds;
                string warningText = new PredictiveHelper().GetPredictiveSurveyAssignmentWarning(selectedSurveys, interviewerSids);
                if (!String.IsNullOrEmpty(warningText))
                {
                    ShowClientMessage(warningText, true);
                }

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        /// <summary>
        /// Returns page of information to show in grid.
        /// </summary>
        protected object GetPage(out int totalCount)
        {
            List<SurveyInfoItem> list;
            if (InterviewerOrGroupIds.Count == 1)
            {
                list = cbRecent.Checked ?
                    _assignmentManager.RemoveAssignedSurveysFromList(SurveyManager.GetRecentSurveys(User.Name, string.Empty), InterviewerOrGroupIds[0], User.Name, IsGroup) :
                    _assignmentPageProvider.GetSurveysListForAssignment(InterviewerOrGroupIds[0], User.Name, IsGroup);
            }
            else
            {
                list = cbRecent.Checked ?
                    SurveyManager.GetRecentSurveys(User.Name, string.Empty) :
                    SurveyManager.GetSurveys(User.Name, String.Empty);
            }

            return BaseMethods.GetPage(list, surveyListGrid.PageArguments, out totalCount);
        }
    }
}