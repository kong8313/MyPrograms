using System.Collections.Generic;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.Threads
{
    internal interface IPeriodicalThreadsManager
    {
        /// <summary>
        /// Starts all passed in periodical threads.
        /// </summary>
        void Start(IEnumerable<IPeriodicalThread> threads);

        /// <summary>
        /// Stops all started previously periodical threads.
        /// If exception occured while stopping particular thread then
        /// logs and stops other threads.
        /// </summary>
        void Stop();
    }
}