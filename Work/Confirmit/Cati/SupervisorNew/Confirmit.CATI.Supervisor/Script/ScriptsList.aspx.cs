using System;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Script
{
    public partial class ScriptsList : BaseForm
    {
        [StoreInViewState]
        protected int? AutoOpenSchedulingScriptId;

        public override string Title
        {
            get { return Strings.ScriptsList; }
        }

        public override string TopTitle
        {
            get { return Strings.SchedulingScripts; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                if (Request["ItemId"] != null)
                {
                    AutoOpenSchedulingScriptId = int.Parse(Request["ItemId"]);
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (AutoOpenSchedulingScriptId.HasValue && IsPostBack == false)
            {
                RegisterStartupScript(String.Format("openScriptInfoFrame({0});", AutoOpenSchedulingScriptId));
            }
        }
    }
}
