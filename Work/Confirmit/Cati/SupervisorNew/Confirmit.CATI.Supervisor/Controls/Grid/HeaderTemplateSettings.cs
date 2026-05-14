using System;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.Controls.Grid
{
    public class HeaderTemplateSettings
    {
        // We jave to use delegates here to get data because this class is created too early and this data may be updated later in page lifecycle.
        public Func<string> SortColumnKeyProvider { private get; set; }
        public Func<SortDirection> SortDirectionProvider { private get; set; }
        public string GridClientController { get; set; }

        public string SortColumnKey
        {
            get { return SortColumnKeyProvider(); }
        }

        public SortDirection SortDirection
        {
            get { return SortDirectionProvider(); }
        }

        public bool HasSearchControls { get; set; }
        public bool IsSortable { get; set; }
    }
}