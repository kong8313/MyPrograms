using System;
using System.Diagnostics;
using Confirmit.CATI.Backend.ProcessInitializers;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Backend
{
    public class Host
    {
        private IProcessInitializer _processInitializer;

        public void OnStart()
        {
            Trace.TraceInformation("Confirmit.CATI.Backend.BackendWin32Service.OnStart");

            try
            {
                var evt = new StartMultimodeInstanceEvent();

                using (new EventDetailsScope(evt.Details))
                {
                    IProcessInitializerFactory processInitializerFactory = new ProcessInitializerFactory();

                    EventDetailsScope.Current.AddTiming("BackendWin32Service.CreateInitializerFactory");

                    _processInitializer = processInitializerFactory.CreateProcessInitializer(BackendInstance.Current.CompanyId);

                    EventDetailsScope.Current.AddTiming("BackendWin32Service.CreateProcessInitializer");

                    _processInitializer.InitializeService();

                    EventDetailsScope.Current.AddTiming("BackendWin32Service.InitializeService");

                    evt.Finish();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Confirmit.CATI.Backend.BackendWin32Service.OnStart: Failed\r\n\r\nException:\r\n{0}", ex);

                _processInitializer.UninitializeService();

                throw;
            }

            Trace.TraceInformation("Confirmit.CATI.Backend.BackendWin32Service.OnStart: Successfully Finished.");
        }

        public void OnStop()
        {
            try
            {
                Trace.TraceInformation("Confirmit.CATI.Backend.BackendWin32Service.OnStop");

                var evt = new StopMultimodeInstanceEvent();

                using (new EventDetailsScope(evt.Details))
                {
                    _processInitializer.UninitializeService();

                    evt.Finish();
                }

                Trace.TraceInformation("Confirmit.CATI.Backend.BackendWin32Service.OnStop: successfully finished.");
            }
            catch (Exception ex)
            {
                try
                {
                    Trace.TraceError("Confirmit.CATI.Backend.BackendWin32Service.OnStop: Failed.\r\n\r\nException:\r\n{0}", ex);
                }
                catch 
                { 
                    // Ignore all errors on stopping service to be able to stop it in any case
                }
            }
            
        }
    }
}
