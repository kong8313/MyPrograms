using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using System;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Common.Logging;
using System.Runtime.Remoting.Messaging;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanActiveSupervisorsAction : IRoutineMaintenanceAction
    {
        private readonly IActiveSupervisorService _activeSupervisorService;

        public CleanActiveSupervisorsAction(IActiveSupervisorService activeSupervisorService)
        {
            _activeSupervisorService = activeSupervisorService;
        }

        public string Name
        {
            get { return "Clean BvSupervisorsActive table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return RoutineMaintenanceShiftType.Daily; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            var rowsDeleted = _activeSupervisorService.CleanActiveSupervisors(TimeSpan.FromDays(2));

            EventDetailsScope.Current.AddMessage($"CleanActiveSupervisorsAction resulted in removing '{rowsDeleted}' rows.");
        }
    }
}
