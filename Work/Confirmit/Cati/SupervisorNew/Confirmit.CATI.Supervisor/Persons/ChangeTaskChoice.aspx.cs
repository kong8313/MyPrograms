using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Persons;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class ChangeTaskChoice : BaseForm
    {
        private List<int> m_IDs;

        // Orinal dialog size which is used when dialog is opened first time
        private const int m_OriginalWidth = 345;
        private const int m_OriginalHeight = 140;

        // Dialog size when user selects "Survey assignment" task choice
        private const int m_SurveyAssignmentWidth = 620;
        private const int m_SurveyAssignmentHeight = 480;

        // Dialog size when user selects "Choice" task choice
        private const int m_ChoiceWidth = 345;
        private const int m_ChoiceHeight = 285;

        // Dialog size when user selects "Choice" task choice
        private const int m_ChoiceWithSurveyAssignmentWidth = 650;
        private const int m_ChoiceWithSurveyAssignmentHeight = 610;

        /// <summary>
        /// Selected call ids
        /// </summary>
        protected List<int> IDs
        {
            get
            {
                if (m_IDs == null)
                {
                    string requestIDS = (String)ViewState["IDS"];
                    string[] ids = requestIDS.Split(',');
                    m_IDs = ids.Select(x => Int32.Parse(x)).ToList();
                }
                return m_IDs;
            }
        }

        /// <summary>
        /// True value means 'IDS' collection contains group's ids
        /// False value means 'IDS' collection contains user's ids
        /// </summary>
        public bool IsGroup
        {
            get
            {
                if (ViewState["IsGroup"] != null)
                {
                    return (bool)ViewState["IsGroup"];
                }
                return false;
            }
        }

        /// <summary>
        /// Gets selected task choice permissions
        /// </summary>
        public TaskChoicePermissions? SelectedTaskChoicePermissions
        {
            get
            {
                if (ddlTaskChoice.SelectedTaskChoice == AgentTaskChoiceMode.Choice)
                {
                    return m_SelectTaskChoicePermissions.Permissions;
                }

                return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["IDS"] = Request.Params["IDS"];
                ViewState["IsGroup"] = (Request.Params["IsGroup"] != null);
            }

            m_SelectTaskChoicePermissions.SurveySelectionPermissionChanged += new EventHandler(m_SelectTaskChoicePermissions_SurveySelectionPermissionChanged);
        }

        /// <summary>
        /// Executing task choice change for users/groups
        /// </summary>
        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("Supervisor.ChangeTaskChoice", DeadlockPriority.Supervisor))
                {
                    if (IsGroup)
                    {
                        foreach (int groupId in IDs)
                        {
                            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

                            IEnumerable<int> userIDs = PersonManager.GetAllPersons(groupId, callCenterId).Select(x => x.Id);
                            PersonService.ChangeTaskChoice(userIDs, ddlTaskChoice.SelectedTaskChoice, SelectedTaskChoicePermissions, false);
                        }
                    }
                    else
                    {
                        PersonService.ChangeTaskChoice(IDs, ddlTaskChoice.SelectedTaskChoice, SelectedTaskChoicePermissions, false);
                        int? autoSurveyId = surveyList.SelectedSurveyId;
                        if (autoSurveyId.HasValue)
                        {
                            foreach (int sid in IDs)
                            {
                                PersonService.SetAutomaticSurvey(sid, autoSurveyId.Value, false);
                            }
                        }
                    }

                    transaction.Commit();
                }

                PersonRepository.RefreshCache();
                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void SelectedChoiceChanged(object sender, EventArgs e)
        {
            choicePanel.Visible = false;
            surveyAssignmentPanel.Visible = false;
            dialog.PutActionButtonsInsideGridIfPossible = false;

            if (ddlTaskChoice.SelectedTaskChoice == AgentTaskChoiceMode.Choice)
            {
                choicePanel.Visible = true;

                ResizeToChoice();
            }
            else if (!IsGroup && ddlTaskChoice.SelectedTaskChoice == AgentTaskChoiceMode.CampaignAssignment)
            {
                surveyList.PersonId = null;
                surveyList.Bind();
                surveyAssignmentPanel.Visible = true;

                dialog.PutActionButtonsInsideGridIfPossible = true;

                ResizeToSurveyAssignment();
            }
            else
            {
                m_SelectTaskChoicePermissions.ClearSelection();

                ResizeToOriginal();
            }
        }

        void m_SelectTaskChoicePermissions_SurveySelectionPermissionChanged(object sender, EventArgs e)
        {                        
            dialog.PutActionButtonsInsideGridIfPossible = false;
            if (ddlTaskChoice.SelectedTaskChoice == AgentTaskChoiceMode.Choice)
            {
                bool selectedSurveyAssignmentPermission = m_SelectTaskChoicePermissions.Permissions.HasValue &&
                    (m_SelectTaskChoicePermissions.Permissions.Value & TaskChoicePermissions.SurveyAssignment) == TaskChoicePermissions.SurveyAssignment;

                if (!IsGroup && selectedSurveyAssignmentPermission)
                {
                    surveyList.PersonId = null;
                    surveyList.Bind();
                    surveyAssignmentPanel.Visible = true;
                    dialog.PutActionButtonsInsideGridIfPossible = true;

                    ResizeToChoiceWithSurveyAssignment();
                }
                else
                {
                    surveyAssignmentPanel.Visible = false;

                    ResizeToChoice();
                }
                              
            }          
        }

        /// <summary>
        /// Resizes window to original size used during first dialog opening.
        /// </summary>
        private void ResizeToOriginal()
        {
            ResizeWindow(m_OriginalWidth, m_OriginalHeight);
        }

        /// <summary>
        /// Resizs window to size used for "Survey assignment" mode.
        /// </summary>
        private void ResizeToSurveyAssignment()
        {
            ResizeWindow(m_SurveyAssignmentWidth, m_SurveyAssignmentHeight);
        }

        /// <summary>
        /// Resizs window to size used for "Survey assignment" mode.
        /// </summary>
        private void ResizeToChoiceWithSurveyAssignment()
        {
            ResizeWindow(m_ChoiceWithSurveyAssignmentWidth, m_ChoiceWithSurveyAssignmentHeight);
        }

        /// <summary>
        /// Resizs window to size used for "Choice" mode.
        /// </summary>
        private void ResizeToChoice()
        {
            ResizeWindow(m_ChoiceWidth, m_ChoiceHeight);
        }
    }
}
