using System.Linq;
using System.Threading;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.RoutineMaintenance;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.ExecuteRoutineMaintenance
{
    public class Operation : AsyncOperationWithoutEvent<Descriptor, Parameters>
    {
        private readonly RoutineMaintenanceService _routineMaintenanceService;

        public Operation(RoutineMaintenanceService routineMaintenanceService)
        {
            _routineMaintenanceService = routineMaintenanceService;
        }

        public override AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity, Parameters parameters,
            IAsyncOperationProgressLogger progressLogger, CancellationToken cancellationToken)
        {
            var logger = new AsyncOperationRoutineMaintenanceLogger(progressLogger, entity.Id);
            var maintenanceResult = _routineMaintenanceService.ExecuteMaintenance(logger, false, cancellationToken);

            return new AsyncOperationResult
            {
                Errors = maintenanceResult.Errors.ToList(),
                ProcessedItemsCount = maintenanceResult.SuccessfulActions,
                FailedItemsCount = maintenanceResult.FailedActions,
                State = GetAsyncOperationState(maintenanceResult.SuccessfulActions, maintenanceResult.FailedActions)
            };
        }

        private AsyncOperationState GetAsyncOperationState(int successful, int failed)
        {
            if (failed > 0)
            {
                if (successful == 0)
                {
                    return AsyncOperationState.Failed;
                }

                return AsyncOperationState.PartiallyCompleted;
            }

            return AsyncOperationState.Completed;
        }
    }
}
