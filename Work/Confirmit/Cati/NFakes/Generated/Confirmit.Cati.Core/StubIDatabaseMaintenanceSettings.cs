using System;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions.Fakes
{
    public class StubIDatabaseMaintenanceSettings : IDatabaseMaintenanceSettings 
    {
        private IDatabaseMaintenanceSettings _inner;

        public StubIDatabaseMaintenanceSettings()
        {
            _inner = null;
        }

        public IDatabaseMaintenanceSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _FragmentationIndexRebuildThreshold;
        public Func<int> FragmentationIndexRebuildThresholdGet;
        public Action<int> FragmentationIndexRebuildThresholdSetInt32;

        int IDatabaseMaintenanceSettings.FragmentationIndexRebuildThreshold
        {
            get
            {
                if (FragmentationIndexRebuildThresholdGet != null)
                {
                    return FragmentationIndexRebuildThresholdGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseMaintenanceSettings)_inner).FragmentationIndexRebuildThreshold;
                }

                if (FragmentationIndexRebuildThresholdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _FragmentationIndexRebuildThreshold;
                }

                return default(int);
            }

            set
            {
                if (FragmentationIndexRebuildThresholdSetInt32 != null)
                {
                    FragmentationIndexRebuildThresholdSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseMaintenanceSettings)_inner).FragmentationIndexRebuildThreshold = value;
                    return;
                }

                if (FragmentationIndexRebuildThresholdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _FragmentationIndexRebuildThreshold = value;
                }

            }
        }

        private int _FragmentationIndexReorganizeThreshold;
        public Func<int> FragmentationIndexReorganizeThresholdGet;
        public Action<int> FragmentationIndexReorganizeThresholdSetInt32;

        int IDatabaseMaintenanceSettings.FragmentationIndexReorganizeThreshold
        {
            get
            {
                if (FragmentationIndexReorganizeThresholdGet != null)
                {
                    return FragmentationIndexReorganizeThresholdGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseMaintenanceSettings)_inner).FragmentationIndexReorganizeThreshold;
                }

                if (FragmentationIndexReorganizeThresholdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _FragmentationIndexReorganizeThreshold;
                }

                return default(int);
            }

            set
            {
                if (FragmentationIndexReorganizeThresholdSetInt32 != null)
                {
                    FragmentationIndexReorganizeThresholdSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseMaintenanceSettings)_inner).FragmentationIndexReorganizeThreshold = value;
                    return;
                }

                if (FragmentationIndexReorganizeThresholdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _FragmentationIndexReorganizeThreshold = value;
                }

            }
        }

        private string _IgnoredIndexes;
        public Func<string> IgnoredIndexesGet;
        public Action<string> IgnoredIndexesSetString;

        string IDatabaseMaintenanceSettings.IgnoredIndexes
        {
            get
            {
                if (IgnoredIndexesGet != null)
                {
                    return IgnoredIndexesGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseMaintenanceSettings)_inner).IgnoredIndexes;
                }

                if (IgnoredIndexesSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IgnoredIndexes;
                }

                return default(string);
            }

            set
            {
                if (IgnoredIndexesSetString != null)
                {
                    IgnoredIndexesSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseMaintenanceSettings)_inner).IgnoredIndexes = value;
                    return;
                }

                if (IgnoredIndexesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IgnoredIndexes = value;
                }

            }
        }

        private string _IndexFragmentationDetectMode;
        public Func<string> IndexFragmentationDetectModeGet;
        public Action<string> IndexFragmentationDetectModeSetString;

        string IDatabaseMaintenanceSettings.IndexFragmentationDetectMode
        {
            get
            {
                if (IndexFragmentationDetectModeGet != null)
                {
                    return IndexFragmentationDetectModeGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseMaintenanceSettings)_inner).IndexFragmentationDetectMode;
                }

                if (IndexFragmentationDetectModeSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IndexFragmentationDetectMode;
                }

                return default(string);
            }

            set
            {
                if (IndexFragmentationDetectModeSetString != null)
                {
                    IndexFragmentationDetectModeSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseMaintenanceSettings)_inner).IndexFragmentationDetectMode = value;
                    return;
                }

                if (IndexFragmentationDetectModeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IndexFragmentationDetectMode = value;
                }

            }
        }

        private int _MinIndexPageCount;
        public Func<int> MinIndexPageCountGet;
        public Action<int> MinIndexPageCountSetInt32;

        int IDatabaseMaintenanceSettings.MinIndexPageCount
        {
            get
            {
                if (MinIndexPageCountGet != null)
                {
                    return MinIndexPageCountGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseMaintenanceSettings)_inner).MinIndexPageCount;
                }

                if (MinIndexPageCountSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MinIndexPageCount;
                }

                return default(int);
            }

            set
            {
                if (MinIndexPageCountSetInt32 != null)
                {
                    MinIndexPageCountSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseMaintenanceSettings)_inner).MinIndexPageCount = value;
                    return;
                }

                if (MinIndexPageCountGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MinIndexPageCount = value;
                }

            }
        }

        private int _RebuildIndexShiftType;
        public Func<int> RebuildIndexShiftTypeGet;
        public Action<int> RebuildIndexShiftTypeSetInt32;

        int IDatabaseMaintenanceSettings.RebuildIndexShiftType
        {
            get
            {
                if (RebuildIndexShiftTypeGet != null)
                {
                    return RebuildIndexShiftTypeGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseMaintenanceSettings)_inner).RebuildIndexShiftType;
                }

                if (RebuildIndexShiftTypeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RebuildIndexShiftType;
                }

                return default(int);
            }

            set
            {
                if (RebuildIndexShiftTypeSetInt32 != null)
                {
                    RebuildIndexShiftTypeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseMaintenanceSettings)_inner).RebuildIndexShiftType = value;
                    return;
                }

                if (RebuildIndexShiftTypeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _RebuildIndexShiftType = value;
                }

            }
        }

        private int _ShiftType;
        public Func<int> ShiftTypeGet;
        public Action<int> ShiftTypeSetInt32;

        int IDatabaseMaintenanceSettings.ShiftType
        {
            get
            {
                if (ShiftTypeGet != null)
                {
                    return ShiftTypeGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseMaintenanceSettings)_inner).ShiftType;
                }

                if (ShiftTypeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ShiftType;
                }

                return default(int);
            }

            set
            {
                if (ShiftTypeSetInt32 != null)
                {
                    ShiftTypeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseMaintenanceSettings)_inner).ShiftType = value;
                    return;
                }

                if (ShiftTypeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ShiftType = value;
                }

            }
        }

        private string _UpdateStatisticTables;
        public Func<string> UpdateStatisticTablesGet;
        public Action<string> UpdateStatisticTablesSetString;

        string IDatabaseMaintenanceSettings.UpdateStatisticTables
        {
            get
            {
                if (UpdateStatisticTablesGet != null)
                {
                    return UpdateStatisticTablesGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseMaintenanceSettings)_inner).UpdateStatisticTables;
                }

                if (UpdateStatisticTablesSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _UpdateStatisticTables;
                }

                return default(string);
            }

            set
            {
                if (UpdateStatisticTablesSetString != null)
                {
                    UpdateStatisticTablesSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseMaintenanceSettings)_inner).UpdateStatisticTables = value;
                    return;
                }

                if (UpdateStatisticTablesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _UpdateStatisticTables = value;
                }

            }
        }

    }
}