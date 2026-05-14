using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

using Confirmit.CATI.Common.Logging;

namespace Confirmit.CATI.Core.ActivityLogging
{
    public class BaseEventDetails : IEventDetails
    {
        [XmlArrayItem(ElementName = "T")]
        public List<string> Timings { get; set; }
        [XmlArrayItem(ElementName = "M")]
        public List<string> Messages { get; set; }

        public const int DefaultMinimumTimingToIgnoreInMs = 50;

        /// <summary>
        /// Provides the ability to record custom timings.
        /// </summary>
        private readonly Stopwatch _timingsStopWatch = new Stopwatch();

        public BaseEventDetails()
        {
            Timings = new List<string>();
            Messages = new List<string>();
            _timingsStopWatch.Start();
        }

        public void AddTiming(string timingName)
        {
            AddTiming(timingName, DefaultMinimumTimingToIgnoreInMs);
        }

        public void AddTiming(string timingName, int minimumTimingToIgnore)
        {
            var timing = _timingsStopWatch.ElapsedMilliseconds;

            if (timing > minimumTimingToIgnore)
            {
                // Restarting only if we logged time not to loose less than milliseconds periods
                _timingsStopWatch.Restart();

                Timings.Add(timingName + ": " + timing);
            }
        }

        public void AddTiming(string format, params object[] args)
        {
            var name = String.Format(format, args);
            AddTiming(name);
        }

        public void AddMessage(string format, params object[] args)
        {
            Messages.Add(String.Format(format, args));
        }

        public TimeSpan GetElapsed()
        {
            return _timingsStopWatch.Elapsed;
        }
    }
}