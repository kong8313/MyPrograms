using System.Collections.Generic;
using System.Web.UI;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public class PredefinedDatePeriodHeaderTemplate : SearchableHeaderTemplate
    {
        public PredefinedDatePeriodHeaderTemplate(string defaultValue, HeaderTemplateSettings settings)
            : base(defaultValue, settings)
        {
        }

        protected override IEnumerable<Control> GetSearchControls()
        {
            yield return TemplatedHeaderHelper.GetDropDownControl(
                Settings.GridClientController,
                TemplatedHeaderHelper.GetPredefinedDateList(),
                DefaultValue);
        }
    }
}