namespace Confirmit.SystemTestFramework.Settings
{
    public class UserSettings
    {
        public UserSettings(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get; set; }
        public string Password { get; set; }
    }
}