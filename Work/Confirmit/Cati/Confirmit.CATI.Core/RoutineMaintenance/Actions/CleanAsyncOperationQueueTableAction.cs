using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanAsyncOperationQueueTableAction : IRoutineMaintenanceAction
    {
        private readonly IAsyncOperationQueueTableCleanupSettings _settings;
        private readonly IAsyncOperationRepository _asyncOperationRepository;

        public CleanAsyncOperationQueueTableAction(
            IAsyncOperationQueueTableCleanupSettings settings,
            IAsyncOperationRepository asyncOperationRepository)
        {
            _settings = settings;
            _asyncOperationRepository = asyncOperationRepository;
        }

        public string Name
        {
            get { return "Clean AsyncOperationQueue table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return (RoutineMaintenanceShiftType)_settings.ShiftType; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            _asyncOperationRepository.Clean(_settings.ExpirationPeriod);
        }

    }
}
