using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsynchronousTrigger.Messages;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Triggers.CacheTriggers.CustomCacheTriggers.ScheduleTriggers
{
    /// <summary>
    /// Base class for all triggers implemented to drop custom 'Schedule Cache'.
    /// See <see cref="IShiftServiceFactory.DropScheduleCache"/>
    /// </summary>
    internal abstract class ScheduleTriggerBase
    {
        private readonly IShiftServiceFactory _shiftServiceFactory;

        public ScheduleTriggerBase()
        {
            _shiftServiceFactory = ServiceLocator.Resolve<IShiftServiceFactory>();
        }

        public abstract string TableName
        {
            get;
        }

        public string TrigerName
        {
            get
            {
                return this.TableName + " Asynchronous Trigger, Updates 'Schedule/Shifts Cache'";
            }
        }

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
            _shiftServiceFactory.DropScheduleCache();
        }
    }
}
