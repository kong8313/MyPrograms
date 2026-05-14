using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.WaitingService;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Threading
{
    public abstract class PeriodicalThread : IPeriodicalThread
    {
        private readonly Thread _thread;
        protected readonly ManualResetEvent StopEvent;
        protected CancellationTokenSource CancellationTokenSource;   

        public abstract TimeSpan StopTimeout { get; }

        public abstract TimeSpan SleepTimeout { get; }
        private TimeSpan _sleepTimeout = TimeSpan.FromSeconds(0);

        private TimeSpan _sleepTimeoutAfterException = TimeSpan.FromSeconds(60);

        private IWaitingService _waitingService;

        private readonly PeriodicalThreadSettings _periodicalThreadSettings;

        private TimeSpan GetSleepTimeout()
        {
            try
            {
                _sleepTimeout = SleepTimeout;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }

            return _sleepTimeout;
        }

        public string ThreadName
        {
            get
            {
                return _thread.Name;
            }
        }

        public ISystemSettings SystemSettings { get; private set; }
        public ISideBySideManager SideBySideManager { get; private set; }

        protected PeriodicalThread( 
            string threadName )
        {
            SideBySideManager = ServiceLocator.Resolve<ISideBySideManager>();
            SystemSettings = ServiceLocator.Resolve<ISystemSettings>();
            _waitingService = ServiceLocator.Resolve<IWaitingService>();
            _periodicalThreadSettings = ServiceLocator.Resolve<PeriodicalThreadSettings>();
            
            StopEvent = new ManualResetEvent( false );
            
            _thread = new Thread( InternalThreadProc )
            {
                Name = threadName
            };
        }

        /// <summary>
        /// Starts thread.
        /// </summary>
        public void Start()
        {
            Start( null );
        }

        /// <summary>
        /// Starts thread and pass single custom parameter.
        /// </summary>
        /// <param name="parameter"></param>
        public void Start(object parameter)
        {
            if (!_thread.IsAlive)
            {
                CancellationTokenSource = new CancellationTokenSource();
                _thread.Start(parameter);
            }
            else
            {
                throw new InvalidOperationException(String.Format("thread '{0}' has been already started", ThreadName));
            }
        }

        public bool Wait(int period)
        {
            return _waitingService.Wait(StopEvent, period);
        }

        public bool Wait(TimeSpan period)
        {
            return _waitingService.Wait(StopEvent, period);
        }

        /// <summary>
        /// Stops thread.
        /// </summary>
        public void Stop()
        {
            if ( !_thread.IsAlive )
            {
                throw new InvalidOperationException(String.Format(
                    "Thread '{0}' has been already stopped",
                    ThreadName ) );
            }

            StopEvent.Set();
            CancellationTokenSource.Cancel();
            if (!_thread.Join(StopTimeout))
            {
                Trace.TraceError(
                    "Thread '{0}' not stopped during {1} milliseconds out. Thread '{0}' aborted.",
                    ThreadName,
                    StopTimeout);

                _thread.Abort();
            }

            StopEvent.Reset(); // thread can be restarted
        }

        internal void InternalThreadProc(object parameter)
        {
            try
            {
                do
                {
                    try
                    {
                        if (!_periodicalThreadSettings.IsCurrentCompanySuspended())
                            DoWork(parameter);
                        else
                            Trace.TraceInformation("Periodical  thread '{0}' suspended due to the configuration of SuspendStartingAsyncOperations and SuspendStartingAsyncOperationsAllowCompanies settings", ThreadName);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Error in PeriodicalThread with name '{0}'. Exception details:\r\n{1}", ThreadName, e.ToString());
                        Wait(_sleepTimeoutAfterException);
                    }
                }
                while (Wait(GetSleepTimeout()));

                OnStop();
            }
            catch ( Exception e )
            {
                Trace.TraceError( e.ToString() );
            }
        }

        protected abstract void DoWork( object parameter );

        public virtual void OnStop()
        {
        }

        protected void SetThreadCulture(CultureInfo cultureInfo)
        {
            _thread.CurrentCulture = cultureInfo;
            _thread.CurrentUICulture = cultureInfo;
        }
    }
}
