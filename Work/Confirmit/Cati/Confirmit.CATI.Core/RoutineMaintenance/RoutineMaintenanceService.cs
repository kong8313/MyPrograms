using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.RoutineMaintenance
{
    public class RoutineMaintenanceService
    {
        private readonly IRoutineMaintenanceAction[] _routineMaintenanceActions;
        private readonly IRoutineMaintenanceShiftService _routineMaintenanceShiftService;
        private readonly IRoutineMaintenanceSettings _settings;
        private readonly ITimeService _timeService;
        
        public RoutineMaintenanceService(IRoutineMaintenanceAction[] routineMaintenanceActions, 
            IRoutineMaintenanceShiftService routineMaintenanceShiftService,
            IRoutineMaintenanceSettings settings,
            ITimeService timeService)
        {
            _routineMaintenanceActions = routineMaintenanceActions;
            _routineMaintenanceShiftService = routineMaintenanceShiftService;
            _settings = settings;
            _timeService = timeService;
        }

        public RoutineMaintenanceResult ExecuteMaintenance(IRoutineMaintenanceLogger progressLogger, bool isMasterInstance, CancellationToken cancellationToken)
        {
            using (var dbLock = DatabaseLockService.CreatePeriodicalLock(
                DatabaseLockTimeoutsAndRecourceNames.RoutingMaintenanceResourceName,
                "Operations.ExecuteRoutineMaintenance",
                (int) _settings.Duration.TotalMilliseconds))
            {
                if (!dbLock.TryEnterLock())
                {
                    return new RoutineMaintenanceResult();
                }

                var currentShiftType = _routineMaintenanceShiftService.GetMatchedShiftType(_timeService.GetUtcNow());

                var actions = GetActionsForExecution(currentShiftType, isMasterInstance);
                
                int total = actions.Length;
                int successful = 0;
                int failed = 0;
                var errors = new List<Exception>();
                var time = Stopwatch.StartNew();

                var evt = new RoutineMaintenanceEvent();

                progressLogger.AppendText( $"Count of actions to execute: '{actions.Length}'", time.Elapsed, false);

                using (new EventDetailsScope(evt.Details))
                {
                    foreach (var action in actions)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            failed = total - successful;
                            progressLogger.AppendText("Cancelling routine maintenance", time.Elapsed, true);
                            evt.AddTiming("Cancelling routine maintenance");
                            break;
                        }

                        progressLogger.AppendText($"Action '{action.Name}': Starting...", time.Elapsed, true);
                        evt.AddTiming($"Action '{action.Name}': Starting...");

                        try
                        {
                            action.Execute(currentShiftType);

                            successful++;

                            progressLogger.AppendText($"Action '{action.Name}': Complete successful.", time.Elapsed, true);
                            evt.AddTiming($"Action '{action.Name}': Complete successful.");
                        }
                        catch (Exception ex)
                        {
                            failed++;
                            errors.Add(ex);

                            progressLogger.AppendText($"Action '{action.Name}': Execution failed.", time.Elapsed, true);
                            evt.AddTiming($"Action '{action.Name}': Execution failed.");

                            TraceHelper.TraceException(ex, $"An error occured during execution of {action.Name} action.");
                        }

                        progressLogger.UpdateProgress(total, successful, failed);
                    }
                }

                if (total > 0)
                {
                    evt.Finish();
                }

                return new RoutineMaintenanceResult()
                {
                    SuccessfulActions = successful,
                    FailedActions = failed,
                    Errors = errors
                };
            }
        }

        private IRoutineMaintenanceAction[] GetActionsForExecution(RoutineMaintenanceShiftType shiftType,
            bool isMasterInstance)
        {
            var actions = _routineMaintenanceActions
                .Where(action => _routineMaintenanceShiftService.IsShiftTypeHitToAnother(action.ShiftType, shiftType))
                .ToArray();

            return actions.Where(x
                => isMasterInstance && x.ExecuteForMasterInstance
                || !isMasterInstance && x.ExecuteForCompanySpecificInstance).ToArray();
        }
    }
}