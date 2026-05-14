using System;
using System.Diagnostics;

using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.ActivityLogging
{
    /// <summary>
    /// Provides the common methods and fields for all management activity log events.
    /// </summary>
    public abstract class ManagementActivityEvent<TDetails> : ManagementActivityEventBase<TDetails>, IManagementActivityEvent where TDetails : ManagementActivityEventDetails, new()
    {
        public bool WithMetric { get; set; }

        /// <summary>
        /// Provides the ability to measure the duration of the event activity.
        /// </summary>
        private readonly Stopwatch _stopWatch = new Stopwatch();

        /// <summary>
        /// Flag indicates that activity event should be committed on Finish method call.
        /// </summary>
        private bool _shouldBeCommitedOnDispose;

        private TimeSpan? _duration;

        /// <summary>
        /// Gets or sets the duration of activity event.
        /// </summary>
        public TimeSpan Duration
        {
            get { return _duration ?? _stopWatch.Elapsed; }
            set { _duration = value; }
        }

        /// <summary>
        /// Initializes a new instance of the ManagementActivityEvent class.
        /// Starts the activity duration counter.
        /// </summary>
        internal ManagementActivityEvent(ManagementEventCategory category, ManagementEvent eventType, bool withMetric = false)
            : this(category, eventType, ServiceLocator.Resolve<ISupervisorNameProvider>().Name, withMetric)
        {
        }

        internal ManagementActivityEvent(ManagementEventCategory category, ManagementEvent eventType, string supervisor, bool withMetric = false):
            base( category, eventType)
        {
            Initialize(supervisor, withMetric);
        }

        internal void Initialize(string supervisor, bool withMetric)
        {
            StartTime = DateTime.UtcNow;

            _stopWatch.Start();

            CompanyId = BackendInstance.Current.CompanyId;

            ServerName = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>().MachineName;

            Supervisor = supervisor;
            
            WithMetric = withMetric;

            RegisterEventForCommit();
        }

        /// <summary>
        /// Registers the event for commit in <see cref="DatabaseTransactionScope"/> or sets the manual commit flag if event
        /// supports it.
        /// </summary>
        /// <exception cref="InvalidOperationException">Current activity event is not intended to be used outside 
        /// <see cref="DatabaseTransactionScope"/> and should be wrapped in it.</exception>
        private void RegisterEventForCommit()
        {
            if (DatabaseTransactionScope.Current != null)
            {
                DatabaseTransactionScope.Current.AddActivityEvent(this);
            }
            else
            {
                _shouldBeCommitedOnDispose = true;
            }
        }

        /// <summary>
        /// Finishes the activity event and stops the event duration counter.
        /// </summary>
        public void Finish()
        {
            _stopWatch.Stop();

            var settings = ServiceLocator.Resolve<ISystemSettings>();
            if (_stopWatch.ElapsedMilliseconds < settings.ActivityLogging.ManagementActivityEventTimingsThreshold.TotalMilliseconds ||
                Details.Timings.Count == 0)
            {
                Details.Timings = null;
            }

            if (_shouldBeCommitedOnDispose)
            {
                Save();
            }
        }

        /// <summary>
        /// Determines whether this event is currently measuring the duration.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRunning()
        {
            return _stopWatch.IsRunning;
        }

        public void AddTiming(string timingName)
        {
            Details.AddTiming(timingName);
        }

        /// <summary>
        /// Saves the event details to DB.
        /// </summary>
        public void Save()
        {
            SaveToKibana((int)Duration.TotalMilliseconds);
            Save(BackendInstance.Current.ConfirmlogConnectionString, (int)Duration.TotalMilliseconds);

            if (WithMetric)
            {
                SaveMetric();
            }
        }

        public void SaveMetric()
        {
            var eventName = GetType().Name;
            if (eventName.EndsWith("Event", StringComparison.OrdinalIgnoreCase))
            {
                // remove Event suffix
                eventName = eventName.Substring(0, eventName.Length - 5);
            }
            
            CustomMetrics.OnActivityEvent("Management", eventName, _stopWatch.Elapsed);
        }
    }
}
