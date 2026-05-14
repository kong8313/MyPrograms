using System.Collections.Generic;
using System.Web.UI;

using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public class TimeSpanHeaderTemplate : SearchableHeaderTemplateWithOperator
    {
        public TimeSpanHeaderTemplate(string defaultValue, SearchOperator defaultOperator, HeaderTemplateSettings settings)
            : base(defaultValue, defaultOperator, settings)
        {
        }

        protected override IEnumerable<Control> GetSearchOperatorControls()
        {
            yield return TemplatedHeaderHelper.GetOperatorControl(Settings.GridClientController, DefaultOperator);

        }

        protected override IEnumerable<Control> GetSearchValueControls()
        {
            yield return TemplatedHeaderHelper.GetTimeSpanControl(Settings.GridClientController, DefaultValue);

        }
    }
}