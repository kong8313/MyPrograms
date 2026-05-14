using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.Services.RecordsMigration;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class DeferredRecordsMigration : IRoutineMaintenanceAction
    {
        private readonly IMigrationService _migrationService;
        
        public DeferredRecordsMigration(IMigrationService migrationService)
        {
            _migrationService = migrationService;
        }
        
        public string Name => "Migrate deferred records.";

        public RoutineMaintenanceShiftType ShiftType => RoutineMaintenanceShiftType.Weekly;

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            _migrationService.MigrateDeferredRecords();
        }
    }
}
