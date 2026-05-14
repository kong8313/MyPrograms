using System;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement;
using Confirmit.CATI.Supervisor.Core.UserSettings;

namespace Confirmit.CATI.Supervisor.Core.UserSettings.Fakes
{
    public class StubIUserSettingsService : IUserSettingsService 
    {
        private IUserSettingsService _inner;

        public StubIUserSettingsService()
        {
            _inner = null;
        }

        public IUserSettingsService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate CallManagementColumnSettings GetUserGridSettingDelegate();
        public GetUserGridSettingDelegate GetUserGridSetting;

        CallManagementColumnSettings IUserSettingsService.GetUserGridSetting()
        {


            if (GetUserGridSetting != null)
            {
                return GetUserGridSetting();
            } else if (_inner != null)
            {
                return ((IUserSettingsService)_inner).GetUserGridSetting();
            }

            return default(CallManagementColumnSettings);
        }

        public delegate void SaveUserGridSettingsUserGridSettingsDelegate(CallManagementColumnSettings settings);
        public SaveUserGridSettingsUserGridSettingsDelegate SaveUserGridSettingsUserGridSettings;

        void IUserSettingsService.SaveUserGridSettings(CallManagementColumnSettings settings)
        {

            if (SaveUserGridSettingsUserGridSettings != null)
            {
                SaveUserGridSettingsUserGridSettings(settings);
            } else if (_inner != null)
            {
                ((IUserSettingsService)_inner).SaveUserGridSettings(settings);
            }
        }

    }
}