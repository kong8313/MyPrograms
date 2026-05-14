using System;
using Confirmit.CATI.Core.Threading;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIAsyncOperationsHeartBeatUpdaterThread : IAsyncOperationsHeartBeatUpdaterThread 
    {
        private IAsyncOperationsHeartBeatUpdaterThread _inner;

        public StubIAsyncOperationsHeartBeatUpdaterThread()
        {
            _inner = null;
        }

        public IAsyncOperationsHeartBeatUpdaterThread Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void StartDelegate();
        public StartDelegate Start;

        void IPeriodicalThread.Start()
        {

            if (Start != null)
            {
                Start();
            } else if (_inner != null)
            {
                ((IPeriodicalThread)_inner).Start();
            }
        }

        public delegate void StartObjectDelegate(Object parameter);
        public StartObjectDelegate StartObject;

        void IPeriodicalThread.Start(Object parameter)
        {

            if (StartObject != null)
            {
                StartObject(parameter);
            } else if (_inner != null)
            {
                ((IPeriodicalThread)_inner).Start(parameter);
            }
        }

        public delegate bool WaitInt32Delegate(int period);
        public WaitInt32Delegate WaitInt32;

        bool IPeriodicalThread.Wait(int period)
        {


            if (WaitInt32 != null)
            {
                return WaitInt32(period);
            } else if (_inner != null)
            {
                return ((IPeriodicalThread)_inner).Wait(period);
            }

            return default(bool);
        }

        public delegate bool WaitTimeSpanDelegate(TimeSpan period);
        public WaitTimeSpanDelegate WaitTimeSpan;

        bool IPeriodicalThread.Wait(TimeSpan period)
        {


            if (WaitTimeSpan != null)
            {
                return WaitTimeSpan(period);
            } else if (_inner != null)
            {
                return ((IPeriodicalThread)_inner).Wait(period);
            }

            return default(bool);
        }

        public delegate void StopDelegate();
        public StopDelegate Stop;

        void IPeriodicalThread.Stop()
        {

            if (Stop != null)
            {
                Stop();
            } else if (_inner != null)
            {
                ((IPeriodicalThread)_inner).Stop();
            }
        }

        public delegate void OnStopDelegate();
        public OnStopDelegate OnStop;

        void IPeriodicalThread.OnStop()
        {

            if (OnStop != null)
            {
                OnStop();
            } else if (_inner != null)
            {
                ((IPeriodicalThread)_inner).OnStop();
            }
        }

        public delegate void UpdateRunningOperationsHeartBeatDelegate();
        public UpdateRunningOperationsHeartBeatDelegate UpdateRunningOperationsHeartBeat;

        void IAsyncOperationsHeartBeatUpdaterThread.UpdateRunningOperationsHeartBeat()
        {

            if (UpdateRunningOperationsHeartBeat != null)
            {
                UpdateRunningOperationsHeartBeat();
            } else if (_inner != null)
            {
                ((IAsyncOperationsHeartBeatUpdaterThread)_inner).UpdateRunningOperationsHeartBeat();
            }
        }

        private string _ThreadName;
        public Func<string> ThreadNameGet;
        public Action<string> ThreadNameSetString;

        string IPeriodicalThread.ThreadName
        {
            get
            {
                if (ThreadNameGet != null)
                {
                    return ThreadNameGet();
                } else if (_inner != null)
                {
                    return ((IPeriodicalThread)_inner).ThreadName;
                }

                if (ThreadNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ThreadName;
                }

                return default(string);
            }

        }

        private TimeSpan _StopTimeout;
        public Func<TimeSpan> StopTimeoutGet;
        public Action<TimeSpan> StopTimeoutSetTimeSpan;

        TimeSpan IPeriodicalThread.StopTimeout
        {
            get
            {
                if (StopTimeoutGet != null)
                {
                    return StopTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IPeriodicalThread)_inner).StopTimeout;
                }

                if (StopTimeoutSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _StopTimeout;
                }

                return default(TimeSpan);
            }

        }

        private TimeSpan _SleepTimeout;
        public Func<TimeSpan> SleepTimeoutGet;
        public Action<TimeSpan> SleepTimeoutSetTimeSpan;

        TimeSpan IPeriodicalThread.SleepTimeout
        {
            get
            {
                if (SleepTimeoutGet != null)
                {
                    return SleepTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IPeriodicalThread)_inner).SleepTimeout;
                }

                if (SleepTimeoutSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SleepTimeout;
                }

                return default(TimeSpan);
            }

        }

        private ISystemSettings _SystemSettings;
        public Func<ISystemSettings> SystemSettingsGet;
        public Action<ISystemSettings> SystemSettingsSetISystemSettings;

        ISystemSettings IPeriodicalThread.SystemSettings
        {
            get
            {
                if (SystemSettingsGet != null)
                {
                    return SystemSettingsGet();
                } else if (_inner != null)
                {
                    return ((IPeriodicalThread)_inner).SystemSettings;
                }

                if (SystemSettingsSetISystemSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SystemSettings;
                }

                return default(ISystemSettings);
            }

        }

        private ISideBySideManager _SideBySideManager;
        public Func<ISideBySideManager> SideBySideManagerGet;
        public Action<ISideBySideManager> SideBySideManagerSetISideBySideManager;

        ISideBySideManager IPeriodicalThread.SideBySideManager
        {
            get
            {
                if (SideBySideManagerGet != null)
                {
                    return SideBySideManagerGet();
                } else if (_inner != null)
                {
                    return ((IPeriodicalThread)_inner).SideBySideManager;
                }

                if (SideBySideManagerSetISideBySideManager == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SideBySideManager;
                }

                return default(ISideBySideManager);
            }

        }

    }
}