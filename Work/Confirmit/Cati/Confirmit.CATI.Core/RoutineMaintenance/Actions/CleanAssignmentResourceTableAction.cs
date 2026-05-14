using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using System.Threading;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanAssignmentResourceTableAction : IRoutineMaintenanceAction
    {
        private readonly IAssignmentResourceTableCleanupSettings _settings;

        public CleanAssignmentResourceTableAction(
            IAssignmentResourceTableCleanupSettings settings)
        {
            _settings = settings;
        }

        public string Name
        {
            get { return "Clean AssignmentResource table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return (RoutineMaintenanceShiftType)_settings.ShiftType; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            var unusedAssignments = BvSpAssignmentResource_ListUnusedAdapter.ExecuteEntityList();
            foreach (var assignment in unusedAssignments)
            {
                BvSpAssignmentResource_TryDeleteAdapter.ExecuteNonQuery(assignment.ID);
            }
        }

    }
}
