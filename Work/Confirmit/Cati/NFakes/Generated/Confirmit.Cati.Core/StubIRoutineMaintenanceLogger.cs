using System;
using Confirmit.CATI.Core.RoutineMaintenance;

namespace Confirmit.CATI.Core.RoutineMaintenance.Fakes
{
    public class StubIRoutineMaintenanceLogger : IRoutineMaintenanceLogger 
    {
        private IRoutineMaintenanceLogger _inner;

        public StubIRoutineMaintenanceLogger()
        {
            _inner = null;
        }

        public IRoutineMaintenanceLogger Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AppendTextStringTimeSpanBooleanDelegate(string text, TimeSpan elapsed, bool isNewLine);
        public AppendTextStringTimeSpanBooleanDelegate AppendTextStringTimeSpanBoolean;

        void IRoutineMaintenanceLogger.AppendText(string text, TimeSpan elapsed, bool isNewLine)
        {

            if (AppendTextStringTimeSpanBoolean != null)
            {
                AppendTextStringTimeSpanBoolean(text, elapsed, isNewLine);
            } else if (_inner != null)
            {
                ((IRoutineMaintenanceLogger)_inner).AppendText(text, elapsed, isNewLine);
            }
        }

        public delegate void UpdateProgressInt32Int32Int32Delegate(int total, int successful, int failed);
        public UpdateProgressInt32Int32Int32Delegate UpdateProgressInt32Int32Int32;

        void IRoutineMaintenanceLogger.UpdateProgress(int total, int successful, int failed)
        {

            if (UpdateProgressInt32Int32Int32 != null)
            {
                UpdateProgressInt32Int32Int32(total, successful, failed);
            } else if (_inner != null)
            {
                ((IRoutineMaintenanceLogger)_inner).UpdateProgress(total, successful, failed);
            }
        }

    }
}