using System.Collections.Generic;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public interface ISearchableField
    {
        /// <summary>
        /// Gets/sets search type for column
        /// </summary>
        SearchColumnType SearchColumnType { get; set; }

        /// <summary>
        /// Gets/sets column name in the data source that will be used for filtering 
        /// </summary>
        /// <remarks>
        /// If specified filtration will be applied to column that name is specified
        /// If not specified filtration will be applied to current column        
        /// </remarks>
        string SearchColumnName { get; set; }

        /// <summary>
        /// Gets/sets default value for searching.
        /// </summary>
        string SearchDefaultValue { get; set; }

        int? MinValue { get; set; }

        int? MaxValue { get; set; }

        /// <summary>
        /// Gets/sets default search operator for default search value.
        /// </summary>
        SearchOperator SearchDefaultOperator { get; set; }

        /// <summary>
        /// Gets items. Used when DropDown control is used for filtration.
        /// </summary>
        List<ListItem> Items { get; }

        string Key { get; set; }
    }
}