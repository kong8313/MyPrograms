using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Confirmit.CATI.Supervisor.Persons.Controls
{
    /// <summary>
    /// Control contains all users in current group
    /// It is possible add/remove interviewer into/from this list and save group
    /// </summary>
    public partial class GroupUserList : BaseWUC
    {
        private ICallCenterProvider _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();

        private GridListHolder m_list = null;

        public int ParentID { get; set; }

        public bool OpenAddIntersInCurrentFrame { get; set; }

        public StateChecker StateChecker { get; set; }

        public ToolbarCommandButton SaveButton
        {
            get { return btnSave; }
        }

        public List<int> PersonIds
        {
            get
            {
                return List.AddedItems.Cast<CatiUserItem>().Select(x => x.Id).ToList();
            }
        }

        protected GridListHolder List
        {
            get
            {
                if (m_list == null)
                {
                    if (ViewState["ListID"] != null)
                    {
                        object list_object = Session[(string)ViewState["ListID"]];
                        if (list_object != null) // object not expired
                        {
                            m_list = (GridListHolder)list_object;
                            return m_list;
                        }
                    }

                    ArrayList userList = new ArrayList();
                    m_list = new GridListHolder();
                    if (ParentID > 0)
                    {
                        var list = PersonGroupService.GetChildPersons(ParentID, _callCenterProvider.GetCurrentId());

                        foreach (var person in list)
                        {
                            int sid = person.SID.Value;
                            string name = person.Name;
                            string description = person.Description;

                            CatiUserItem user = new CatiUserItem(sid, name, description);
                            userList.Add(user);
                        }

                        m_list.List = userList;
                    }
                    else m_list.List = new ArrayList();
                    Session[m_list.ID] = m_list;
                }
                return m_list;
            }
        }

        public event EventHandler Save;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["ListID"] = List.ID;
            }

            if (ParentID > 0)
            {
                userListGrid.GridName = "PersonsList";
            }

            var command = (OverlayCommand)userListGrid.GetCommand("Add");
            command.ExternalDynamicParams.Add("ListID", (string) ViewState["ListID"]);
            command.ShowInCurrentFrame = OpenAddIntersInCurrentFrame;

            userListGrid.GetPage += GetUsers;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            btnSave.Visible = (ParentID > 0);
        }

        protected object GetUsers(out int totalCount)
        {
            PagingArgs args = new PagingArgs(
                userListGrid.PageIndex,
                userListGrid.PageSize,
                userListGrid.SortedColumnName,
                userListGrid.SortIndicatorAsc,
                userListGrid.SearchParameterCollection
            );
            List<CatiUserItem> personList = BaseMethods.GetPage(List.List.Cast<CatiUserItem>().ToList(), args, out totalCount);

            return personList;
        }

        protected void SaveClick(object sender, EventArgs e)
        {
            if (Save != null)
            {
                Save(sender, e);
            }
        }

        public void SaveUserList()
        {
            foreach (CatiUserItem user in List.AddedItems)
            {
                user.AssignTo(ParentID);
            }

            foreach (CatiUserItem user in List.DeletedItems)
            {
                user.ExcludeFrom(ParentID, PersonManager.GetCatiRootID());
            }
        }

        protected void RemoveUser(object sender, EventArgs args)
        {
            foreach (string userId in userListGrid.SelectedKeys)
            {
                int id = Convert.ToInt32(userId);
                foreach (CatiUserItem u in List.List)
                {
                    if (u.Id == id)
                    {
                        List.RemoveItem(u);
                        break;
                    }
                }
            }
            userListGrid.RefreshData();

            StateChecker.MarkAsChanged();
        }

        protected void OnInterviewersAdded(object sender, EventArgs e)
        {
            userListGrid.RefreshData();
            StateChecker.MarkAsChanged();
        }
    }
}
