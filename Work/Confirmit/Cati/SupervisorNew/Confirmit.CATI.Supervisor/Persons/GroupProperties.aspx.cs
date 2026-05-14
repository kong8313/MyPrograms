using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Persons.Controls;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using System;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Services;

namespace Confirmit.CATI.Supervisor.Persons
{
    /// <summary>
    /// Summary description for GroupProperties.
    /// </summary>
    public partial class GroupProperties : BaseForm
    {
        protected GroupUserList userList;
        private readonly IInputParameterValidator _inputParameterValidator = ServiceLocator.Resolve<IInputParameterValidator>();
        private readonly IPersonGroupRepository _personGroupRepository = ServiceLocator.Resolve<IPersonGroupRepository>();
        private readonly IToggleSettings _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
        private readonly IConsoleSettings _consoleSettings = ServiceLocator.Resolve<IConsoleSettings>();
        
        public override string Title
        {
            get { return Strings.GroupProperties; }
        }

        private int? GroupID
        {
            get
            {
                object o = ViewState["GroupID"];
                return (int?)o;
            }
            set { ViewState["GroupID"] = value; }
        }

        /// <summary>
        /// Parent group ID.
        /// </summary>
        private int ParentID
        {
            get
            {
                return ViewState["ParentID"] == null ? 0 : (int)ViewState["ParentID"];
            }
            set
            {
                ViewState["ParentID"] = value;
            }
        }

        /// <summary>
        /// Gets group name
        /// </summary>
        private string GroupName
        {
            get
            {
                return nameInput.Text.Trim();
            }
        }

        /// <summary>
        /// Gets group description
        /// </summary>
        private string GroupDescription
        {
            get
            {
                return descriptionInput.Text.Trim();
            }
        }

        private bool IsDefaultGroup => GroupID.HasValue && PersonManager.IsRootGroup(GroupID.Value);

        public bool? OldIsAdministrative;

        protected void Page_Load(object sender, EventArgs e)
        {
            stateChecker.AddSaveButton(btnSaveProperties);
            stateChecker.AddSaveButton(userList.SaveButton);
            userList.StateChecker = stateChecker;

            if (!IsPostBack)
            {
                if (Request["ID"] != null)
                {
                    GroupID = Int32.Parse(Request["ID"]);
                }

                if (Request["ParentID"] != null)
                {
                    ParentID = Convert.ToInt32(Request["ParentID"]);
                }

                if (_consoleSettings.AllowTransferToAssignedSurveysOnly)
                {
                    AllowTransferredCallsFromOtherSurveyHelpTextId.HelpTextId = "AllowTransferredCallsFromAssignedSurveyHelpText";
                    AllowTransferringHelpTextId.HelpTextId = "AllowAssignedTransferringHelpText";
                }
                else
                {
                    AllowTransferredCallsFromOtherSurveyHelpTextId.HelpTextId = "AllowTransferredCallsFromOtherSurveyHelpText";
                    AllowTransferringHelpTextId.HelpTextId = "AllowTransferringHelpText";
                }
            }

            if (GroupID.HasValue)
            {
                if (!IsPostBack)
                {
                    BvPersonGroupEntity group = PersonGroupRepository.GetById(GroupID.Value);
                    nameInput.Text = group.Name;
                    descriptionInput.Text = group.Description;
                    cbAllowInboundCallsForOtherSurvey.Checked = group.InboundBehavior == InboundGroupBehavior.DeliverCallsFromOtherSurvey;
                    cbAllowTransfering.Checked = group.TransferBehavior != TransferGroupBehavior.Disabled;
                    cbAllowTransferredCallsFromOtherSurvey.Checked = group.TransferBehavior == TransferGroupBehavior.DeliverCallsFromOtherSurvey;
                    cbAdministrativeGroup.Checked = group.IsAdministrative;
                    OldIsAdministrative = group.IsAdministrative;
                    
                    trAllowInboundCallsForOtherSurvey.Visible = _toggleSettings.EnableInbound;
                    trAllowTransfering.Visible = _toggleSettings.EnableInternalTransfer;
                    trAllowTransferedCallsFromOtherSurvey.Visible = _toggleSettings.EnableInternalTransfer;

                    string tabKey = MaintainTabHelper.GetTabKey(ViewWithTabs.PersonGroupProperties);

                    if (String.IsNullOrEmpty(tabKey) == false)
                    {
                        tabs.SelectTabByKey(tabKey);
                    }
                }

                dialog.Mode = DialogWindowMode.Frame;
                dialog.HideButtons = true;
                dialog.HideHeader = true;
            }
            else
            {
                dialog.Mode = DialogWindowMode.Modal;
                dialog.HideButtons = false;
                dialog.HideHeader = true;
                stateChecker.Disabled = true;
            }

            if (GroupID.HasValue)
            {
                userList.ParentID = GroupID.Value;
                AssignmentList.SID = GroupID.Value;
                dialog.OKButton.Visible = false;

                //tab remaining has to work only for existen group
                tabs.ClientEvents.SelectedIndexChanged = "SelectedIndexChanged";
            }
            else
            {
                userList.OpenAddIntersInCurrentFrame = true;
                tabs.FindTabFromKey("Assignment").Hidden = true;
            }

            userList.Save += SaveButtonClick;
            dialog.OKButton.Click += DialogButtonClick;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            btnSaveProperties.Visible = GroupID.HasValue;

            if (GroupID.HasValue)
            {
                lbPageInfo.Text = $"'{nameInput.Text}' {Strings.Properties}";
                divHeader.Visible = true;

                nameInput.Enabled = !IsDefaultGroup;
                descriptionInput.Enabled = !IsDefaultGroup;
            }
        }

        protected void SaveButtonClick(object sender, EventArgs args)
        {
            SaveCatiGroup(false);
        }

        protected void DialogButtonClick(object sender, EventArgs args)
        {
            SaveCatiGroup(true);
        }

        protected void SaveCatiGroup(bool needToClose)
        {
            try
            {
                if (!ValidateData())
                {
                    return;
                }

                var groups = PersonGroupService.RootGroupId.CreateList();

                IEnumerable<int> childInterviewers = userList.PersonIds;
                var evt = !GroupID.HasValue
                              ? (IManagementActivityEvent)
                                new CreateInterviewerGroupEvent(0, GroupName, groups, childInterviewers)
                              : new UpdateInterviewerGroupEvent(0, GroupName, groups, childInterviewers);

                using (var transaction = new DatabaseTransactionScope("Supervisor.CreatePersonGroup", DeadlockPriority.Supervisor))
                {
                    if (!GroupID.HasValue)
                    {
                        var group = new BvPersonGroupEntity();
                        SavePropertiesToGroup(group);
                        GroupID = PersonManager.CreatePersonGroup(group, groups.ToArray());
                    }
                    else
                    {
                        BvPersonGroupEntity group = PersonGroupRepository.GetById(GroupID.Value);
                        SavePropertiesToGroup(group);
                        if (!IsDefaultGroup)
                        {
                            PersonGroupService.SetParentGroups(GroupID.Value, groups.ToArray());
                        }

                        _personGroupRepository.Update(group);
                    }

                    evt.ObjectId = GroupID.Value;

                    transaction.Commit();
                }

                userList.ParentID = GroupID.Value;

                // Here we set parent groups for the list of users, it results in WS calls. So we execute it outside of the transaction.
                userList.SaveUserList();

                stateChecker.MarkAsUnchanged();

                evt.Finish();

                dialog.RefreshListFrameIfDialogNonModal();
                RefreshLeftFrame();

                if (needToClose)
                {
                    CloseOverlay(true);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void SavePropertiesToGroup(BvPersonGroupEntity group)
        {
            if (!IsDefaultGroup)
            {
                group.Name = GroupName;
                group.Description = GroupDescription;
            }

            group.InboundBehavior = cbAllowInboundCallsForOtherSurvey.Checked ? InboundGroupBehavior.DeliverCallsFromOtherSurvey : InboundGroupBehavior.DeliverCallsFromTheSameSurvey;
        
            if (cbAllowTransfering.Checked)
            {
                group.TransferBehavior = cbAllowTransferredCallsFromOtherSurvey.Checked ? TransferGroupBehavior.DeliverCallsFromOtherSurvey : TransferGroupBehavior.DeliverCallsFromTheSameSurvey;
            }
            else
            {
                group.TransferBehavior = TransferGroupBehavior.Disabled;
            }

            group.IsAdministrative = cbAdministrativeGroup.Checked;
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(GroupName))
            {
                AddUserMessage("Err_EmptyName");
                return false;
            }

            if (!_inputParameterValidator.IsValid(GroupName))
            {
                AddUserMessage(Strings.ErrorIncorrectValue);
                return false;
            }

            if (!_inputParameterValidator.IsValid(GroupDescription))
            {
                AddUserMessage(Strings.ErrorIncorrectValue);
                return false;
            }

            if (!GroupID.HasValue)
            {
                if (PersonManager.IsPersonGroupNameUsed(GroupName))
                {
                    AddUserMessage(string.Format(Strings.ErrorInterviewerGroupAlreadyExists, GroupName));
                    return false;
                }
            }
            else if (PersonManager.IsPersonGroupNameUsed(GroupName, GroupID.Value))
            {
                AddUserMessage(string.Format(Strings.ErrorInterviewerGroupAlreadyExists, GroupName));
                return false;
            }

            if (!PersonManager.IsPersonGroupNameValid(GroupName))
            {
                AddUserMessage(string.Format(Strings.ErrorInterviewerGroupNameIsIncorrect, GroupName));
                return false;
            }

            return true;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod()]
        public static void SetSelectedTab(string tabKey)
        {
            MaintainTabHelper.SetTabKey(ViewWithTabs.PersonGroupProperties, tabKey);
        }
    }
}
