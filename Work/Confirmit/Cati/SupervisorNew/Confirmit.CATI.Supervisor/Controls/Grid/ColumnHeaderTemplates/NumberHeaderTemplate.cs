using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public class NumberHeaderTemplate : SearchableHeaderTemplateWithOperator
    {
        private readonly int _maxValue;
        private readonly int _minValue;

        public NumberHeaderTemplate(string defaultValue, SearchOperator defaultOperator, HeaderTemplateSettings settings, int? maxValue, int? minValue)
            : base(defaultValue, defaultOperator, settings)
        {
            _maxValue = maxValue ?? int.MaxValue;
            _minValue = minValue ?? int.MinValue;
        }

        protected override IEnumerable<Control> GetSearchOperatorControls()
        {
            yield return TemplatedHeaderHelper.GetOperatorControl(Settings.GridClientController, DefaultOperator);
        }

        protected override IEnumerable<Control> GetSearchValueControls()
        {
            yield return TemplatedHeaderHelper.GetValidatorControl(ValidationDataType.Integer, _maxValue, _minValue);
            yield return TemplatedHeaderHelper.GetNumberValueControl(Settings.GridClientController, false, DefaultValue);
        }
    }
}