namespace CatiInstallation
{
    public class BackendParameters
    {
        public string CatiSqlServerName { get; set; }
        public string CatiSqlAdminUserName { get; set; }
        public string CatiSqlAdminPassword { get; set; }
        public string CatiSqlUserName { get; set; }
        public string CatiSqlPassword { get; set; }
        public string CatiConnectionString { get; set; }
        public string TypeOfActionWithDatabase { get; set; }
        public string CatiDefaultDbRecoveryModel { get; set; }
        public string ConfirmSqlServerName { get; set; }
        public string ConfirmUserName { get; set; }
        public string ConfirmConnectionString { get; set; }
        public string ConfirmlogConnectionString { get; set; }
        public string ConfirmPassword { get; set; }
        public string IsLoadBalancedEnvironment { get; set; }
        public string CertificateType { get; set; }
        public string TestCertificateName { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }
        public string CatiDatabasesDataFilePath { get; set; }
        public string CatiDatabasesLogsFilePath { get; set; }
        public string NotificationEmailBCC { get; set; }
        public string ConfirmitLinkedServerName { get; set; }
        public string MsiInstallLocation { get; set; }
        public string LoadBalancerIsAlivePageUrl { get; set; }
        public string LoadBalancerIsAlivePageRenameTimeout { get; set; }
    }
}
