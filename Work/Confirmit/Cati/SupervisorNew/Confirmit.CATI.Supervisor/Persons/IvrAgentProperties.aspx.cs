using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.CompanyService;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class AgentProperties : BaseForm
    {
        private readonly ICallCenterProvider _callCenterProvider;
        private readonly IPersonRepository _personRepository;
        private readonly ISupervisorServiceClient _supervisorServiceClient;
        private readonly ICompanyInformationService _companyInformationService;
        private readonly IToggleSettings _toggleSettings;
        
        public AgentProperties()
        {
            _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
            _personRepository = ServiceLocator.Resolve<IPersonRepository>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _companyInformationService = ServiceLocator.Resolve<ICompanyInformationService>();
            _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
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
        /// Gets flag indicated is new IVR agent created or existent is edited
        /// </summary>
        protected bool IsNewPerson
        {
            get
            {
                return PersonSid == 0;
            }
        }

        /// <summary>
        /// Gets IVR agent name prefix
        /// </summary>
        private string AgentNamePrefix
        {
            get
            {
                return tbxAgentNamePrefix.Text.Trim();
            }
        }

        private int NumberOfAgentsToCreate => neNumberOfAgentsToCreate.ValueInt;

        private int CountOfAvailableForCreatingIvrAgents
        {
            get
            {
                return _companyInformationService.GetMaxIvrAgentsForCurrentCompany() -
                       _personRepository.GetAll().Count(p => p.Type == (byte)AgentType.IvrAgent);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            stateChecker.AddSaveButton(btnSaveMembership);

            if (!IsPostBack)
            {
                ParentId = Request["ParentID"] != null
                    ? Convert.ToInt32(Request["ParentID"])
                    : PersonGroupService.RootGroupId;

                if (Request["PersonSID"] != null)
                    PersonSid = Convert.ToInt32(Request["PersonSID"]);

                if (!IsNewPerson)
                {
                    BindExistingPersonProperties();
                }
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
                tabs.ClientEvents.SelectedIndexChanged = "SelectedIndexChanged";
            }

            FillMembershipLists();

            SetControlsVisibility();
        }

        private void SetControlsVisibility()
        {
            dialogControl.OKButton.Visible = IsNewPerson;
            btnSaveMembership.Visible = !IsNewPerson;
            dialTypeRow.Visible = _toggleSettings.ShowDialType;
            
            if (IsNewPerson)
            {
                tabs.FindTabFromKey("Assignment").Hidden = true;
                dialogControl.HideHeader = true;
                dialogControl.HideButtons = false;
            }
            else
            {
                tabs.FindTabFromKey("Properties").Hidden = true;
            }
        }

        private void BindExistingPersonProperties()
        {
            var person = _personRepository.GetById(PersonSid);

            tbxAgentNamePrefix.Text = "IVRAgent";//it required because of validation cannot be disabled
            neNumberOfAgentsToCreate.ValueText = "0";//too
            dialogControl.Title = string.Format(Strings.IvrAgentProperties, person.Name);
            lbPageInfo.Text = string.Format(Strings.IvrAgentProperties, person.Name);
            lblMembership.Text = string.Format(Strings.IvrAgentProperties, person.Name);

            string tabKey = MaintainTabHelper.GetTabKey(ViewWithTabs.PersonProperties);

            if (String.IsNullOrEmpty(tabKey) == false)
            {
                tabs.SelectTabByKey(tabKey);
            }
        }

        protected void SaveHandler(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateData())
                {
                    return;
                }

                var parentGroups = new List<int>(membershipLists.RightValues);
                if (parentGroups.Count == 0)
                {
                    parentGroups.Add(ParentId);
                }

                var callCenterId = _callCenterProvider.GetCurrentId();

                if (IsNewPerson)
                {
                    CreateIvrAgents(callCenterId, parentGroups);
                }
                else
                {
                    UpdateIvrAgent(callCenterId, parentGroups);
                }

                ClearMembershipLists();
                FillMembershipLists();

                stateChecker.MarkAsUnchanged();

                dialogControl.RefreshListFrameIfDialogNonModal();

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

        private void UpdateIvrAgent(int callCenterId, List<int> parentGroups)
        {
            var person = _personRepository.GetById(PersonSid);
            CreateOrUpdateIvrAgent(callCenterId, parentGroups, person.Name);
        }

        private void CreateIvrAgents(int callCenterId, List<int> parentGroups)
        {
            var initialIndex = GetInitialIndex();

            for (int i = 1; i <= NumberOfAgentsToCreate; i++)
            {
                var agentName = string.Format("{0}#{1}", AgentNamePrefix, i + initialIndex);
                CreateOrUpdateIvrAgent(callCenterId, parentGroups, agentName);
            }
        }

        private int GetInitialIndex()
        {
            var agents = _personRepository.GetByType(AgentType.IvrAgent).Where(p => p.Name.StartsWith(AgentNamePrefix));

            if (!agents.Any())
            {
                return 0;
            }

            return agents.Select(p => int.Parse(p.Name.Split('#')[1])).Max();
        }

        private void CreateOrUpdateIvrAgent(int callCenterId, List<int> parentGroups, string agentName)
        {
            _supervisorServiceClient.CreateOrUpdatePerson(
                callCenterId,
                PersonSid,
                agentName,
                "",
                "",
                "",
                AgentTaskChoiceMode.Automatic,
                PersonAssignmentListMode.AllCalls,
                null,
                parentGroups,
                null,
                0,
                "",
                ddlDialType.SelectedDialType ?? DialType.Landline,
                AgentType.IvrAgent);
        }

        /// <summary>
        /// Validates user provided data 
        /// </summary>
        private bool ValidateData()
        {
            if (!IsNewPerson)
            {
                return true;
            }

            if (CountOfAvailableForCreatingIvrAgents <= 0)
            {
                AddUserMessage(string.Format(Strings.ErrorLimitOfIvrAgentsIsAchieved, NumberOfAgentsToCreate));
                return false;
            }

            if (NumberOfAgentsToCreate > CountOfAvailableForCreatingIvrAgents)
            {
                AddUserMessage(string.Format(Strings.ErrorLimitOfIvrAgentsIsAlmostAchieved, NumberOfAgentsToCreate, CountOfAvailableForCreatingIvrAgents));
                return false;
            }

            return !string.IsNullOrEmpty(tbxAgentNamePrefix.Text) && !string.IsNullOrWhiteSpace(tbxAgentNamePrefix.Text);
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

        [WebMethod(EnableSession = true)]
        [ScriptMethod()]
        public static void SetSelectedTab(string tabKey)
        {
            MaintainTabHelper.SetTabKey(ViewWithTabs.PersonProperties, tabKey);
        }
    }
}
