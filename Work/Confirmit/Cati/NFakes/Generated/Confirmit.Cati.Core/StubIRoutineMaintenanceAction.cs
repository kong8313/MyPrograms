using System;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;

namespace Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Fakes
{
    public class StubIRoutineMaintenanceAction : IRoutineMaintenanceAction 
    {
        private IRoutineMaintenanceAction _inner;

        public StubIRoutineMaintenanceAction()
        {
            _inner = null;
        }

        public IRoutineMaintenanceAction Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExecuteRoutineMaintenanceShiftTypeDelegate(RoutineMaintenanceShiftType curentShiftType);
        public ExecuteRoutineMaintenanceShiftTypeDelegate ExecuteRoutineMaintenanceShiftType;

        void IRoutineMaintenanceAction.Execute(RoutineMaintenanceShiftType curentShiftType)
        {

            if (ExecuteRoutineMaintenanceShiftType != null)
            {
                ExecuteRoutineMaintenanceShiftType(curentShiftType);
            } else if (_inner != null)
            {
                ((IRoutineMaintenanceAction)_inner).Execute(curentShiftType);
            }
        }

        private string _Name;
        public Func<string> NameGet;
        public Action<string> NameSetString;

        string IRoutineMaintenanceAction.Name
        {
            get
            {
                if (NameGet != null)
                {
                    return NameGet();
                } else if (_inner != null)
                {
                    return ((IRoutineMaintenanceAction)_inner).Name;
                }

                if (NameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Name;
                }

                return default(string);
            }

        }

        private RoutineMaintenanceShiftType _ShiftType;
        public Func<RoutineMaintenanceShiftType> ShiftTypeGet;
        public Action<RoutineMaintenanceShiftType> ShiftTypeSetRoutineMaintenanceShiftType;

        RoutineMaintenanceShiftType IRoutineMaintenanceAction.ShiftType
        {
            get
            {
                if (ShiftTypeGet != null)
                {
                    return ShiftTypeGet();
                } else if (_inner != null)
                {
                    return ((IRoutineMaintenanceAction)_inner).ShiftType;
                }

                if (ShiftTypeSetRoutineMaintenanceShiftType == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ShiftType;
                }

                return default(RoutineMaintenanceShiftType);
            }

        }

        private bool _ExecuteForCompanySpecificInstance;
        public Func<bool> ExecuteForCompanySpecificInstanceGet;
        public Action<bool> ExecuteForCompanySpecificInstanceSetBoolean;

        bool IRoutineMaintenanceAction.ExecuteForCompanySpecificInstance
        {
            get
            {
                if (ExecuteForCompanySpecificInstanceGet != null)
                {
                    return ExecuteForCompanySpecificInstanceGet();
                } else if (_inner != null)
                {
                    return ((IRoutineMaintenanceAction)_inner).ExecuteForCompanySpecificInstance;
                }

                if (ExecuteForCompanySpecificInstanceSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ExecuteForCompanySpecificInstance;
                }

                return default(bool);
            }

        }

        private bool _ExecuteForMasterInstance;
        public Func<bool> ExecuteForMasterInstanceGet;
        public Action<bool> ExecuteForMasterInstanceSetBoolean;

        bool IRoutineMaintenanceAction.ExecuteForMasterInstance
        {
            get
            {
                if (ExecuteForMasterInstanceGet != null)
                {
                    return ExecuteForMasterInstanceGet();
                } else if (_inner != null)
                {
                    return ((IRoutineMaintenanceAction)_inner).ExecuteForMasterInstance;
                }

                if (ExecuteForMasterInstanceSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ExecuteForMasterInstance;
                }

                return default(bool);
            }

        }

    }
}