using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanCallHistoryTableAction : IRoutineMaintenanceAction
    {
        private readonly ICallHistoryTableCleanupSettings _settings;
        private readonly ICallHistoryRepository _callHistoryRepository;

        public CleanCallHistoryTableAction(
            ICallHistoryTableCleanupSettings settings, 
            ICallHistoryRepository callHistoryRepository)
        {
            _settings = settings;
            _callHistoryRepository = callHistoryRepository;
        }

        public string Name
        {
            get { return "Clean CallHistory table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return (RoutineMaintenanceShiftType)_settings.ShiftType; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            _callHistoryRepository.CleanUpExpiredRecords(_settings.ExpirationPeriod);
        }

    }
}
