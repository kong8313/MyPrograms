using System.Diagnostics;

using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsynchronousTrigger.Messages;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.WcfServices.Clients;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Triggers
{
    /// <summary>
    /// Needed to enable/disable dialer on 2dn server when dialer enabled/disabled on 1st server.
    /// </summary>
    internal class BvDialersTrigger : IAsynchronousTrigger
    {
        private readonly IIpFilterCache _ipFilterCache;
        private readonly ITelephony _telephony;

        public string TrigerName
        {
            get
            {
                return TableName + " Asynchronous Trigger";
            }
        }

        public string TableName
        {
            get
            {
                return "BvDialers";
            }
        }

        public BvDialersTrigger(
            IIpFilterCache ipFilterCache,
            ITelephony telephony)
        {
            _ipFilterCache = ipFilterCache;
            _telephony = telephony;
        }

        public void Initialize()
        {
        }

        public void Uninitialize()
        {
        }

        public void OnTableChanged(TriggerMessage triggerMessage)
        {
            lock (this)
            {
                _ipFilterCache.Reset();
                EnableOrDisableDialersIfNeeded();
            }
        }

        /// <summary>
        /// So this function is called on any change in BvDialers table: 
        /// - a dialer was added
        /// - a dialer was removed
        /// - any dialer field was changed: dialer name, connection parameters, Dialer state notification
        /// In any case we must reload the cached collection of dialers.
        /// </summary>
        internal void EnableOrDisableDialersIfNeeded()
        {
            lock (this)
            {
                _telephony.UpdateDialersCollection();
            }
        }
    }
}