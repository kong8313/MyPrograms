namespace SimulatorDialerDriver.Models
{
    public class RegisteredSoftphoneAgent
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Extension { get; set; }
        public string FrontendUrl { get; set; }

        public RegisteredSoftphoneAgent(string login, string password, string host, string extension, string frontendUrl)
        {
            Login = login;
            Password = password;
            Host = host;
            Extension = extension;
            FrontendUrl = frontendUrl;
        }
    }
}
