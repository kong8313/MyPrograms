using System.Web.UI;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public class SelectionColumnHeaderTemplate : ITemplate
    {
        private readonly bool _hasSearchableToolbar;
        private readonly string _gridClientControllerName;

        public SelectionColumnHeaderTemplate(bool hasSearchableToolbar, string gridClientControllerName)
        {
            _hasSearchableToolbar = hasSearchableToolbar;
            _gridClientControllerName = gridClientControllerName;
        }

        void ITemplate.InstantiateIn(Control container)
        {
            foreach (var control in TemplatedHeaderHelper.GetSelectionControl(_gridClientControllerName, _hasSearchableToolbar))
            {
                container.Controls.Add(control);
            }
        }
    }
}