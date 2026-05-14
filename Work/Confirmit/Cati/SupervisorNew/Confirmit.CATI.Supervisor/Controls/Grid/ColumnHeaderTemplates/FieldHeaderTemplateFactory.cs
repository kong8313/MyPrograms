using System;
using System.Web.UI;

using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.ServerControls;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates
{
    public class FieldHeaderTemplateFactory
    {
        public ITemplate Create(GridField field, HeaderTemplateSettings settings, string defaultValue, SearchOperator defaultOperator)
        {
            if (field is ISearchableField)
            {
                var searchableField = (field as ISearchableField);

                switch (searchableField.SearchColumnType)
                {
                    case SearchColumnType.Text:
                        return new TextHeaderTemplate(defaultValue, settings);
                    case SearchColumnType.DateTime:
                        return new DateTimeHeaderTemplate(defaultValue, defaultOperator, settings);
                    case SearchColumnType.DropDown:
                    case SearchColumnType.TextDropDown:
                        return new DropdownHeaderTemplate(defaultValue, searchableField.Items, settings);
                    case SearchColumnType.Number:
                        return new NumberHeaderTemplate(defaultValue, defaultOperator, settings, searchableField.MaxValue, searchableField.MinValue);
                    case SearchColumnType.Decimal:
                        return new DecimalHeaderTemplate(defaultValue, defaultOperator, settings);
                    case SearchColumnType.TimeSpan:
                        return new TimeSpanHeaderTemplate(defaultValue, defaultOperator, settings);
                    case SearchColumnType.PredefinedDatePeriod:
                        return new PredefinedDatePeriodHeaderTemplate(defaultValue, settings);
                    case SearchColumnType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new NotSearchableHeaderTemplate(defaultValue, settings);
        }
    }
}