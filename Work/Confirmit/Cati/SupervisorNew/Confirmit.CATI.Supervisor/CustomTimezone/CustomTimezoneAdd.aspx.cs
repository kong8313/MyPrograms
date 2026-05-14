using System;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.CustomTimezone
{
    public partial class CustomTimezoneAdd : BaseForm
    {
        public override string Title => Strings.EditCustomTimezone;

        protected void CustomTimezoneSaved(object sender, EventArgs e)
        {
            CloseOverlay(true, CustomTimezoneAddControl.CustomTimezoneId.ToString());
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            CustomTimezoneAddControl.CustomTimezoneId = Request["tzID"] != null ? Convert.ToInt32(Request["tzID"]) : int.MinValue;
            CustomTimezoneAddControl.ParentTimezoneId = Convert.ToInt32(Request["Id"]);
            CustomTimezoneAddControl.CustomTimezoneSaved += CustomTimezoneSaved;
        }
    }
}
