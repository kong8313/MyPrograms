using System.Collections.Generic;
using System.Web.UI;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public class TextHeaderTemplate : SearchableHeaderTemplate
    {
        public TextHeaderTemplate(string defaultValue, HeaderTemplateSettings settings)
            : base(defaultValue, settings)
        {
        }

        protected override IEnumerable<Control> GetSearchControls()
        {
            yield return TemplatedHeaderHelper.GetValueControl(Settings.GridClientController, DefaultValue);
        }
    }
}