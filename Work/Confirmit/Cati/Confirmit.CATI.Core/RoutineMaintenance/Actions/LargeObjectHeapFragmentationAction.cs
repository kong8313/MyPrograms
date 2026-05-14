using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using System;
using System.Runtime;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class LargeObjectHeapFragmentationAction : IRoutineMaintenanceAction
    {
        private readonly ILargeObjectHeapFragmentationSettings _settings;

        public LargeObjectHeapFragmentationAction(
            ILargeObjectHeapFragmentationSettings settings)
        {
            _settings = settings;
        }

        public string Name => "Run fragmentation of large object heap";

        public RoutineMaintenanceShiftType ShiftType => (RoutineMaintenanceShiftType)_settings.ShiftType;
        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => true;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(2, GCCollectionMode.Forced, true, true);
        }
    }
}
