namespace CatiInstallation
{
    public class SupervisorParameters
    {
        public string CatiSqlServerName { get; set; }
        public string CatiSqlAdminUserName { get; set; }
        public string CatiSqlAdminPassword { get; set; }

        public string SessionStateServerName { get; set; }
        public string SessionStateUserName { get; set; }
        public string SessionStatePassword { get; set; }
        public string SessionStateConnectionString { get; set; }
        public string SessionStateMode { get; set; }
        public string RedisHostName { get; set; }
        public string SessionStateCookieName { get; set; }

        public string SupervisorVirtualDirectoryName { get; set; }
        public string SupervisorAppPoolName { get; set; }
        public string SupervisorSiteName { get; set; }
    }
}
