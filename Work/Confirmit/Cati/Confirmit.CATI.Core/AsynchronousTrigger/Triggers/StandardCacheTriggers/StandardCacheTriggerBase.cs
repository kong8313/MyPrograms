using Confirmit.CATI.Core.AsynchronousTrigger.Messages;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Triggers.CacheTriggers.StandardCacheTriggers
{
    /// <summary>
    /// Base class for all cache triggers.
    /// All standard cached derives from StandardCacheTriggerBase.
    /// </summary>
    public abstract class StandardCacheTriggerBase : IAsynchronousTrigger
    {
        public abstract string CachedTableName
        {
            get;
        }

        public string TrigerName
        {
            get
            {
                return this.CachedTableName + " Asynchronous Trigger, Updates Standard Cache";
            }
        }

        public string TableName
        {
            get
            {
                return this.CachedTableName;
            }
        }

        /// <summary>
        /// Have to be implemented in cache object.
        /// Must invalidate/drop/release cache.
        /// </summary>
        public abstract void SetCacheExpired();

        public void Initialize()
        {
            this.OnTableChanged(null);
        }

        public void Uninitialize()
        {
            this.OnTableChanged(null);
        }

        public void OnTableChanged(TriggerMessage triggerMessage)
        {
            this.SetCacheExpired();
        }
    }
}
