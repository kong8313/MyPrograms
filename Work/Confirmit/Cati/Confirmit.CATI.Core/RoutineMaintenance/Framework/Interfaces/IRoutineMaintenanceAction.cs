using System;
using System.Threading;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;

namespace Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces
{
    public interface IRoutineMaintenanceAction
    {
        string Name { get; }

        RoutineMaintenanceShiftType ShiftType { get; }
        
        bool ExecuteForCompanySpecificInstance { get; }
        
        bool ExecuteForMasterInstance { get; }

        void Execute(RoutineMaintenanceShiftType curentShiftType);

    }
}
