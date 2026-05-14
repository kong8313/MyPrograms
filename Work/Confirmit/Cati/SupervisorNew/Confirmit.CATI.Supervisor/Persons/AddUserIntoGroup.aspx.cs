using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Resources;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.Http.Results;
using System.Windows.Forms;

namespace Confirmit.CATI.Supervisor.Persons
{
    /// <summary>
    /// Contains users that can be added into parent dialog's group
    /// </summary>
    public partial class AddUserIntoGroup : BaseForm
    {
        public override string Title
        {
            get { return Strings.SelectUsersToAdd; }
        }

        protected Hashtable m_HashTable;

        private readonly ICallCenterProvider _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Remove("userhash");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            dialogControl.CancelButton.Attributes["onclick"] = "if(parent.overlay.isOpen) parent.overlay.closeLast(); if(window.top.overlay.isOpen) top.overlay.closeLast();";
            userListGrid.GetPage += GetPage;
            if (!IsPostBack)
            {
                ViewState["ListID"] = Request["ListID"];                
            }
            userListGrid.GridName = Title;
        }
    
        protected object GetPage(out int totalCount)
        {

            var resultList = new List<CatiUserItem>();

            m_HashTable = new Hashtable();
            object hash = Session["userhash"];
            if (hash != null) m_HashTable = (Hashtable)hash;

            PagingArgs args = new PagingArgs(
                userListGrid.PageIndex,
                userListGrid.PageSize,
                userListGrid.SortedColumnName,
                userListGrid.SortIndicatorAsc,
                userListGrid.SearchParameterCollection
            );

            var pList = PersonManager.GetPersonsListPage(args, out totalCount);

            foreach (var person in pList)
            {
                int sid = person.PersonSID.Value;
                string name = person.PersonName;
                string description = person.PersonDescription;

                if (m_HashTable[sid] == null)
                {
                    var user = new CatiUserItem(sid, name, description);
                    resultList.Add(user);
                    m_HashTable[sid] = user;
                }
            }

            Session["userhash"] = m_HashTable;

            return pList;
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                object listObject = Session[(string)ViewState["ListID"]];
                object hash = Session["userhash"];
                if (hash != null) m_HashTable = (Hashtable)hash;

                if (listObject != null)
                {
                    GridListHolder list = (GridListHolder)listObject;

                    foreach (string strUserId in userListGrid.SelectedKeys)
                    {
                        int userId = Convert.ToInt32(strUserId);
                        CatiUserItem user = (CatiUserItem)m_HashTable[userId];
                        bool bFound = false;

                        foreach (CatiUserItem u in list.List)
                        {
                            if (u.Id == user.Id)
                            {
                                bFound = true;
                                break;
                            }
                        }

                        if (!bFound)
                        {
                            list.AddItem(user);
                        }
                    }
                }
                else
                {
                    AddUserMessage("SessionInformationExpired");
                }

                CloseOverlay(true,null,true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion
    }
}
