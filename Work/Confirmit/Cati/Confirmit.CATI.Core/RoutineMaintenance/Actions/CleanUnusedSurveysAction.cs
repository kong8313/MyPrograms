using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanUnusedSurveysAction : IRoutineMaintenanceAction
    {
        private readonly ISurveyCleanupSettings _settings;
        private readonly ISurveyCleaningService _surveyCleaningService;

        public CleanUnusedSurveysAction(
            ISurveyCleanupSettings settings,
            ISurveyCleaningService surveyCleaningService)
        {
            _settings = settings;
            _surveyCleaningService = surveyCleaningService;
        }

        public string Name
        {
            get { return "Clean UnusedSurveys table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return (RoutineMaintenanceShiftType)_settings.ShiftType; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            _surveyCleaningService.CleanAllUnusedSurveys();
        }

    }
}
