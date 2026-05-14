using System;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanActiveDialAction: IRoutineMaintenanceAction
    {
        private readonly IActiveDialService _activeDialService;

        public CleanActiveDialAction(IActiveDialService activeDialService)
        {
            _activeDialService = activeDialService;
        }

        public string Name
        {
            get { return "Clean BvActiveDial table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return RoutineMaintenanceShiftType.Daily; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            var rowsDeleted = _activeDialService.CleanActiveDials(TimeSpan.FromDays(1));

            EventDetailsScope.Current.AddMessage($"CleanActiveDialAction resulted in removing '{rowsDeleted}' rows.");
        }
    }
}