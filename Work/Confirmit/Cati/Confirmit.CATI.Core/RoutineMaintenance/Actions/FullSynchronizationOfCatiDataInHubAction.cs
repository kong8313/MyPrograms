using System;
using System.Runtime;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.RoutineMaintenance.Actions
{
    public class FullSynchronizationOfCatiDataInHubAction : IRoutineMaintenanceAction
    {
        private readonly IFullSynchronizationOfCatiDataInHubSettings _settings;
        public FullSynchronizationOfCatiDataInHubAction(IFullSynchronizationOfCatiDataInHubSettings settings)
        {
            _settings = settings;
        }

        public string Name => "Run mark hubs with CATI data for full synchronization";

        public RoutineMaintenanceShiftType ShiftType => (RoutineMaintenanceShiftType)_settings.ShiftType;
        public bool ExecuteForCompanySpecificInstance => true;
        public bool ExecuteForMasterInstance => false;

        public void Execute(RoutineMaintenanceShiftType curentShiftType)
        {
            var syncedHubs = BvHubDataChangeTrackingAdapter.GetAll();
            foreach (var bvHubDataChangeTrackingEntity in syncedHubs)
            {
                bvHubDataChangeTrackingEntity.ForceSyncBreaks = true;
                bvHubDataChangeTrackingEntity.ForceSyncCallHistory = true;
                bvHubDataChangeTrackingEntity.ForceSyncSessions = true;
                BvHubDataChangeTrackingAdapter.Update(bvHubDataChangeTrackingEntity);
            }
        }
    }
}