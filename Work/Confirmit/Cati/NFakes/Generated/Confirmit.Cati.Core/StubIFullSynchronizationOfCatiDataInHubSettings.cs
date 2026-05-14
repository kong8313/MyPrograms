using System;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions.Fakes
{
    public class StubIFullSynchronizationOfCatiDataInHubSettings : IFullSynchronizationOfCatiDataInHubSettings 
    {
        private IFullSynchronizationOfCatiDataInHubSettings _inner;

        public StubIFullSynchronizationOfCatiDataInHubSettings()
        {
            _inner = null;
        }

        public IFullSynchronizationOfCatiDataInHubSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _ShiftType;
        public Func<int> ShiftTypeGet;
        public Action<int> ShiftTypeSetInt32;

        int IFullSynchronizationOfCatiDataInHubSettings.ShiftType
        {
            get
            {
                if (ShiftTypeGet != null)
                {
                    return ShiftTypeGet();
                } else if (_inner != null)
                {
                    return ((IFullSynchronizationOfCatiDataInHubSettings)_inner).ShiftType;
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
                    ((IFullSynchronizationOfCatiDataInHubSettings)_inner).ShiftType = value;
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