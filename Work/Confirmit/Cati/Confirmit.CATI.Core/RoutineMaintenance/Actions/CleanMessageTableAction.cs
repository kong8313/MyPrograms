using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanMessageTableAction : IRoutineMaintenanceAction
    {
        private readonly IMessageTableCleanupSettings _settings;
        private readonly IPersonMessageService _personMessageService;

        public CleanMessageTableAction(
            IMessageTableCleanupSettings settings,
            IPersonMessageService personMessageService)
        {
            _settings = settings;
            _personMessageService = personMessageService;
        }

        public string Name
        {
            get { return "Clean Message table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return (RoutineMaintenanceShiftType)_settings.ShiftType; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            _personMessageService.CleanMessages(_settings.ExpirationPeriod);
        }
    }
}
