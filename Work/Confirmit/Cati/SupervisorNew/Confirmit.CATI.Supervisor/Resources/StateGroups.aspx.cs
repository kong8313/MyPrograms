using System;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class StateGroups : BaseForm
    {
        [StoreInViewState]
        protected int? AutoOpenStateGroupId;

        public override string TopTitle
        {
            get
            {
                return Strings.ExtendedStatusGroups;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                if (Request["ItemId"] != null)
                {
                    AutoOpenStateGroupId = int.Parse(Request["ItemId"]);
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (AutoOpenStateGroupId.HasValue && IsPostBack == false)
            {
                RegisterStartupScript(String.Format("openScriptInfoFrame({0});", AutoOpenStateGroupId));
            }
        }
    }
}
