using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Threading;
using System;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.RoutineMaintenance;

namespace Confirmit.CATI.Backend.Threads
{
    public class RoutineMaintenanceThread : PeriodicalThread
    {
        private readonly IRoutineMaintenanceSettings _settings;
        private readonly IMultimodeInstanceName _multimodeInstanceName;
        private readonly IConnectionStrings _connectionStrings;
        private readonly IAsyncOperationQueue _asyncOperationQueue;
        private readonly ISideBySideManager _sideBySideManager;
        private readonly IAsyncOperationAwaiter _asyncOperationAwaiter;
        private readonly RoutineMaintenanceService _routineMaintenanceService;

        public override TimeSpan StopTimeout
        {
            get { return TimeSpan.FromSeconds(30); }
        }

        public override TimeSpan SleepTimeout
        {
            get { return _settings.FrequencyExecution; }
        }

        public RoutineMaintenanceThread(
            IRoutineMaintenanceSettings settings,
            IMultimodeInstanceName multimodeInstanceName,
            IConnectionStrings connectionStrings,
            IAsyncOperationQueue asyncOperationQueue,
            ISideBySideManager sideBySideManager,
            IAsyncOperationAwaiter asyncOperationAwaiter,
            RoutineMaintenanceService routineMaintenanceService
        )
            : base("RoutineMaintenanceThread")
        {
            _settings = settings;
            _multimodeInstanceName = multimodeInstanceName;
            _connectionStrings = connectionStrings;
            _asyncOperationQueue = asyncOperationQueue;
            _sideBySideManager = sideBySideManager;
            _asyncOperationAwaiter = asyncOperationAwaiter;
            _routineMaintenanceService = routineMaintenanceService;
        }

        private void RunRoutineMaintenanceForMasterInstance()
        {
            // For master instance we execute maintenance directly in this thread
            try
            {
                _routineMaintenanceService.ExecuteMaintenance(new EmptyRoutineMaintenanceLogger(), true, CancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                TraceHelper.TraceException(e);
            }
        }
        
        public void RunRoutineMaintenanceForCompanies()
        {
            // For company-specific instances we schedule async operations that are executed in the context of a corresponding instance
            var companyIds =
                BvBackendInstanceAdapter.GetAll()
                    .Select(s => _sideBySideManager.AddSideBySideNameToServiceName(s.ServiceName))
                    .Select(serviceName => _multimodeInstanceName.ServiceNameToCompanyId(serviceName));

            foreach (var companyId in companyIds)
            {
                var connectionString = _connectionStrings.GetConnectionStringForSpecificCompany(companyId);
                using (new ConnectionScope(connectionString))
                {
                    var operationEntity = ExecuteAsyncOperation();
                    _asyncOperationAwaiter.Await(operationEntity);
                }
            }
        }

        protected override void DoWork(object parameter)
        {
             
            RunRoutineMaintenanceForMasterInstance();
            RunRoutineMaintenanceForCompanies();
        }

        private BvAsyncOperationQueueEntity ExecuteAsyncOperation()
        {
            var parameters = new Core.AsyncOperations.Operations.ExecuteRoutineMaintenance.Parameters();

            return _asyncOperationQueue.Enqueue(
                0,
                "ExecuteRoutineMaintenance",
                true,
                parameters,
                AsyncOperationConstants.NormalPriority,
                "system"
                );
        }
    }
}
