using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public class DecimalHeaderTemplate : SearchableHeaderTemplateWithOperator
    {
        public DecimalHeaderTemplate(string defaultValue, SearchOperator defaultOperator, HeaderTemplateSettings settings)
            : base(defaultValue, defaultOperator, settings)
        {
        }

        protected override IEnumerable<Control> GetSearchOperatorControls()
        {
            yield return TemplatedHeaderHelper.GetOperatorControl(Settings.GridClientController, DefaultOperator);
        }

        protected override IEnumerable<Control> GetSearchValueControls()
        {
            yield return TemplatedHeaderHelper.GetValidatorControl(ValidationDataType.Double);
            yield return TemplatedHeaderHelper.GetNumberValueControl(Settings.GridClientController, true, DefaultValue);
        }
    }
}