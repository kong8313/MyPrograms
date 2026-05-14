using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class CallGroupProperties : BaseForm
    {
        [StoreInViewState]
        protected int? GroupId;

        private readonly IPriorityGroupsManager _priorityGroupsManager =
            ServiceLocator.Resolve<IPriorityGroupsManager>();

        public int? SelectedStateGroupId
        {
            get { return ddlStatesList.SelectedItem != null ? Int32.Parse(ddlStatesList.SelectedItem.Value) : (int?)null; }
            set
            {
                if (value.HasValue)
                {
                    var groupItem = (from ListItem item in ddlStatesList.Items
                                     let id = Int32.Parse(item.Value)
                                     where id == value
                                     select item).FirstOrDefault();

                    if (groupItem != null)
                    {
                        groupItem.Selected = true;
                    }
                }
                else
                {
                    ddlStatesList.SelectedIndex = 0;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["Id"] != null)
                {
                    GroupId = Convert.ToInt32(Request["Id"]);
                }

                ddlStatesList.Items.Clear();
                ddlStatesList.Items.AddRange(StateGroupRepository.GetAll().OrderBy(group => group.ID).Select(group => new ListItem(group.Name, group.ID.ToString())).ToArray());

                if (GroupId.HasValue)
                {
                    dialog.OKButton.Text = Strings.Save;

                    var group = _priorityGroupsManager.GetGroup(GroupId.Value);

                    tbPriorityGroupName.Text = group.Name;
                    tbPriorityGroupDescription.Text = group.Description;

                    SelectedStateGroupId = group.DesignStateGroupID;
                }


            }                                    
        }
        
        protected void OKButtonClick(object sender, EventArgs e)
        {
            if (IsGroupNameBusy())
            {
                return;
            }

            try
            {            
                using (var transaction = new DatabaseTransactionScope("CreatePriorityGroup", DeadlockPriority.Supervisor))
                {
                    var groupName = tbPriorityGroupName.Text.Trim();
                    var groupDescription = tbPriorityGroupDescription.Text.Trim();
                    var designStateGroup = SelectedStateGroupId;

                    if (GroupId.HasValue)
                    {
                        _priorityGroupsManager.UpdateGroup(GroupId.Value, groupName, groupDescription, designStateGroup);
                    }
                    else
                    {
                        _priorityGroupsManager.AddGroup(groupName, groupDescription, designStateGroup);
                    }
                    transaction.Commit();
                }

                CloseOverlay(true);
            }
            catch (ArgumentException ex)
            {
               AddUserMessage(ex);
            }
            catch(Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private bool IsGroupNameBusy()
        {
            var groupName = tbPriorityGroupName.Text.Trim();

            if (GroupId.HasValue == false ||
                groupName != _priorityGroupsManager.GetGroup(GroupId.Value).Name)
            {
                if (_priorityGroupsManager.IsGroupNameBusy(groupName))
                {
                    AddUserMessage(string.Format(Strings.ErrorCallGroupAlreadyExists, groupName));
                    return true;
                }
            }

            return false;
        }
    }
}
