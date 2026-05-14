using System.Collections.Generic;
using System.Web.UI;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public class NotSearchableHeaderTemplate : SearchableHeaderTemplate
    {
        public NotSearchableHeaderTemplate(string defaultValue, HeaderTemplateSettings settings):base(defaultValue,settings)
        {
        }

        protected override IEnumerable<Control> GetSearchControls()
        {
            yield break;
        }
    }
}