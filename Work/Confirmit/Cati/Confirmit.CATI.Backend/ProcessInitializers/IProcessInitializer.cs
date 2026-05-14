using System.Collections.Generic;

using Confirmit.CATI.Backend.WcfServices;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.ProcessInitializers
{
    /// <summary>
    /// Interface have to be used to initialize all process types.
    /// </summary>
    internal interface IProcessInitializer
    {
        /// <summary>
        /// Gets periodical threads process use.
        /// </summary>
        IEnumerable<IPeriodicalThread> PeriodicalThreads { get; }

        /// <summary>
        /// Gets wcf services process use.
        /// </summary>
        IEnumerable<IWcfServiceDescription> WcfServices { get; }

        /// <summary>
        /// Called inside Service thread.
        /// </summary>
        void InitializeService();

        /// <summary>
        /// Called inside Service thread.
        /// </summary>
        void UninitializeService();
    }
}
