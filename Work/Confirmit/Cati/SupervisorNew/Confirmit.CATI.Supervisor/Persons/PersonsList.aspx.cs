using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Classes;

using System.Linq;
using Confirmit.CATI.Supervisor.Core.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using Infragistics.Web.UI.GridControls;

using ConfirmitDialerInterface;
using Newtonsoft.Json;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class PersonsList : BaseForm
    {
        private readonly IToggleSettings _toggleSettings;
        private readonly IConsoleSettings _consoleSettings;
        private readonly ISupervisorServiceClient _supervisorServiceClient;

        public PersonsList()
        {
            _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _consoleSettings = ServiceLocator.Resolve<IConsoleSettings>();
        }

        /// <summary>
        /// ID of parent folder
        /// </summary>
        [StoreInViewState]
        protected int RootGroupId;

        [StoreInViewState]
        protected int? InterviewerId;

        private int? _selectedGroupId;

        public override string TopTitle
        {
            get
            {
                return _selectedGroupId == null
                    ? "All interviewers"
                    : $"Interviewers in \"{PersonGroupRepository.GetById(_selectedGroupId.Value).Name}\" group";
            }
        }

        protected AgentType SelectedInterviewerType
        {
            get
            {
                return cbIvrAgent.Checked ? AgentType.IvrAgent : AgentType.LiveAgent;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RootGroupId = PersonGroupService.RootGroupId;
                if (Request["PersonId"] != null)
                {
                    InterviewerId = int.Parse(Request["PersonId"]);
                }
            }

            cbIvrAgent.CheckedChanged += CbIvrAgent_CheckedChanged;

            m_grid.GridName = TopTitle;
            m_grid.GetPage = null;
            m_grid.GetPage +=
                delegate (out int totalCount)
                {
                    SearchParameterCollection searchCollection = PrepareSearchParameters(m_grid.SearchParameterCollection);
                    searchCollection.Add(new SearchParameter
                    {
                        ColumnName = "Type",
                        ColumnType = SearchColumnType.Number,
                        Operator = SearchOperator.Equal,
                        Value = (byte)SelectedInterviewerType
                    });
                    PagingArgs args = new PagingArgs(
                        m_grid.PageIndex,
                        m_grid.PageSize,
                        m_grid.SortedColumnKey,
                        m_grid.SortIndicatorAsc,
                        searchCollection);

                    return PersonManager.GetPersonsListPage(GetFoldersIDs(), args, out totalCount);
                };

            m_grid.InitializeRow += Grid_InitializeRow;

            if (ServiceLocator.Resolve<ISystemSettings>().CallGroup.Enabled == false)
            {
                m_grid.HideCommand("ChangeCallGroup");
                m_grid.Columns.First(x => x.Key == "CallGroupName").Hidden = true;
            }

            var toggleSettings = ServiceLocator.Resolve<IToggleSettings>();

            if (toggleSettings.EnableIVR == false)
            {
                m_grid.TopToolbarLayout = ToolbarLayout.LabelAndMenu;
            }
            else
            {
                m_grid.TopToolbarLayout = ToolbarLayout.DoubleMenu;
                cbIvrAgent.CheckedChanged += DoUpdateWithColumns;
            }

            if (toggleSettings.EnableSeamlessSurveySwitching == false)
            {
                m_grid.HideCommand("ChangeAutomaticSurvey");
            }

            if (_consoleSettings.EnableSoftphoneIntegration == false)
            {
                m_grid.HideCommand("ChangeSSOIntegration");
                m_grid.Columns.First(x => x.Key == "EnableSoftphoneIntegration").Hidden = true;
            }

            if (!_toggleSettings.ShowDialType)
            {
                m_grid.Columns.First(x => x.Key == "DialTypeId").Hidden = true;
            }

            InitSearchingToolBar();
            SetControlsVisibility();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            AdjustConfirmationTexts();

            if (InterviewerId.HasValue && IsPostBack == false)
            {
                RegisterStartupScript(String.Format("openInterviewerInfoFrame({0});", InterviewerId));
            }
        }

        private void CbIvrAgent_CheckedChanged(object sender, EventArgs e)
        {
            m_grid.ClearSelectedKeys();
        }

        private void AdjustConfirmationTexts()
        {
            var isLiveInterviewer = SelectedInterviewerType == AgentType.LiveAgent;

            m_grid.Commands.First(c => c.Key == "Delete").Confirmation = isLiveInterviewer
                ? "cnfr_DeleteLiveInterviewer"
                : "cnfr_DeleteIvrAgent";
            m_grid.Commands.First(c => c.Key == "Lock").Confirmation = isLiveInterviewer
                ? "cnfr_LockLiveInterviewer"
                : "cnfr_LockIvrAgent";
            m_grid.Commands.First(c => c.Key == "Unlock").Confirmation = isLiveInterviewer
                ? "cnfr_UnlockLiveInterviewer"
                : "cnfr_UnlockIvrAgent";
        }

        private void SetControlsVisibility()
        {
            if (SelectedInterviewerType == AgentType.IvrAgent)
            {
                m_grid.HideCommand("ChangeTaskChoice");
                m_grid.HideCommand("ChangeAutomaticSurvey");
                m_grid.HideCommand("ChangeCallGroup");
                m_grid.HideCommand("SendMessage");
                m_grid.HideCommand("NewInterviewer");
                m_grid.HideCommand("ChangeSSOIntegration");
            }
            else
            {
                m_grid.HideCommand("NewIvrAgent");
            }
        }

        protected void DoUpdateWithColumns(object sender, EventArgs e)
        {
            m_grid.RefreshColumns();
            SetControlsVisibility();
        }

        private SearchParameterCollection PrepareSearchParameters(SearchParameterCollection searchParameterCollection)
        {
            var parameter = searchParameterCollection.FirstOrDefault(x => x.ColumnName == "ManualSelection");

            if (parameter != null)
            {
                AgentTaskChoiceMode mode = (AgentTaskChoiceMode)parameter.Value;

                var allowedChoiceParameter = new SearchParameter()
                {
                    ColumnName = "AllowedChoices",
                    ColumnType = SearchColumnType.Number,
                    Value = null
                };

                if (mode == AgentTaskChoiceMode.Choice)
                {
                    searchParameterCollection.Remove(parameter);
                    allowedChoiceParameter.Operator = SearchOperator.NotEqual;
                    searchParameterCollection.Add(allowedChoiceParameter);
                }
                else
                {
                    allowedChoiceParameter.Operator = SearchOperator.Equal;
                    searchParameterCollection.Add(allowedChoiceParameter);
                }
            }

            parameter = searchParameterCollection.FirstOrDefault(x => x.ColumnName == "GroupNamesJson");

            if (parameter != null)
            {
                _selectedGroupId = Convert.ToInt32(parameter.Value);
                searchParameterCollection.Remove(parameter);
            }
            else
            {
                _selectedGroupId = null;
            }

            return searchParameterCollection;
        }

        /// <summary>
        /// Used to fill row's cells by some values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            var entry = (BvSpGetPersonsListPageEntity)e.Row.DataItem;

            bool isLogged = entry.LoggedIn.GetValueOrDefault();
            e.Row.Items.FindItemByKey("LoggedIn").Text = isLogged ? Strings.PersonLoggedInMessage : String.Empty;

            bool isLocked = entry.IsLocked.GetValueOrDefault();
            bool softphoneIntegrationEnabled = entry.EnableSoftphoneIntegration.GetValueOrDefault();

            InitBoolRow(e.Row, !isLocked, Strings.No, Strings.Yes, "IsLocked");
            if (isLocked)
            {
                e.Row.CssClass += " LockedRow";
            }
            InitBoolRow(e.Row, softphoneIntegrationEnabled, Strings.DefaultSSO, Strings.NoSSO, "EnableSoftphoneIntegration");

            AgentTaskChoiceMode mode = (entry.AllowedChoices != null) ? AgentTaskChoiceMode.Choice : (AgentTaskChoiceMode)entry.ManualSelection.Value;
            e.Row.Items.FindItemByKey("TaskChoice").Text = StringHelper.GetStringFromEnum(mode);

            var groups = JsonConvert.DeserializeObject<string[]>(entry.GroupNamesJson);
            e.Row.Items.FindItemByKey("GroupNamesJson").Text = string.Join(", ", groups.Where(x => !string.IsNullOrWhiteSpace(x)));

            string dialTypeText = string.Empty;

            if (entry.DialTypeId.HasValue &&
                Enum.IsDefined(typeof(DialType), (int)entry.DialTypeId.Value))
            {
                dialTypeText = ((DialType)entry.DialTypeId.Value).ToString();
            }
            else
            {
                dialTypeText = "Undefined";
            }

            e.Row.Items.FindItemByKey("DialTypeId").Text = dialTypeText;

        }

        /// <summary>
        /// Returns string containing IDs of parent group and all its children, delimited by comma.
        /// </summary>
        /// <returns></returns>
        protected string GetFoldersIDs()
        {
            if (_selectedGroupId.HasValue)
                return _selectedGroupId.ToString();

            var rootId = PersonManager.GetCatiRootID();
            var groups = PersonGroupService.GetChildGroups(rootId);

            // Combine group SIDs with root ID
            var groupIds = groups.Select(x => x.SID.ToString()).Append(rootId.ToString());
            var groupList = string.Join(",", groupIds);

            return groupList;
        }

        protected void DeletePerson(object sender, EventArgs e)
        {
            try
            {
                PersonManager.DeletePersons(m_grid.SelectedKeysInt);
                m_grid.ClearSelectedKeys();
                CloseInfoFrame();
            }
            catch (PersonLoggedInException ex)
            {
                BvPersonEntity person = PersonRepository.GetById(ex.PersonId);
                AddUserMessage(String.Format(Strings.ErrorDeletingLoggedInPerson, person.Name));
            }
        }

        protected void LockPerson(object sender, EventArgs e)
        {
            try
            {
                var interviewerIds = m_grid.SelectedKeysInt;
                var evt = new InterviewerLockedBySupervisorEvent(interviewerIds);
                _supervisorServiceClient.LockPersonsBySupervisor(interviewerIds);
                evt.Finish();
                RefreshInfoFrame();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void UnlockPerson(object sender, EventArgs e)
        {
            try
            {
                var interviewerIds = m_grid.SelectedKeysInt;
                var evt = new InterviewerUnLockedBySupervisorEvent(interviewerIds);
                PersonService.UnlockPersons(interviewerIds);
                evt.Finish();
                RefreshInfoFrame();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void InitSearchingToolBar()
        {
            var column = (UnboundGeneralGridColumn)m_grid.Columns.FromKey("LoggedIn");

            column.Items.Add(new ListItem(Strings.PersonLoggedInMessage, "1"));
            column.Items.Add(new ListItem(Strings.PersonNotLoggedInMessage, "0"));

            column = (UnboundGeneralGridColumn)m_grid.Columns.FromKey("IsLocked");

            column.Items.Add(new ListItem(Strings.PersonIsLockedMessage, "1"));
            column.Items.Add(new ListItem(Strings.PersonIsNotLockedMessage, "0"));

            column = (UnboundGeneralGridColumn)m_grid.Columns.FromKey("TaskChoice");

            foreach (int value in Enum.GetValues(typeof(AgentTaskChoiceMode)))
            {
                string name = StringHelper.GetStringFromEnum((AgentTaskChoiceMode)value);
                column.Items.Add(new ListItem(name, value.ToString()));
            }

            var generalColumn = (GeneralGridColumn)m_grid.Columns.FromKey("GroupNamesJson");

            foreach (var group in PersonManager.GetAllPersonGroups(string.Empty))
            {
                generalColumn.Items.Add(new ListItem(group.Name, group.Id.ToString()));
            }

            if (_toggleSettings.ShowDialType)
            {
                column = (UnboundGeneralGridColumn)m_grid.Columns.FromKey("DialTypeId");
                foreach (var dialType in DialTypeOptions.GetAllowed())
                {
                    column.Items.Add(new ListItem(dialType.ToString(), ((int)dialType).ToString()));
                }
            }

            if (_consoleSettings.EnableSoftphoneIntegration)
            {
                column = (UnboundGeneralGridColumn)m_grid.Columns.FromKey("EnableSoftphoneIntegration");

                column.Items.Add(new ListItem(Strings.NoSSO, "0"));
                column.Items.Add(new ListItem(Strings.DefaultSSO, "1"));
            }
        }

        private void InitBoolRow(GridRecord row, bool greenCondition, string greenText, string redText, string itemKey)
        {
            var item = row.Items.FindItemByKey(itemKey);
            if (greenCondition)
            {
                item.Text = greenText;
                item.CssClass += " greenFont";
            }
            else
            {
                item.Text = redText;
                item.CssClass += " redFont";
            }
        }
    }
}