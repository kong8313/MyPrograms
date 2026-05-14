using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.Web.UI;
using Infragistics.Web.UI.GridControls;
using SortDirection = System.Web.UI.WebControls.SortDirection;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public abstract class SearchableHeaderTemplate : ITemplate
    {
        protected readonly HeaderTemplateSettings Settings;

        protected SearchableHeaderTemplate(string defaultValue, HeaderTemplateSettings settings)
        {
            Settings = settings;
            DefaultValue = defaultValue;
        }

        public string DefaultValue { get; set; }

        void ITemplate.InstantiateIn(Control container)
        {
            var fieldCaption = (FieldCaption) ((TemplateContainer) container).Item;

            container.ID = fieldCaption.FieldKey;

            var panel = GetPanel(fieldCaption, container);

            if (!Settings.HasSearchControls) return;

            foreach (var control in GetSearchControls())
            {
                var div = TemplatedHeaderHelper.BeautifyHeaderControl(control);

                panel.Controls.Add(div);
            }

            container.Controls.Add(panel);
        }

        private Panel GetPanel(FieldCaption fieldCaption, Control container)
        {
            var labelPanel = new Panel { Width = new Unit("100%"), CssClass = "gridHeaderLabel"};
            if (!Settings.IsSortable)
            {
                labelPanel.Style[HtmlTextWriterStyle.Cursor] = "default";
                labelPanel.Attributes["onclick"] = "return false;";
            }

            container.Controls.Add(labelPanel);

            var innerPanel = new Panel() { CssClass = "grid-header-label__wrapper"};
            labelPanel.Controls.Add(innerPanel);
                
            if (fieldCaption.OwnerField.Key == Settings.SortColumnKey)
            {
                var wrapperForSvg = new HtmlGenericControl("div");
                switch (Settings.SortDirection)
                {
                    case SortDirection.Ascending:
                        wrapperForSvg.InnerHtml = new ImageProvider().GetSvg("SortAsc", "Sort Ascending");
                        break;
                    case SortDirection.Descending:
                        wrapperForSvg.InnerHtml = new ImageProvider().GetSvg("SortDesc", "Sort Descending");
                        break;
                }

                innerPanel.Controls.Add(wrapperForSvg);
            }

            var lcColumnName = new HtmlGenericControl("span")
            {
                InnerText = fieldCaption.Text,
            };

            lcColumnName.Attributes.Add("class", "grid-header-label__caption");

            innerPanel.Controls.Add(lcColumnName);

            var panel = new Panel {CssClass = "gridHeaderFilter"};
            return panel;
        }

        protected abstract IEnumerable<Control> GetSearchControls();
    }
}