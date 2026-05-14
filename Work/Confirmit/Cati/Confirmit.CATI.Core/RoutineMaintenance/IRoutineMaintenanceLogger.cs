using System;

namespace Confirmit.CATI.Core.RoutineMaintenance
{
    public interface IRoutineMaintenanceLogger
    {
        void AppendText(string text, TimeSpan elapsed, bool isNewLine);
        void UpdateProgress(int total, int successful, int failed);
    }
}