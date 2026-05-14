using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework
{
    public class UserProvider
    {
        public UserSettings GetUserSettings()
        {
            return new UserSettings(Properties.Settings.Default.Login, Properties.Settings.Default.Password);
        }
    }
}
