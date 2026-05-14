using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public abstract class SearchableHeaderTemplateWithOperator : SearchableHeaderTemplate
    {
        protected SearchableHeaderTemplateWithOperator(string defaultValue, SearchOperator defaultOperator, HeaderTemplateSettings settings)
            : base(defaultValue, settings)
        {
            DefaultOperator = defaultOperator;
        }

        protected SearchOperator DefaultOperator { get; set; }

        private static HtmlTable GetTable()
        {
            var table = new HtmlTable { CellPadding = 0, CellSpacing = 0, Border = 0 };
            table.Rows.Add(new HtmlTableRow());
            table.Rows[0].Cells.Add(new HtmlTableCell());
            var valueCell = new HtmlTableCell { Width = "100%" };
            valueCell.Style["padding-left"] = "1px";
            table.Rows[0].Cells.Add(valueCell);
            return table;
        }

        protected sealed override IEnumerable<Control> GetSearchControls()
        {
            var table = GetTable();

            foreach (var operatorControl in GetSearchOperatorControls())
            {
                table.Rows[0].Cells[0].Controls.Add(operatorControl);
            }

            foreach (var valueControl in GetSearchValueControls())
            {
                table.Rows[0].Cells[1].Controls.Add(valueControl);
            }

            yield return table;
        }

        protected abstract IEnumerable<Control> GetSearchOperatorControls();
        protected abstract IEnumerable<Control> GetSearchValueControls();
    }
}