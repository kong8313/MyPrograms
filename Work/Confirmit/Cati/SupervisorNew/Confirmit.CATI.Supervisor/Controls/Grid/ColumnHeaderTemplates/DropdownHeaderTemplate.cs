using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public class DropdownHeaderTemplate : SearchableHeaderTemplate
    {
        private readonly List<ListItem> _listItems;

        public DropdownHeaderTemplate(string defaultValue, List<ListItem> listItems, HeaderTemplateSettings settings)
            : base(defaultValue, settings)
        {
            _listItems = listItems;
        }

        protected override IEnumerable<Control> GetSearchControls()
        {
            yield return TemplatedHeaderHelper.GetDropDownControl(Settings.GridClientController, _listItems, DefaultValue);
        }
    }
}