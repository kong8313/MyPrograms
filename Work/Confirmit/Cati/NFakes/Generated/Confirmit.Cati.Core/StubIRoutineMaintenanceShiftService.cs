using System;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;

namespace Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Fakes
{
    public class StubIRoutineMaintenanceShiftService : IRoutineMaintenanceShiftService 
    {
        private IRoutineMaintenanceShiftService _inner;

        public StubIRoutineMaintenanceShiftService()
        {
            _inner = null;
        }

        public IRoutineMaintenanceShiftService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DateTime GetScheduledTimeRoutineMaintenanceShiftTypeDelegate(RoutineMaintenanceShiftType shiftType);
        public GetScheduledTimeRoutineMaintenanceShiftTypeDelegate GetScheduledTimeRoutineMaintenanceShiftType;

        DateTime IRoutineMaintenanceShiftService.GetScheduledTime(RoutineMaintenanceShiftType shiftType)
        {


            if (GetScheduledTimeRoutineMaintenanceShiftType != null)
            {
                return GetScheduledTimeRoutineMaintenanceShiftType(shiftType);
            } else if (_inner != null)
            {
                return ((IRoutineMaintenanceShiftService)_inner).GetScheduledTime(shiftType);
            }

            return default(DateTime);
        }

        public delegate TimeSpan GetShiftDurationRoutineMaintenanceShiftTypeDelegate(RoutineMaintenanceShiftType shiftType);
        public GetShiftDurationRoutineMaintenanceShiftTypeDelegate GetShiftDurationRoutineMaintenanceShiftType;

        TimeSpan IRoutineMaintenanceShiftService.GetShiftDuration(RoutineMaintenanceShiftType shiftType)
        {


            if (GetShiftDurationRoutineMaintenanceShiftType != null)
            {
                return GetShiftDurationRoutineMaintenanceShiftType(shiftType);
            } else if (_inner != null)
            {
                return ((IRoutineMaintenanceShiftService)_inner).GetShiftDuration(shiftType);
            }

            return default(TimeSpan);
        }

        public delegate RoutineMaintenanceShiftType GetMatchedShiftTypeDateTimeDelegate(DateTime utcTime);
        public GetMatchedShiftTypeDateTimeDelegate GetMatchedShiftTypeDateTime;

        RoutineMaintenanceShiftType IRoutineMaintenanceShiftService.GetMatchedShiftType(DateTime utcTime)
        {


            if (GetMatchedShiftTypeDateTime != null)
            {
                return GetMatchedShiftTypeDateTime(utcTime);
            } else if (_inner != null)
            {
                return ((IRoutineMaintenanceShiftService)_inner).GetMatchedShiftType(utcTime);
            }

            return default(RoutineMaintenanceShiftType);
        }

        public delegate bool IsShiftTypeHitToAnotherRoutineMaintenanceShiftTypeRoutineMaintenanceShiftTypeDelegate(RoutineMaintenanceShiftType shiftType, RoutineMaintenanceShiftType anotherShiftType);
        public IsShiftTypeHitToAnotherRoutineMaintenanceShiftTypeRoutineMaintenanceShiftTypeDelegate IsShiftTypeHitToAnotherRoutineMaintenanceShiftTypeRoutineMaintenanceShiftType;

        bool IRoutineMaintenanceShiftService.IsShiftTypeHitToAnother(RoutineMaintenanceShiftType shiftType, RoutineMaintenanceShiftType anotherShiftType)
        {


            if (IsShiftTypeHitToAnotherRoutineMaintenanceShiftTypeRoutineMaintenanceShiftType != null)
            {
                return IsShiftTypeHitToAnotherRoutineMaintenanceShiftTypeRoutineMaintenanceShiftType(shiftType, anotherShiftType);
            } else if (_inner != null)
            {
                return ((IRoutineMaintenanceShiftService)_inner).IsShiftTypeHitToAnother(shiftType, anotherShiftType);
            }

            return default(bool);
        }

    }
}