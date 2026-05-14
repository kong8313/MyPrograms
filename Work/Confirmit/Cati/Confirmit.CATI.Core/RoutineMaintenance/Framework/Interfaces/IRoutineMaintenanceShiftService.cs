using System;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;

namespace Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces
{
    public interface IRoutineMaintenanceShiftService
    {
        DateTime GetScheduledTime(RoutineMaintenanceShiftType shiftType);
        TimeSpan GetShiftDuration(RoutineMaintenanceShiftType shiftType);
        RoutineMaintenanceShiftType GetMatchedShiftType(DateTime utcTime);
        bool IsShiftTypeHitToAnother(RoutineMaintenanceShiftType shiftType, RoutineMaintenanceShiftType anotherShiftType);
    }
}