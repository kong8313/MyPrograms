using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class PersonProperties : BaseForm
    {
        private readonly ICallCenterProvider _callCenterProvider;
        private readonly ICallGroupRepository _callGroupRepository;
        private readonly ICallGroupSettings _callGroupSettings;
        private readonly IToggleSettings _toggleSettings;
        private readonly IConsoleSettings _consoleSettings;
        private readonly ISupervisorServiceClient _supervisorServiceClient;
        private readonly IInterviewerPropertiesSettings _interviewerPropertiesSettings;
        
        public PersonProperties()
        {
            _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
            _callGroupRepository = ServiceLocator.Resolve<ICallGroupRepository>();
            _callGroupSettings = ServiceLocator.Resolve<ICallGroupSettings>();
            _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _consoleSettings = ServiceLocator.Resolve<IConsoleSettings>();
            _interviewerPropertiesSettings = ServiceLocator.Resolve<IInterviewerPropertiesSettings>();
        }

        public override string Title
        {
            get { return Strings.PersonProperties; }
        }

        [StoreInViewState]
        protected int ParentId;

        [StoreInViewState]
        protected int PersonSid;

        /// <summary>
        /// Gets flag indicated is new person created or existent is edited
        /// </summary>
        protected bool IsNewPerson
        {
            get
            {
                return PersonSid == 0;
            }
        }

        public int CallGroupId
        {
            get
            {
                int callGroupId;
                Int32.TryParse(ddlCallGroups.SelectedValue, out callGroupId);

                return callGroupId;
            }
        }

        /// <summary>
        /// Gets selected auto-survey Id 
        /// </summary>
        /// <remarks>
        /// Uses when user selects "Survey Assignment" mode
        /// </remarks>
        private int? SelectedAutoSurveyId
        {
            get
            {
                if (String.IsNullOrEmpty(m_AutoSurveyId.Value) == false)
                {
                    return Int32.Parse(m_AutoSurveyId.Value);
                }

                return null;
            }
            set
            {
                m_AutoSurveyId.Value = value.HasValue ? value.ToString() : String.Empty;
            }
        }

        /// <summary>
        /// Gets user login
        /// </summary>
        private string UserLogin
        {
            get
            {
                return tbxLogin.Text.Trim();
            }
        }

        /// <summary>
        /// Gets user description  
        /// </summary>
        private string Description
        {
            get
            {
                return tbxDescription.Text.Trim();
            }
        }

        /// <summary>
        /// Gets user's display name  
        /// </summary>
        private string FullName
        {
            get
            {
                return tbxDisplayName.Text.Trim();
            }
        }
        
        /// <summary>
        /// Returns true if person has task choice 'Choice' with allowed 'SurveySelection' permission
        /// </summary>
        private bool HasPersonChoiceModeWithSurveySelection
        {
            get
            {
                if (m_SelectTaskChoicePermissions.Permissions != null)
                {
                    if ((m_SelectTaskChoicePermissions.Permissions & TaskChoicePermissions.SurveyAssignment) ==
                        TaskChoicePermissions.SurveyAssignment)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            stateChecker.AddSaveButton(btnSaveProperties);
            stateChecker.AddSaveButton(btnSaveMembership);

            if (!IsPostBack)
            {
                ListEnumInitializer.FillListControlWithEnumValues<PersonAssignmentListMode>(ddlAssignmentListMode);

                ParentId = Request["ParentID"] != null
                    ? Convert.ToInt32(Request["ParentID"])
                    : PersonGroupService.RootGroupId;

                if (Request["PersonSID"] != null)
                {
                    PersonSid = Convert.ToInt32(Request["PersonSID"]);
                    var isIvrAgent = PersonRepository.GetById(PersonSid).Type == (byte)AgentType.IvrAgent;
                    if (isIvrAgent)
                    {
                        var key = MaintainTabHelper.GetTabKey(ViewWithTabs.PersonProperties);
                        if (key == Strings.Properties || key == string.Empty)
                        {
                            MaintainTabHelper.SetTabKey(ViewWithTabs.PersonProperties, Strings.Membership);
                        }

                        var url = string.Format("IvrAgentProperties.aspx?ParentID={0}&PersonSID={1}", ParentId, PersonSid);
                        Redirect(url);
                        return;
                    }
                }

                InitCallGroupDropdown();

                InitAttributes();
                
                if (!IsNewPerson)
                {
                    BindExistingPersonProperties();
                }

                UpdateTaskChoicePanels();
            }

            if (IsNewPerson)
            {
                stateChecker.Disabled = true;
                dialogControl.OKButton.Click += SaveHandler;
                dialogControl.Mode = DialogWindowMode.Modal;
            }

            if (!IsNewPerson)
            {
                AssignmentList.SID = PersonSid;

                //tab remaining has to work only for existen person
                tabs.ClientEvents.SelectedIndexChanged = "SelectedIndexChanged";

                ddlTaskChoice.SelectedIndexChanged += ddlTaskChoice_SelectedIndexChanged;

                AssignmentList.AutomaticSurveyChanged += AssignmentList_AutomaticSurveyChanged;

                AssignmentList.EnableAutomaticSurveyButton(ddlTaskChoice.SelectedTaskChoice == AgentTaskChoiceMode.CampaignAssignment);

                lbtnChangePassword.OnClientClick = String.Format("changePasswordDialog({0}); return false;", PersonSid);
            }

            FillMembershipLists();

            SetControlsVisibility();
        }

        private void InitAttributes()
        {
            var attributesList = _interviewerPropertiesSettings.AttributesList;
            
            if (string.IsNullOrWhiteSpace(attributesList))
            {
                tabs.FindTabFromKey("Attributes").Hidden = true;
                return;
            }
            
            tabs.FindTabFromKey("Attributes").Hidden = false;
            
            var attributes = attributesList.Split(',', ';')
                .Select(a => a.Trim())
                .Where(a => !string.IsNullOrEmpty(a))
                .Take(5)
                .ToArray();
            
            var attributeControls = new[]
            {
                new { Label = lblAttribute1, TextBox = tbxAttribute1 },
                new { Label = lblAttribute2, TextBox = tbxAttribute2 },
                new { Label = lblAttribute3, TextBox = tbxAttribute3 },
                new { Label = lblAttribute4, TextBox = tbxAttribute4 },
                new { Label = lblAttribute5, TextBox = tbxAttribute5 }
            };

            for (int i = 0; i < attributeControls.Length; i++)
            {
                if (i < attributes.Length)
                {
                    attributeControls[i].Label.InnerText = attributes[i];
                    attributeControls[i].Label.Visible = true;
                    attributeControls[i].TextBox.Visible = true;
                }
                else
                {
                    attributeControls[i].Label.Visible = false;
                    attributeControls[i].TextBox.Visible = false;
                }
            }
        }

        private void InitCallGroupDropdown()
        {
            if (_callGroupSettings.Enabled)
            {
                trCallGroup.Visible = true;
                ddlCallGroups.Items.Clear();
                ddlCallGroups.Items.Add(new ListItem { Text = Strings.None, Value = "0" });
                ddlCallGroups.Items.AddRange(
                    _callGroupRepository.GetAllGroups()
                        .Select(x => new ListItem { Text = x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture) })
                        .ToArray());
            }
        }

        private void SetControlsVisibility()
        {
            dialogControl.OKButton.Visible = IsNewPerson;
            pnlNewPassword.Visible = IsNewPerson;
            trConfirmPassword.Visible = IsNewPerson;

            ddlTaskChoice.AutoPostBack = !IsNewPerson;
            pnlChangePassword.Visible = !IsNewPerson;
            btnSaveMembership.Visible = !IsNewPerson;
            btnSaveProperties.Visible = !IsNewPerson;
            statusRow.Visible = !IsNewPerson;

            dialTypeRow.Visible = _toggleSettings.ShowDialType;
            typeOfDialerSSO.Visible = _consoleSettings.EnableSoftphoneIntegration;

            if (IsNewPerson)
            {
                rowAutomaticSurvey.Visible = false;
                pnlTaskChoicePermissions.Visible = true;
                trIdRow.Visible = false;
                tabs.FindTabFromKey("Assignment").Hidden = true;
                dialogControl.HideButtons = false;
            }
        }

        private void BindExistingPersonProperties()
        {
            BvPersonEntity person = PersonRepository.GetById(PersonSid);

            lblID.Text = person.SID.ToString();
            tbxLogin.Text = person.Name;
            tbxDescription.Text = person.Description;
            dialogControl.Title = string.Format(Strings.InterviewerProperties, person.Name);
            lbPageInfo.Text = string.Format(Strings.InterviewerProperties, person.Name);
            lblAttributes.Text = string.Format(Strings.InterviewerProperties, person.Name);
            lblMembership.Text = string.Format(Strings.InterviewerProperties, person.Name);
            lblStatus.Text = person.IsLocked ? Strings.PersonIsLockedMessage : Strings.PersonIsNotLockedMessage;
            ddlAssignmentListMode.SelectedIndex = person.AssignmentsListMode;
            tbLocation.Text = person.Location;
            ddlCallGroups.SelectedValue = person.CallGroupID.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
            ddlDialType.SelectedValue = person.DialTypeId.ToString();
            ddlSSOIntegration.SelectedIndex = person.EnableSoftphoneIntegration ? 1 : 0;
            tbxDisplayName.Text = person.FullName;
            tbxAttribute1.Text = person.Attribute1;
            tbxAttribute2.Text = person.Attribute2;
            tbxAttribute3.Text = person.Attribute3;
            tbxAttribute4.Text = person.Attribute4;
            tbxAttribute5.Text = person.Attribute5;
            
            if (person.IsLocked)
            {
                pnlLockedDate.Visible = true;
                lblLockedDate.Text = person.LockedDate.GetValueOrDefault().ToString("G");
            }

            var mode = (AgentTaskChoiceMode)Enum.ToObject(typeof(AgentTaskChoiceMode), person.ManualSelection);

            var permissions = (TaskChoicePermissions?)person.AllowedChoices;

            if (permissions != null)
            {
                ddlTaskChoice.SelectedTaskChoice = AgentTaskChoiceMode.Choice;
                m_SelectTaskChoicePermissions.Permissions = permissions.Value;
            }
            else
            {
                ddlTaskChoice.SelectedTaskChoice = mode;
            }

            string tabKey = MaintainTabHelper.GetTabKey(ViewWithTabs.PersonProperties);

            if (String.IsNullOrEmpty(tabKey) == false)
            {
                tabs.SelectTabByKey(tabKey);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (IsNewPerson)
            {
                ddlTaskChoice.Attributes["onchange"] = "taskChoiceChanged();";
                //Synchronize selected task choice with visibility of task choice permission panel
                RegisterStartupScript("taskChoiceChanged();");
            }
            else
            {
                lbtnChangeAutoSurvey.Attributes["onclick"] =
                        String.Format("showSelectAutomaticSurveyDialog('{0}','{1}','{2}','{3}'); return false;",
                            PersonSid,
                            Strings.SelectAutomaticSurvey,
                            650,
                            550
                        );

                if (ddlTaskChoice.SelectedTaskChoice == AgentTaskChoiceMode.Choice)
                {
                    m_SelectTaskChoicePermissions.PermissionChangedClientHandler = "taskChoicePermissionChanged";

                    //Synchronize selected 'SurveySelection' permission with visibility of survey assignement panel
                    RegisterStartupScript(string.Format("taskChoicePermissionChanged({0}, {1}, true);",
                                                 (int)TaskChoicePermissions.SurveyAssignment,
                                                 HasPersonChoiceModeWithSurveySelection.ToString().ToLower()));
                }
                else
                {
                    RegisterStartupScript(String.Format("showCallGroupWarning({0})",
                                          (ddlTaskChoice.SelectedTaskChoice != AgentTaskChoiceMode.CampaignAssignment).ToString(CultureInfo.InvariantCulture).ToLower()));
                }
            }
        }

        /// <summary>
        /// Occurs when user changes task choice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ddlTaskChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            stateChecker.MarkAsChanged();
            UpdateTaskChoicePanels();
        }

        protected void SaveHandler(object sender, EventArgs e)
        {
            try
            {
                if (ValidateData() == false)
                {
                    return;
                }

                var parentGroups = new List<int>(membershipLists.RightValues);
                if (parentGroups.Count == 0)
                {
                    parentGroups.Add(ParentId);
                }

                var callCenterId = _callCenterProvider.GetCurrentId();
                var attributes = new [] { tbxAttribute1.Text, tbxAttribute2.Text, tbxAttribute3.Text, tbxAttribute4.Text, tbxAttribute5.Text };
                
                _supervisorServiceClient.CreateOrUpdatePerson(
                    callCenterId,
                    PersonSid,
                    UserLogin,
                    Description,
                    FullName,
                    tbxPassword.Text.Trim(),
                    ddlTaskChoice.SelectedTaskChoice,
                    (PersonAssignmentListMode)ddlAssignmentListMode.SelectedIndex,
                    m_SelectTaskChoicePermissions.Permissions,
                    parentGroups,
                    SelectedAutoSurveyId,
                    CallGroupId,
                    tbLocation.Text,
                    ddlDialType.SelectedDialType ?? DialType.Landline,
                    AgentType.LiveAgent,
                    Convert.ToBoolean(ddlSSOIntegration.SelectedIndex),
                    attributes);

                ClearMembershipLists();
                FillMembershipLists();

                UpdateTaskChoicePanels();

                stateChecker.MarkAsUnchanged();

                dialogControl.RefreshListFrameIfDialogNonModal();
                RefreshInfoFrame();

                if (IsNewPerson)
                {
                    CloseOverlay(true);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void AssignmentList_AutomaticSurveyChanged(object sender, EventArgs e)
        {
            SelectedAutoSurveyId = null;
            UpdateTaskChoicePanels();
        }

        /// <summary>
        /// Validates user provided data 
        /// </summary>
        private bool ValidateData()
        {
            if (IsNewPerson)
            {
                if (!ValidatePassword(tbxPassword.Text, tbxConfirm.Text))
                {
                    return false;
                }
            }

            int personSid = PersonManager.LookupPersonName(UserLogin);
            if (personSid != 0 && (IsNewPerson || (!IsNewPerson && personSid != PersonSid)))
            {
                AddUserMessage(string.Format(Strings.ErrorInterviewerAlreadyExists, UserLogin));
                return false;
            }

            if (ddlTaskChoice.SelectedTaskChoice == AgentTaskChoiceMode.Choice &&
               m_SelectTaskChoicePermissions.Permissions == null)
            {
                AddUserMessage("Err_YouShouldSpecifyAtLeastOneTaskChoice");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the password.
        /// </summary>
        /// <param name="password">Password.</param>
        /// <param name="confirm">Confirmed password.</param>
        /// <returns>true, if password is valid; otherwise false.</returns>
        private bool ValidatePassword(string password, string confirm)
        {
            if (String.IsNullOrEmpty(password))
            {
                AddUserMessage(Strings.Err_PasswordIsEmpty);
                return false;
            }

            if (password != confirm)
            {
                AddUserMessage(Strings.Err_PasswordsDontMatch);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Binds data about person automatic survey.
        /// </summary>
        ///<remarks>
        ///Used only for existent person
        /// </remarks>
        private void BindAutomaticSurvey()
        {
            BvSurveyEntity survey = null;
            lblAutoSurveyName.Text = String.Empty;
            btnClearAutomaticSurvey.Style["visibility"] = "hidden";

            if (IsNewPerson == false)
            {
                if (SelectedAutoSurveyId.HasValue)
                {
                    survey = SurveyRepository.GetById(SelectedAutoSurveyId.Value);
                }
                else
                {
                    BvPersonEntity person = PersonRepository.GetById(PersonSid);
                    survey = PersonService.GetPersonAutomaticSurvey(person);
                }

                if (survey != null)
                {
                    SelectedAutoSurveyId = survey.SID;
                    lblAutoSurveyName.Text = SurveyManager.FormatSurveyName(survey);
                    btnClearAutomaticSurvey.Style["visibility"] = "visible";
                }
            }
        }

        protected void ClearMembershipLists()
        {
            membershipLists.Clear();
        }

        protected void FillMembershipLists()
        {
            membershipLists.LeftCaption = Strings.NotMemberOf;
            membershipLists.RightCaption = Strings.MemberOf;

            List<CatiGroupItem> tempGroups = PersonManager.GetPersonGroups(PersonManager.GetCatiRootID());

            //Keep first group unordered because it is 'CATI Interviewers' group
            var groups = tempGroups.First().CreateList();
            groups.AddRange(tempGroups.Skip(1).OrderBy(x => x.Name));

            List<int> currentGroups = IsNewPerson ? ParentId.CreateList() : PersonService.GetParentGroups(PersonSid).ToList();

            foreach (CatiGroupItem group in groups)
            {
                var listBoxSide = currentGroups.Contains(group.Id) ? ListBoxSide.RightListBox : ListBoxSide.LeftListBox;
                membershipLists.AddRecord(group.Name, group.Id, listBoxSide);
            }
        }

        /// <summary>
        /// Shows/hides task choice panels 
        /// depending on selected task choice
        /// </summary>
        /// <remarks>
        /// In the choice mode pnlAutomaticSurvey need to be always rendered,
        /// Is hidden or show`n using client script depending on checked permissions
        /// </remarks>
        private void UpdateTaskChoicePanels()
        {
            rowAutomaticSurvey.Visible = false;
            pnlTaskChoicePermissions.Visible = false;

            switch (ddlTaskChoice.SelectedTaskChoice)
            {
                case AgentTaskChoiceMode.CampaignAssignment:
                    rowAutomaticSurvey.Visible = true;
                    BindAutomaticSurvey();
                    break;
                case AgentTaskChoiceMode.Choice:
                    pnlTaskChoicePermissions.Visible = true;

                    rowAutomaticSurvey.Visible = true;
                    BindAutomaticSurvey();

                    break;
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod()]
        public static void SetSelectedTab(string tabKey)
        {
            MaintainTabHelper.SetTabKey(ViewWithTabs.PersonProperties, tabKey);
        }
    }
}
