using System;
using System.Web.Script.Services;
using System.Web.Services;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class PriorityGroupView : BaseForm
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            tabs.ClientEvents.SelectedIndexChanged = "SelectedIndexChangedHandler";

            string tabKey = MaintainTabHelper.GetTabKey(ViewWithTabs.CallGroupView);

            if (String.IsNullOrEmpty(tabKey) == false)
            {
                tabs.SelectTabByKey(tabKey);
            }
        }
        
        [WebMethod(EnableSession = true)]
        [ScriptMethod()]
        public static void SetSelectedTab(string tabKey)
        {
            MaintainTabHelper.SetTabKey(ViewWithTabs.CallGroupView, tabKey);
        }

    }
}
