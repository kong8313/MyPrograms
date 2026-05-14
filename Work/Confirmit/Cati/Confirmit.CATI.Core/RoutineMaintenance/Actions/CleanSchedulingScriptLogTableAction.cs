using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanSchedulingScriptLogTableAction : IRoutineMaintenanceAction
    {
        private readonly ISchedulingScriptLogTableCleanupSettings _settings;
        private readonly ISchedulingScriptLogRepository _schedulingScriptLogRepository;

        public CleanSchedulingScriptLogTableAction(
            ISchedulingScriptLogTableCleanupSettings settings,
            ISchedulingScriptLogRepository schedulingScriptLogRepository)
        {
            _settings = settings;
            _schedulingScriptLogRepository = schedulingScriptLogRepository;
        }

        public string Name
        {
            get { return "Clean ScheduingScriptLog table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return (RoutineMaintenanceShiftType)_settings.ShiftType; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType currentShiftType)
        {
            _schedulingScriptLogRepository.CleanUpExpiredRecords(_settings.ExpirationPeriod);
        }
    }
}
