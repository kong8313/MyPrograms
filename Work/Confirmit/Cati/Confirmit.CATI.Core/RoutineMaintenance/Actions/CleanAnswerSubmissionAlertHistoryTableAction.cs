using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanAnswerSubmissionAlertHistoryTableAction : IRoutineMaintenanceAction
    {
        private readonly IAnswerSubmissionAlertHistoryTableCleanupSettings _settings;

        public CleanAnswerSubmissionAlertHistoryTableAction(
            IAnswerSubmissionAlertHistoryTableCleanupSettings settings)
        {
            _settings = settings;
        }

        public string Name
        {
            get { return "Clean AnswerSubmissionAlertHistory table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return (RoutineMaintenanceShiftType)_settings.ShiftType; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            AnswerSubmissionAlertHistoryRepository.CleanUpHistoryRecords((int)_settings.ExpirationPeriod.TotalDays);
        }

    }
}
