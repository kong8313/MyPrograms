using System;
using System.Web.Script.Services;
using System.Web.Services;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.AsyncOperations
{
    public partial class AsyncOperationView : BaseForm
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            DisableControlsOnPostback = false;
        }        

        protected void Page_Load(object sender, EventArgs e)
        {
            tabs.FindTabFromKey("tabSpecificParameters").Hidden = !User.IsProsUser;

            if (IsPostBack == false)
            {
                tabs.GetTabByKey("tabProgress").ContentUrl += String.Format("?OperationId={0}&OperationTitle={1}&IsOpenedFromList={2}", 
                                                                            Request["OperationId"], 
                                                                            Request["OperationTitle"], 
                                                                            true);

                tabs.GetTabByKey("tabParameters").ContentUrl += String.Format("?OperationId={0}", Request["OperationId"]);
                tabs.GetTabByKey("tabSpecificParameters").ContentUrl += String.Format("?OperationId={0}", Request["OperationId"]);

                /* must be done after visibility of some tabs was changed*/
                string tabKey = MaintainTabHelper.GetTabKey(ViewWithTabs.AsyncOperationView);

                if (String.IsNullOrEmpty(tabKey) == false)
                {
                    tabs.SelectTabByKey(tabKey);
                }
            }            
        }        

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public static void SetSelectedTab(string tabKey)
        {
            MaintainTabHelper.SetTabKey(ViewWithTabs.AsyncOperationView, tabKey);
        }
    }
}
