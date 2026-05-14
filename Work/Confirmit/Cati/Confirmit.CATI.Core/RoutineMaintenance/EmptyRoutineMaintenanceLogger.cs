using System;

namespace Confirmit.CATI.Core.RoutineMaintenance
{
    public class EmptyRoutineMaintenanceLogger : IRoutineMaintenanceLogger
    {
        public void AppendText(string text, TimeSpan elapsed, bool isNewLine)
        {
        }

        public void UpdateProgress(int total, int successful, int failed)
        {
        }
    }
}