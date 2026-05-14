using System;
using Confirmit.CATI.Supervisor.Core.Quotas;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Quota;

namespace Confirmit.CATI.Supervisor.Core.Quotas.Fakes
{
    public class StubIQuotaSettingsProvider : IQuotaSettingsProvider 
    {
        private IQuotaSettingsProvider _inner;

        public StubIQuotaSettingsProvider()
        {
            _inner = null;
        }

        public IQuotaSettingsProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate QuotaPageViewSettings UpdateAndGetSettingsInt32Delegate(int surveySid);
        public UpdateAndGetSettingsInt32Delegate UpdateAndGetSettingsInt32;

        QuotaPageViewSettings IQuotaSettingsProvider.UpdateAndGetSettings(int surveySid)
        {


            if (UpdateAndGetSettingsInt32 != null)
            {
                return UpdateAndGetSettingsInt32(surveySid);
            } else if (_inner != null)
            {
                return ((IQuotaSettingsProvider)_inner).UpdateAndGetSettings(surveySid);
            }

            return default(QuotaPageViewSettings);
        }

    }
}