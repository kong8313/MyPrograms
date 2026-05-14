using Confirmit.CATI.Core.AsynchronousTrigger.Messages;
using Confirmit.CATI.Core.DAL.Handmade.Cache;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Triggers.CacheTriggers.CustomCacheTriggers.SystemSettingTriggers
{
    /// <summary>
    /// Needed to reset cache of CF variable info on launch/relaunch survey.
    /// BvSurvey updated every time we launch/relaunch survey so trigger fired.
    /// The cache used in scheduling script in 'f'/'fr' functions.
    /// </summary>
    internal class BvSystemSettingTrigger : IAsynchronousTrigger
    {
        public string TrigerName
        {
            get
            {
                return this.TableName + " Asynchronous Trigger, Updates 'Survey Schema Cache'";
            }
        }

        public string TableName
        {
            get
            {
                return "BvSystemSettings";
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
            ServiceLocator.Resolve<ISystemSettingCache>().Reset();
        }
    }
}
