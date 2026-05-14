using System;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanUserSurveyListTableAction : IRoutineMaintenanceAction
    {
        private readonly IUserSurveyListTableCleanupSettings _settings;
        
        public CleanUserSurveyListTableAction(
            IUserSurveyListTableCleanupSettings settings)
        {
            _settings = settings;
        }

        public string Name
        {
            get { return "Clean UserSurveyList table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return (RoutineMaintenanceShiftType)_settings.ShiftType; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            var maxAddedTime = DateTime.UtcNow.Add(-_settings.ExpirationPeriod);
            
            BvSpUserSurveyList_CleanAdapter.ExecuteNonQuery(maxAddedTime);
        }
    }
}