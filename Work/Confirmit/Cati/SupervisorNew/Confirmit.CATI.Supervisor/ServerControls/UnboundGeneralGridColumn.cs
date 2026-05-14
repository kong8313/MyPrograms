using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Paging;
using Infragistics.Web.UI;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class UnboundGeneralGridColumn : UnboundField, ICollectionObject, ISearchableField, IMinWidth
    {
        private int _minWidth = int.MinValue;
        private const int DefaultMinWidth = 100;


        public UnboundGeneralGridColumn()
        {
        }

        public UnboundGeneralGridColumn(bool trackViewState)
            : base(trackViewState)
        {
        }

        string ICollectionObject.GetObjectType()
        {
            return GetType().AssemblyQualifiedName.Replace('.', '~');
        }

        public SearchColumnType SearchColumnType { get; set; }
        public string SearchColumnName { get; set; }
        public string SearchDefaultValue { get; set; }
        public SearchOperator SearchDefaultOperator { get; set; }
        private List<ListItem> _items = new List<ListItem>();
        public List<ListItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }
    
        public int MinWidth
        {
            get
            {
                if (_minWidth == int.MinValue)
                {
                    return Width.Type == UnitType.Pixel ? (int)Width.Value : DefaultMinWidth;
                }

                return _minWidth;
            }
            set { _minWidth = value; }
        }

        public int? MinValue { get; set; }

        public int? MaxValue { get; set; }
    }
}