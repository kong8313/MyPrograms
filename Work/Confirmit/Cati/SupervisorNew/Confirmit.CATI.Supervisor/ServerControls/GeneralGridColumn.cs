using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Supervisor.Classes;
using Infragistics.Web.UI;
using Confirmit.CATI.Core.Paging;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.Web.UI.GridControls;
using SortDirection = System.Web.UI.WebControls.SortDirection;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Templated column for filtration in the general grid
    /// </summary>
    public class GeneralGridColumn : BoundDataField, ICollectionObject, ISearchableField, ISortableField, IMinWidth
    {
        private int _minWidth = int.MinValue;
        private const int DefaultMinWidth = 100;
        private const string DefaultDateTimeFormat = "{0:G}";
        
        public GeneralGridColumn()
        {
            EnableSorting = true;
        }

        public GeneralGridColumn(bool trackViewState)
            : base(trackViewState)
        {
            EnableSorting = true;
        }

        string ICollectionObject.GetObjectType()
        {
            return GetType().AssemblyQualifiedName.Replace('.', '~');
        }

        public SortDirection? SortIndicator { get; set; }

        public bool EnableSorting
        {
            get; set;
        }

        private List<ListItem> m_Items = new List<ListItem>();

        /// <summary>
        /// Gets/sets search type for column
        /// </summary>
        public SearchColumnType SearchColumnType
        {
            get;
            set;
        }

        public string HeaderText
        {
            get { return Header.Text; }
            set { Header.Text = value; }
        }

        public string HeaderTextId
        {
            set { HeaderText = ResourceWrapper.Instance.GetString(value); }
        }

        public override string DataFormatString
        {
            get
            {
                var format = base.DataFormatString;
                if (string.IsNullOrEmpty(format) && (Type == typeof(DateTime) || Type == typeof(DateTime?)))
                {
                    return DefaultDateTimeFormat;
                }

                return format;
            }
            set
            {
                base.DataFormatString = value;
            }
        }

        private string _searchColumnName;

        /// <summary>
        /// Gets/sets column name in the data source that will be used for filtering 
        /// </summary>
        /// <remarks>
        /// If specified filtration will be applied to column that name is specified
        /// If not specified filtration will be applied to current column        
        /// </remarks>
        public string SearchColumnName
        {
            get
            {
                return string.IsNullOrEmpty(_searchColumnName) ? DataFieldName : _searchColumnName;
            }
            set
            {
                _searchColumnName = value;
            }
        }

        /// <summary>
        /// Gets/sets default value for searching.
        /// </summary>
        public string SearchDefaultValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets default search operator for default search value.
        /// </summary>
        public SearchOperator SearchDefaultOperator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets items. Used when DropDown control is used for filtration.
        /// </summary>
        public List<ListItem> Items
        {
            get
            {
                return m_Items;
            }
        }

        public int? MinValue { get; set; }

        public int? MaxValue { get; set; }

        public int MinWidth
        {
            get
            {
                if (_minWidth == int.MinValue)
                {
                    return Width.Type == UnitType.Pixel ? (int) Width.Value : DefaultMinWidth;
                }

                return _minWidth;
            }
            set { _minWidth = value; }
        }
    }

    public interface IMinWidth
    {
        int MinWidth { get; set; }
    }

    public interface ISortableField
    {
        SortDirection? SortIndicator { get; set; }

        string DataFieldName { get; set; }

        string Key { get; set; }
        bool EnableSorting { get; set; }
    }

    [PersistenceMode(PersistenceMode.InnerProperty)]
    public class GridColumnCollection : List<GridField>
    {
        public GridField FromKey(string key)
        {
            return this.FirstOrDefault(x => x.Key == key);
        }

        /// <summary>
        /// Returns true if collection contains searchable columns, otherwise false
        /// </summary>
        /// <returns></returns>
        public bool HasSearchColumn()
        {
            return this.Any(x => x is ISearchableField && (x as ISearchableField).SearchColumnType != SearchColumnType.None);
        }
    }
}
