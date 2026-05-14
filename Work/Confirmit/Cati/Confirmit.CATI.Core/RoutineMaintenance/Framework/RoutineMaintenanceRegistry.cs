using Confirmit.CATI.Core.RoutineMaintenance.Actions;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.RoutineMaintenance.Framework
{
    public class RoutineMaintenanceRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator
                .Register<IRoutineMaintenanceAction, CleanAnswerSubmissionAlertHistoryTableAction>("Action.CleanAnswerSubmissionAlertHistoryTableAction")
                .Register<IRoutineMaintenanceAction, CleanAsyncOperationQueueTableAction>("Action.CleanAsyncOperationQueueTableAction")
                .Register<IRoutineMaintenanceAction, CleanCallsSentToDialerTableAction>("Action.CleanCallsSentToDialerTableAction")
                .Register<IRoutineMaintenanceAction, CleanAssignmentResourceTableAction>("Action.CleanAssignmentResourceTableAction")
                .Register<IRoutineMaintenanceAction, CleanMessageTableAction>("Action.CleanMessageTableAction")
                .Register<IRoutineMaintenanceAction, CleanUserSurveyListTableAction>("Action.CleanUserSurveyListTableAction")
                .Register<IRoutineMaintenanceAction, CleanUnusedSurveysAction>("Action.CleanUnusedSurveysAction")
                .Register<IRoutineMaintenanceAction, CleanPersonDeferredMonitoringTableAction>("Action.CleanPersonDeferredMonitoringTableAction")
                .Register<IRoutineMaintenanceAction, CleanPromotionHistoryTableAction>("Action.CleanPromotionHistoryTableAction")
                .Register<IRoutineMaintenanceAction, CleanCallHistoryTableAction>("Action.CleanCallHistoryTableAction")
                .Register<IRoutineMaintenanceAction, DatabaseMaintenanceAction>("Action.DatabaseMaintenanceAction")
                .Register<IRoutineMaintenanceAction, CleanSchedulingScriptLogTableAction>("Action.CleanSchedulingScriptLogTableAction")
                .Register<IRoutineMaintenanceAction, CleanActiveSupervisorsAction>("Action.CleanActiveSupervisorsAction")
                .Register<IRoutineMaintenanceAction, LargeObjectHeapFragmentationAction>("Action.LargeObjectHeapFragmentationAction")
                .Register<IRoutineMaintenanceAction, FullSynchronizationOfCatiDataInHubAction>("Action.FullSynchronizationOfCatiDataInHubAction")
                .Register<IRoutineMaintenanceAction, CleanActiveDialAction>("Action.CleanActiveDialAction")
                .Register<IRoutineMaintenanceAction, DeferredRecordsMigration>("Action.DeferredRecordsMigration")
                .Register<IRoutineMaintenanceShiftService, RoutineMaintenanceShiftService>()
                .Register<RoutineMaintenanceService>();
        }
    }
}
