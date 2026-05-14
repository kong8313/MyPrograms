using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.RoutineMaintenance
{
    public class RoutineMaintenanceResult
    {
        public int SuccessfulActions { get; set; }
        public int FailedActions { get; set; }
        public IReadOnlyCollection<Exception> Errors { get; set; } = new Exception[0];
    }
}