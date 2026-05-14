using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.Threads
{
    internal class PeriodicalThreadsManager : IPeriodicalThreadsManager
    {
        private IEnumerable<IPeriodicalThread> periodicalThreads;

        /// <summary>
        /// Starts all passed in periodical threads.
        /// </summary>
        /// <param name="threads">
        /// Threads to start.
        /// </param>
        public void Start(IEnumerable<IPeriodicalThread> threads)
        {
            if (this.periodicalThreads != null)
            {
                throw new InternalErrorException("PeriodicalThreadsManager.Start already called");
            }

            this.periodicalThreads = threads;

            foreach (var thread in this.periodicalThreads)
            {
                Trace.TraceInformation("Starting thread {0}...", thread.ThreadName);

                thread.Start();

                Trace.TraceInformation("Thread {0} started successfully", thread.ThreadName);
            }
        }

        /// <summary>
        /// Stops all passed in periodical threads.
        /// If exception occured while stopping particular thread then
        /// logs and stops other threads
        /// </summary>
        public void Stop()
        {
            if (this.periodicalThreads == null)
            {
                return;
            }

            Parallel.ForEach(this.periodicalThreads, (thread) =>
            {
                try
                {
                    Trace.TraceInformation("Stopping thread {0}...", thread.ThreadName);

                    thread.Stop();

                    Trace.TraceInformation("Thread {0} stopped successfully", thread.ThreadName);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(
                        "Exception occurred while stopping {0} periodical thread.\r\n\r\n{1}",
                        thread.ThreadName,
                        ex);
                }
            });
        }
    }
}
