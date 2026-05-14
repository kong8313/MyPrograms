using System;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions.Fakes
{
    public class StubILargeObjectHeapFragmentationSettings : ILargeObjectHeapFragmentationSettings 
    {
        private ILargeObjectHeapFragmentationSettings _inner;

        public StubILargeObjectHeapFragmentationSettings()
        {
            _inner = null;
        }

        public ILargeObjectHeapFragmentationSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _ShiftType;
        public Func<int> ShiftTypeGet;
        public Action<int> ShiftTypeSetInt32;

        int ILargeObjectHeapFragmentationSettings.ShiftType
        {
            get
            {
                if (ShiftTypeGet != null)
                {
                    return ShiftTypeGet();
                } else if (_inner != null)
                {
                    return ((ILargeObjectHeapFragmentationSettings)_inner).ShiftType;
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
                    ((ILargeObjectHeapFragmentationSettings)_inner).ShiftType = value;
                    return;
                }

                if (ShiftTypeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ShiftType = value;
                }

            }
        }

    }
}