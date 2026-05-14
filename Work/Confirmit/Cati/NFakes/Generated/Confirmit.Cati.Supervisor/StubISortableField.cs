using System;
using Confirmit.CATI.Supervisor.ServerControls;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.ServerControls.Fakes
{
    public class StubISortableField : ISortableField 
    {
        private ISortableField _inner;

        public StubISortableField()
        {
            _inner = null;
        }

        public ISortableField Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private SortDirection? _SortIndicator;
        public Func<SortDirection?> SortIndicatorGet;
        public Action<SortDirection?> SortIndicatorSetNullableOfSortDirection;

        SortDirection? ISortableField.SortIndicator
        {
            get
            {
                if (SortIndicatorGet != null)
                {
                    return SortIndicatorGet();
                } else if (_inner != null)
                {
                    return ((ISortableField)_inner).SortIndicator;
                }

                if (SortIndicatorSetNullableOfSortDirection == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SortIndicator;
                }

                return default(SortDirection?);
            }

            set
            {
                if (SortIndicatorSetNullableOfSortDirection != null)
                {
                    SortIndicatorSetNullableOfSortDirection(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISortableField)_inner).SortIndicator = value;
                    return;
                }

                if (SortIndicatorGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SortIndicator = value;
                }

            }
        }

        private string _DataFieldName;
        public Func<string> DataFieldNameGet;
        public Action<string> DataFieldNameSetString;

        string ISortableField.DataFieldName
        {
            get
            {
                if (DataFieldNameGet != null)
                {
                    return DataFieldNameGet();
                } else if (_inner != null)
                {
                    return ((ISortableField)_inner).DataFieldName;
                }

                if (DataFieldNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DataFieldName;
                }

                return default(string);
            }

            set
            {
                if (DataFieldNameSetString != null)
                {
                    DataFieldNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISortableField)_inner).DataFieldName = value;
                    return;
                }

                if (DataFieldNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DataFieldName = value;
                }

            }
        }

        private string _Key;
        public Func<string> KeyGet;
        public Action<string> KeySetString;

        string ISortableField.Key
        {
            get
            {
                if (KeyGet != null)
                {
                    return KeyGet();
                } else if (_inner != null)
                {
                    return ((ISortableField)_inner).Key;
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
                    ((ISortableField)_inner).Key = value;
                    return;
                }

                if (KeyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Key = value;
                }

            }
        }

        private bool _EnableSorting;
        public Func<bool> EnableSortingGet;
        public Action<bool> EnableSortingSetBoolean;

        bool ISortableField.EnableSorting
        {
            get
            {
                if (EnableSortingGet != null)
                {
                    return EnableSortingGet();
                } else if (_inner != null)
                {
                    return ((ISortableField)_inner).EnableSorting;
                }

                if (EnableSortingSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableSorting;
                }

                return default(bool);
            }

            set
            {
                if (EnableSortingSetBoolean != null)
                {
                    EnableSortingSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISortableField)_inner).EnableSorting = value;
                    return;
                }

                if (EnableSortingGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableSorting = value;
                }

            }
        }

    }
}