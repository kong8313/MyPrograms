using System;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Core.Paging;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.ServerControls.Fakes
{
    public class StubISearchableField : ISearchableField 
    {
        private ISearchableField _inner;

        public StubISearchableField()
        {
            _inner = null;
        }

        public ISearchableField Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private SearchColumnType _SearchColumnType;
        public Func<SearchColumnType> SearchColumnTypeGet;
        public Action<SearchColumnType> SearchColumnTypeSetSearchColumnType;

        SearchColumnType ISearchableField.SearchColumnType
        {
            get
            {
                if (SearchColumnTypeGet != null)
                {
                    return SearchColumnTypeGet();
                } else if (_inner != null)
                {
                    return ((ISearchableField)_inner).SearchColumnType;
                }

                if (SearchColumnTypeSetSearchColumnType == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SearchColumnType;
                }

                return default(SearchColumnType);
            }

            set
            {
                if (SearchColumnTypeSetSearchColumnType != null)
                {
                    SearchColumnTypeSetSearchColumnType(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISearchableField)_inner).SearchColumnType = value;
                    return;
                }

                if (SearchColumnTypeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SearchColumnType = value;
                }

            }
        }

        private string _SearchColumnName;
        public Func<string> SearchColumnNameGet;
        public Action<string> SearchColumnNameSetString;

        string ISearchableField.SearchColumnName
        {
            get
            {
                if (SearchColumnNameGet != null)
                {
                    return SearchColumnNameGet();
                } else if (_inner != null)
                {
                    return ((ISearchableField)_inner).SearchColumnName;
                }

                if (SearchColumnNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SearchColumnName;
                }

                return default(string);
            }

            set
            {
                if (SearchColumnNameSetString != null)
                {
                    SearchColumnNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISearchableField)_inner).SearchColumnName = value;
                    return;
                }

                if (SearchColumnNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SearchColumnName = value;
                }

            }
        }

        private string _SearchDefaultValue;
        public Func<string> SearchDefaultValueGet;
        public Action<string> SearchDefaultValueSetString;

        string ISearchableField.SearchDefaultValue
        {
            get
            {
                if (SearchDefaultValueGet != null)
                {
                    return SearchDefaultValueGet();
                } else if (_inner != null)
                {
                    return ((ISearchableField)_inner).SearchDefaultValue;
                }

                if (SearchDefaultValueSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SearchDefaultValue;
                }

                return default(string);
            }

            set
            {
                if (SearchDefaultValueSetString != null)
                {
                    SearchDefaultValueSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISearchableField)_inner).SearchDefaultValue = value;
                    return;
                }

                if (SearchDefaultValueGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SearchDefaultValue = value;
                }

            }
        }

        private int? _MinValue;
        public Func<int?> MinValueGet;
        public Action<int?> MinValueSetNullableOfInt32;

        int? ISearchableField.MinValue
        {
            get
            {
                if (MinValueGet != null)
                {
                    return MinValueGet();
                } else if (_inner != null)
                {
                    return ((ISearchableField)_inner).MinValue;
                }

                if (MinValueSetNullableOfInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MinValue;
                }

                return default(int?);
            }

            set
            {
                if (MinValueSetNullableOfInt32 != null)
                {
                    MinValueSetNullableOfInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISearchableField)_inner).MinValue = value;
                    return;
                }

                if (MinValueGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MinValue = value;
                }

            }
        }

        private int? _MaxValue;
        public Func<int?> MaxValueGet;
        public Action<int?> MaxValueSetNullableOfInt32;

        int? ISearchableField.MaxValue
        {
            get
            {
                if (MaxValueGet != null)
                {
                    return MaxValueGet();
                } else if (_inner != null)
                {
                    return ((ISearchableField)_inner).MaxValue;
                }

                if (MaxValueSetNullableOfInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaxValue;
                }

                return default(int?);
            }

            set
            {
                if (MaxValueSetNullableOfInt32 != null)
                {
                    MaxValueSetNullableOfInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISearchableField)_inner).MaxValue = value;
                    return;
                }

                if (MaxValueGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MaxValue = value;
                }

            }
        }

        private SearchOperator _SearchDefaultOperator;
        public Func<SearchOperator> SearchDefaultOperatorGet;
        public Action<SearchOperator> SearchDefaultOperatorSetSearchOperator;

        SearchOperator ISearchableField.SearchDefaultOperator
        {
            get
            {
                if (SearchDefaultOperatorGet != null)
                {
                    return SearchDefaultOperatorGet();
                } else if (_inner != null)
                {
                    return ((ISearchableField)_inner).SearchDefaultOperator;
                }

                if (SearchDefaultOperatorSetSearchOperator == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SearchDefaultOperator;
                }

                return default(SearchOperator);
            }

            set
            {
                if (SearchDefaultOperatorSetSearchOperator != null)
                {
                    SearchDefaultOperatorSetSearchOperator(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISearchableField)_inner).SearchDefaultOperator = value;
                    return;
                }

                if (SearchDefaultOperatorGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SearchDefaultOperator = value;
                }

            }
        }

        private List<ListItem> _Items;
        public Func<List<ListItem>> ItemsGet;
        public Action<List<ListItem>> ItemsSetListOfListItem;

        List<ListItem> ISearchableField.Items
        {
            get
            {
                if (ItemsGet != null)
                {
                    return ItemsGet();
                } else if (_inner != null)
                {
                    return ((ISearchableField)_inner).Items;
                }

                if (ItemsSetListOfListItem == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Items;
                }

                return default(List<ListItem>);
            }

        }

        private string _Key;
        public Func<string> KeyGet;
        public Action<string> KeySetString;

        string ISearchableField.Key
        {
            get
            {
                if (KeyGet != null)
                {
                    return KeyGet();
                } else if (_inner != null)
                {
                    return ((ISearchableField)_inner).Key;
                }

                if (KeySetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Key;
                }

                return default(string);
            }

            set
            {
                if (KeySetString != null)
                {
                    KeySetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISearchableField)_inner).Key = value;
                    return;
                }

                if (KeyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Key = value;
                }

            }
        }

    }
}