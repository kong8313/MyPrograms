using System;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Filter
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class FilterAdd : BaseForm
    {
        public override string Title
        {
            get { return Strings.EditAdvancedFilter; }
        }

        protected void FilterSaved(object sender, EventArgs e)
        {
            CloseOverlay(true, FilterAdd1.FilterID.ToString());
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            FilterAdd1.SurveyID = Convert.ToInt32(Request["ID"]);
            FilterAdd1.FilterID = Request["fltID"] != null ? Convert.ToInt32(Request["fltID"]) : Int32.MinValue;
            FilterAdd1.FilterSaved += FilterSaved;
        }
    }
}
