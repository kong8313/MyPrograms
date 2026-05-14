using System;
using Confirmit.CATI.Core.SystemSettings.Supervisor;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Supervisor.Fakes
{
    public class StubISurveyListSettingsGroup : ISurveyListSettingsGroup 
    {
        private ISurveyListSettingsGroup _inner;

        public StubISurveyListSettingsGroup()
        {
            _inner = null;
        }

        public ISurveyListSettingsGroup Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnChangedDelegate();
        public OnChangedDelegate OnChanged;

        void ISystemSettingsNotifyChanged.OnChanged()
        {

            if (OnChanged != null)
            {
                OnChanged();
            } else if (_inner != null)
            {
                ((ISystemSettingsNotifyChanged)_inner).OnChanged();
            }
        }

        private bool _ShowTciDialerCampaignIdColumn;
        public Func<bool> ShowTciDialerCampaignIdColumnGet;
        public Action<bool> ShowTciDialerCampaignIdColumnSetBoolean;

        bool ISurveyListSettings.ShowTciDialerCampaignIdColumn
        {
            get
            {
                if (ShowTciDialerCampaignIdColumnGet != null)
                {
                    return ShowTciDialerCampaignIdColumnGet();
                } else if (_inner != null)
                {
                    return ((ISurveyListSettings)_inner).ShowTciDialerCampaignIdColumn;
                }

                if (ShowTciDialerCampaignIdColumnSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ShowTciDialerCampaignIdColumn;
                }

                return default(bool);
            }

            set
            {
                if (ShowTciDialerCampaignIdColumnSetBoolean != null)
                {
                    ShowTciDialerCampaignIdColumnSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISurveyListSettings)_inner).ShowTciDialerCampaignIdColumn = value;
                    return;
                }

                if (ShowTciDialerCampaignIdColumnGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ShowTciDialerCampaignIdColumn = value;
                }

            }
        }

    }
}