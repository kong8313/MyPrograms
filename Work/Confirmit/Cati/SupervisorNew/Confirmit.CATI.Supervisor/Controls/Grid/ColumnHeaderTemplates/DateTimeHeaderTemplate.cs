using System.Collections.Generic;
using System.Web.UI;

using Confirmit.CATI.Core.Paging;
using Infragistics.Web.UI.EditorControls;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public class DateTimeHeaderTemplate : SearchableHeaderTemplateWithOperator, IRequiresPreInitialization
    {
        const string StubDatePickerId = "StubDatePicker";
        public DateTimeHeaderTemplate(string defaultValue, SearchOperator defaultOperator, HeaderTemplateSettings settings)
            : base(defaultValue, defaultOperator, settings)
        {
        }

        protected override IEnumerable<Control> GetSearchOperatorControls()
        {
            yield return TemplatedHeaderHelper.GetOperatorControl(Settings.GridClientController, DefaultOperator);
        }

        protected override IEnumerable<Control> GetSearchValueControls()
        {
            yield return TemplatedHeaderHelper.GetCalendarControl(Settings.GridClientController, DefaultValue);
        }

        /// <remarks>
        /// DatePicker control should be instantiated before the LoadComplete event to be correctly registered and initialized on a page.
        /// Unfortunately UltraWebGrid instantiates header templates during the DataBind, that could be done on PreRender.
        /// So here we use a small hack - if grid contains searchable header with DatePicker controls - 
        /// we call PreInitialize method during PageLoad that adds stub DatePicker control to the Grid.
        /// If it is done - all futher added DatePicker controls work correctly.
        /// </remarks>
        public void PreInitialize(Control owner)
        {
            if (owner.FindControl(StubDatePickerId) == null)
            {
                var stubDatePicker = new WebDatePicker { ID = StubDatePickerId, Visible = false };
                owner.Controls.Add(stubDatePicker);
            }
        }
    }
}