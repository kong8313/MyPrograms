using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using System.Threading;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class CleanPersonDeferredMonitoringTableAction : IRoutineMaintenanceAction
    {
        private readonly IPersonDeferredMonitoringTableCleanupSettings _settings;

        public CleanPersonDeferredMonitoringTableAction(
            IPersonDeferredMonitoringTableCleanupSettings settings)
        {
            _settings = settings;
        }

        public string Name
        {
            get { return "Clean PersonDeferredMonitoring table."; }
        }

        public RoutineMaintenanceShiftType ShiftType
        {
            get { return (RoutineMaintenanceShiftType)_settings.ShiftType; }
        }

        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            int deletedRows;
            do
            {
                using (var transaction = new DatabaseTransactionScope("CleanDeferredMonitoring", DeadlockPriority.PeriodicalThread))
                {
                    BvSpCleanDeferredMonitoringAdapter.ExecuteNonQuery(
                        (int)_settings.ExpirationPeriod.TotalDays,
                        _settings.DeleteTopRows,
                        out deletedRows
                        );

                    transaction.Commit();
                }

                if (deletedRows != 0)
                {
                    Thread.Sleep(
                       (int)_settings.DelayBetweenDeletes.TotalMilliseconds
                       );
                }
            } while (deletedRows > 0);
        }

    }
}
