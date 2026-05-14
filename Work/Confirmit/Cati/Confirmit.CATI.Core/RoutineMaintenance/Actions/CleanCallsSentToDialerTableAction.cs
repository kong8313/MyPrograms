using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanCallsSentToDialerTableAction : IRoutineMaintenanceAction
    {
        private readonly ICallsSentToDialerTableCleanupSettings _settings;
        private readonly ISurveyCallDistributionService _callDistributionService;
        
        public CleanCallsSentToDialerTableAction(
            ICallsSentToDialerTableCleanupSettings settings,
            ISurveyCallDistributionService callDistributionService
            )
        {
            _settings = settings;
            _callDistributionService = callDistributionService;
        }

        public string Name
        {
            get { return "Clean CallsSentToDialer table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return (RoutineMaintenanceShiftType)_settings.ShiftType; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            _callDistributionService.CleanupCallsDistribution(_settings.ExpirationPeriod);
        }

    }
}
