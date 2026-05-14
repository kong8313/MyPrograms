using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Validators.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Exceptions;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.ServerControls;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Persons
{
    /// <summary>
    /// Represents list of groups
    /// </summary>
    public partial class GroupsList : BaseForm, IPostBackEventHandler
    {
        private const string DeleteGroupsClientPostBackEvent = "DeletionConfirmed";
        private readonly IMultipleAssignmentValidator _multipleAssignmentValidator;
        private readonly IToggleSettings _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();

        private const string InboundColumnKey = "Inbound";
        private const string TransferColumnKey = "Transfer";
        private const string AdministrativeColumnKey = "IsAdministrative";
        
        [StoreInViewState]
        protected int? GroupId;
        public override string Title
        {
            get { return Strings.GroupsList; }
        }

        public override string TopTitle
        {
            get
            {
                var group = PersonGroupRepository.GetById(PersonGroupService.RootGroupId);
                return string.Format("Groups in \"{0}\" group", group.Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["GroupId"] != null)
                {
                    GroupId = int.Parse(Request["GroupId"]);
                }
            }
            
            groupsListGrid.GetPage = (out int totalCount) =>
            {
                List<PersonGroupInfoItem> groups = PersonManager.GetAllPersonGroups(string.Empty);

                return BaseMethods.GetPage(groups, groupsListGrid.PageArguments, out totalCount);
            };
            groupsListGrid.InitializeRow += GroupsListGrid_InitializeRow;

            GeneralGridColumn inboundColumn = (GeneralGridColumn)groupsListGrid.Columns.FromKey(InboundColumnKey);
            GeneralGridColumn transferColumn = (GeneralGridColumn)groupsListGrid.Columns.FromKey(TransferColumnKey);
            GeneralGridColumn administrativeColumn = (GeneralGridColumn)groupsListGrid.Columns.FromKey(AdministrativeColumnKey);

            inboundColumn.Items.Add(new ListItem(GetInboundSettingText(InboundGroupBehavior.DeliverCallsFromTheSameSurvey), ((int)InboundGroupBehavior.DeliverCallsFromTheSameSurvey).ToString()));
            inboundColumn.Items.Add(new ListItem(GetInboundSettingText(InboundGroupBehavior.DeliverCallsFromOtherSurvey), ((int)InboundGroupBehavior.DeliverCallsFromOtherSurvey).ToString()));

            transferColumn.Items.Add(new ListItem(GetTransferSettingText(TransferGroupBehavior.Disabled), ((int)TransferGroupBehavior.Disabled).ToString()));
            transferColumn.Items.Add(new ListItem(GetTransferSettingText(TransferGroupBehavior.DeliverCallsFromTheSameSurvey), ((int)TransferGroupBehavior.DeliverCallsFromTheSameSurvey).ToString()));
            transferColumn.Items.Add(new ListItem(GetTransferSettingText(TransferGroupBehavior.DeliverCallsFromOtherSurvey), ((int)TransferGroupBehavior.DeliverCallsFromOtherSurvey).ToString()));

            administrativeColumn.Items.Add(new ListItem("Yes", "1"));
            administrativeColumn.Items.Add(new ListItem("No", "0"));
            
            inboundColumn.Hidden = !_toggleSettings.EnableInbound;
            transferColumn.Hidden = !_toggleSettings.EnableInternalTransfer;

            if (ServiceLocator.Resolve<IToggleSettings>().EnableSeamlessSurveySwitching == false)
            {
                groupsListGrid.HideCommand("ChangeAutomaticSurvey");
            }
        }

        void GroupsListGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Items.FindItemByKey(InboundColumnKey).Column.Type = typeof(string);
            e.Row.Items.FindItemByKey(TransferColumnKey).Column.Type = typeof(string);
            e.Row.Items.FindItemByKey(AdministrativeColumnKey).Column.Type = typeof(string);
            var row = (PersonGroupInfoItem)e.Row.DataItem;
            e.Row.Items.FindItemByKey(AdministrativeColumnKey).Text = row.IsAdministrative ? "Yes" : "No";
            e.Row.Items.FindItemByKey(InboundColumnKey).Text = GetInboundSettingText(row.InboundCallBehavior);
            e.Row.Items.FindItemByKey(TransferColumnKey).Text = GetTransferSettingText(row.CallTransferBehavior);
        }

        private string GetTransferSettingText(TransferGroupBehavior behavior)
        {
            switch (behavior)
            {
                case TransferGroupBehavior.Disabled:
                    return GetResString("Disabled");
                case TransferGroupBehavior.DeliverCallsFromTheSameSurvey:
                    return GetResString("TransferSetting_DeliveryCallForTheSameSurvey");
                case TransferGroupBehavior.DeliverCallsFromOtherSurvey:
                    return GetResString("TransferSetting_DeliveryCallForOtherSurvey");
                default:
                    return behavior.ToString();
            }
        }

        private string GetInboundSettingText(InboundGroupBehavior behavior)
        {
            switch (behavior)
            {
                case InboundGroupBehavior.DeliverCallsFromTheSameSurvey:
                    return GetResString("InboundSetting_DeliveryCallForTheSameSurvey");
                case InboundGroupBehavior.DeliverCallsFromOtherSurvey:
                    return GetResString("InboundSetting_DeliveryCallForOtherSurvey");
                default:
                    return behavior.ToString();
            }
        }

        public GroupsList()
        {
            _multipleAssignmentValidator = ServiceLocator.Resolve<IMultipleAssignmentValidator>();
        }

        #region Methods

        protected void DeleteGroup(object sender, EventArgs args)
        {
            if (DisplayDeletionWarning())
            {
                return;
            }

            DeleteGroups();
        }

        private bool DisplayDeletionWarning()
        {
            var multupleAssignmentGroups = groupsListGrid.SelectedKeysInt
                .Where(_multipleAssignmentValidator.IsMultipleAssignmentGroup)
                .ToList();

            if (!multupleAssignmentGroups.Any())
            {
                return false;
            }

            CreateGroupDeletionClientMessage(multupleAssignmentGroups);
            return true;
        }

        private void CreateGroupDeletionClientMessage(IEnumerable<int> multupleAssignmentGroups)
        {
            var client = ClientScript.GetPostBackEventReference(this, DeleteGroupsClientPostBackEvent);

            var names = string.Join(", ", multupleAssignmentGroups.Select(id => PersonGroupRepository.GetById(id).Name));
            var message = string.Format(GetResString("MultipleAssignmentGroupsDeletionWarning"), names);

            RegisterScriptBlock("if (confirm('" + message + "')) {" + client + ";}");
        }

        private void DeleteGroups()
        {
            try
            {
                if(groupsListGrid.SelectedKeysInt.Contains(PersonGroupService.RootGroupId))
                {
                    AddUserMessage(Strings.RootGroupDeleteWarning);
                    groupsListGrid.RefreshData();
                    return;
                }

                using (var transaction = new DatabaseTransactionScope("Supervisor.DeleteGroup", DeadlockPriority.Supervisor))
                {
                    groupsListGrid.SelectedKeysInt.ForEach(PersonManager.DeletePersonGroup);

                    transaction.Commit();
                }

                groupsListGrid.RefreshData();
                RefreshLeftFrame();
                CloseInfoFrame();
            }
            catch (GroupNotEmptyException ex)
            {
                BvPersonGroupEntity group = PersonGroupRepository.GetById(ex.GroupId);
                AddUserMessage(String.Format(Strings.ErrorDeletingNotEmptyGroup, group.Name));
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        #endregion

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.Equals(DeleteGroupsClientPostBackEvent))
            {
                DeleteGroups();
            }
        }
        
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (GroupId.HasValue && IsPostBack == false)
            {
                RegisterStartupScript(String.Format("openGroupInfoFrame({0});", GroupId));
            }
        }
    }
}
