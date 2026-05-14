. "Installation\InitialInitializations.ps1"
. "Installation\PreDeployFunctions.ps1"
. "Installation\DeployFunctions.ps1"

# Load external C# dll's
[Reflection.Assembly]::LoadFile("$installLocation\Installation\CatiInstallation.dll")

$settings = @{
    'Cati.Database.ServerName' = $CatiDatabaseServerName
    'Confirmit.Database.User.SystemAdmin.Name' = $ConfirmitDatabaseUserSystemAdminName
    'Confirmit.Database.User.SystemAdmin.Password' = $ConfirmitDatabaseUserSystemAdminPassword
    'Confirmit.Database.User.DeployC.Name' = $ConfirmitDatabaseUserDeployCName
    'Confirmit.Database.User.DeployC.Password' = $ConfirmitDatabaseUserDeployCPassword
    'Cati.Database.LinkedServerNameToConfirmit' = $CatiDatabaseLinkedServerNameToConfirmit
    'Cati.Database.UseSimpleRecoveryMode' = $CatiDatabaseUseSimpleRecoveryMode
    'Confirmit.Database.Server.System.ServerName' = $ConfirmitDatabaseServerSystemServerName
    'Cati.Database.Server.DataPath' = $CatiDatabaseServerDataPath
    'Cati.Database.Server.LogPath' = $CatiDatabaseServerLogPath
    'Cati.Parameters.ValidWCFIpAddresses' = $CatiParametersValidWCFIpAddresses
    'Cati.Parameters.NotificationEmailBcc' = $CatiParametersNotificationEmailBcc
    'Cati.SSL.GenerateTestCertificate' = $CatiSSLGenerateTestCertificate
    'Cati.SSL.TestCertificateName' = $CatiSSLTestCertificateName
    'Cati.SSL.CertificatePath' = $CatiSSLCertificatePath
    'Cati.SSL.CertificatePassword' = $CatiSSLCertificatePassword
    'Cati.LoadBalancer.UseLoadBalancer' = $CatiLoadBalancerUseLoadBalancer
    'Cati.LoadBalancer.IsAlivePageUrl' = $CatiLoadBalancerIsAlivePageUrl
    'Cati.LoadBalancer.IsAlivePageRenameTimeout' = $CatiLoadBalancerIsAlivePageRenameTimeout
    'Cati.LoadBalancer.IgnoreIfIsAlivePageDoesNotExist' = $CatiLoadBalancerIgnoreIfIsAlivePageDoesNotExist

    'Cati.SessionState.Mode' = $CatiSessionStateMode
    'Cati.SessionState.Database.ServerName' = $CatiSessionStateDatabaseServerName
    'Cati.SessionState.Database.UserName' = $CatiSessionStateDatabaseUserName
    'Cati.SessionState.Database.Password' = $CatiSessionStateDatabasePassword
    'Cati.SessionState.Redis.HostName' = $CatiSessionStateRedisHostName
    'Cati.SessionState.Redis.Password' = $CatiSessionStateRedisPassword
    'Cati.SessionState.CookieName' = $CatiSessionStateCookieName
    'Cati.Parameters.Supervisor.VirtualDirectoryName' = $CatiParametersSupervisorVirtualDirectoryName
    'Cati.Parameters.Supervisor.AppPoolName' = $CatiParametersSupervisorAppPoolName
    'Cati.Parameters.Supervisor.SiteName' = $CatiParametersSupervisorSiteName

    'Confirmit.Site.Identity.ClientKeyGeneratorSecret' = $ConfirmitSiteIdentityClientKeyGeneratorSecret
} 

PrintAllParameter $settings
VerifyAllParameterAssignment $settings

# Vefity that required parameters have a value and that boolean and integer parameters have a correct value
VerifyStringParameter 'Cati.Database.ServerName' $CatiDatabaseServerName
VerifyStringParameter 'Confirmit.Database.User.SystemAdmin.Name' $ConfirmitDatabaseUserSystemAdminName
VerifyStringParameter 'Confirmit.Database.User.SystemAdmin.Password' $ConfirmitDatabaseUserSystemAdminPassword
VerifyStringParameter 'Confirmit.Database.User.DeployC.Name' $ConfirmitDatabaseUserDeployCName
VerifyStringParameter 'Confirmit.Database.User.DeployC.Password' $ConfirmitDatabaseUserDeployCPassword
VerifyStringParameter 'Confirmit.Database.Server.System.ServerName' $ConfirmitDatabaseServerSystemServerName
VerifyStringParameter 'Cati.Parameters.ValidWCFIpAddresses' $CatiParametersValidWCFIpAddresses
VerifyStringParameter 'Cati.Parameters.NotificationEmailBcc' $CatiParametersNotificationEmailBcc

VerifyBoolParameter 'Cati.Database.UseSimpleRecoveryMode' $CatiDatabaseUseSimpleRecoveryMode
VerifyBoolParameter 'Cati.SSL.GenerateTestCertificate' $CatiSSLGenerateTestCertificate
VerifyBoolParameter 'Cati.LoadBalancer.UseLoadBalancer' $CatiLoadBalancerUseLoadBalancer
VerifyBoolParameter 'Cati.LoadBalancer.IgnoreIfIsAlivePageDoesNotExist' $CatiLoadBalancerIgnoreIfIsAlivePageDoesNotExist

VerifyIntParameter 'Cati.LoadBalancer.IsAlivePageRenameTimeout' $CatiLoadBalancerIsAlivePageRenameTimeout

VerifyStringParameter 'Cati.SessionState.Mode' $CatiSessionStateMode
VerifyStringParameter 'Cati.SessionState.CookieName' $CatiSessionStateCookieName
VerifyStringParameter 'Cati.Parameters.Supervisor.VirtualDirectoryName' $CatiParametersSupervisorVirtualDirectoryName
VerifyStringParameter 'Cati.Parameters.Supervisor.AppPoolName' $CatiParametersSupervisorAppPoolName
VerifyStringParameter 'Cati.Parameters.Supervisor.SiteName' $CatiParametersSupervisorSiteName

if('True' -eq $CatiDatabaseUseSimpleRecoveryMode)
{ 
    $catiDefaultDbRecoveryModel = "simple" 
}
else 
{ 
    $catiDefaultDbRecoveryModel = "full" 
}

if('True' -eq $CatiSSLGenerateTestCertificate)
{ 
    $certificateType = "Test" 
}
else 
{ 
    $certificateType = "Real" 
}

$catiConnectionString = "Data Source=$CatiDatabaseServerName;Initial Catalog=ConfirmitCATIV15;User ID=$ConfirmitDatabaseUserDeployCName;Password=$ConfirmitDatabaseUserDeployCPassword;Connect Timeout=120;Max Pool Size=4096"
$confirmConnectionString = "Data Source=$ConfirmitDatabaseServerSystemServerName;Initial Catalog=Confirm;User ID=$ConfirmitDatabaseUserDeployCName;Password=$ConfirmitDatabaseUserDeployCPassword;Connect Timeout=120"
$confirmlogConnectionString = "Data Source=$ConfirmitDatabaseServerSystemServerName;Initial Catalog=Confirmlog;User ID=$ConfirmitDatabaseUserDeployCName;Password=$ConfirmitDatabaseUserDeployCPassword;Connect Timeout=120"
$sessionStateConnectionString = "Data Source=$CatiSessionStateDatabaseServerName;User ID=$CatiSessionStateDatabaseUserName;Password=$CatiSessionStateDatabasePassword"

$isAliveHtmEngine = new-object Confirmit.CATI.Installation.Common.IsAliveHtmEngine($logger, [bool]$CatiLoadBalancerIgnoreIfIsAlivePageDoesNotExist)
$prereqChecker = new-object Confirmit.CATI.Installation.Common.PrereqChecker
$confirmitCatiValidator = new-object Confirmit.CATI.Installation.Common.ConfirmitCATIValidator
$certificateEngine = new-object Confirmit.CATI.Installation.Common.CertificateEngine(new-object Confirmit.CATI.Installation.Common.DialogService)
$installationVerifier = new-object CatiInstallation.InstallationVerifier($logger, $prereqChecker, $confirmitCatiValidator, $certificateEngine, $isAliveHtmEngine)

$installationFacade = new-object CatiInstallation.InstallationFacade($logger)

Write-Host "Call GetTypeOfActionWithDatabase"
$typeOfActionWithDatabase = $installationFacade.GetTypeOfActionWithDatabase($CatiDatabaseServerName, $ConfirmitDatabaseUserSystemAdminName, $ConfirmitDatabaseUserSystemAdminPassword) #UseExistingDB or CreateNewDB

$backendParameters = new-object CatiInstallation.BackendParameters
$backendParameters.CatiSqlServerName = $CatiDatabaseServerName 
$backendParameters.CatiSqlAdminUserName = $ConfirmitDatabaseUserSystemAdminName
$backendParameters.CatiSqlAdminPassword = $ConfirmitDatabaseUserSystemAdminPassword
$backendParameters.CatiSqlUserName = $ConfirmitDatabaseUserDeployCName
$backendParameters.CatiSqlPassword = $ConfirmitDatabaseUserDeployCPassword
$backendParameters.CatiConnectionString = $catiConnectionString
$backendParameters.TypeOfActionWithDatabase = $typeOfActionWithDatabase
$backendParameters.CatiDefaultDbRecoveryModel = $catiDefaultDbRecoveryModel
$backendParameters.ConfirmSqlServerName = $ConfirmitDatabaseServerSystemServerName
$backendParameters.ConfirmUserName = $ConfirmitDatabaseUserDeployCName
$backendParameters.ConfirmPassword = $ConfirmitDatabaseUserDeployCPassword
$backendParameters.ConfirmConnectionString = $confirmConnectionString
$backendParameters.ConfirmlogConnectionString = $confirmlogConnectionString
$backendParameters.IsLoadBalancedEnvironment = $CatiLoadBalancerUseLoadBalancer
$backendParameters.CertificateType = $certificateType
$backendParameters.TestCertificateName = $CatiSSLTestCertificateName
$backendParameters.CertificatePath = $CatiSSLCertificatePath
$backendParameters.CertificatePassword = $CatiSSLCertificatePassword
$backendParameters.CatiDatabasesDataFilePath = $CatiDatabaseServerDataPath
$backendParameters.CatiDatabasesLogsFilePath = $CatiDatabaseServerLogPath
$backendParameters.NotificationEmailBCC = $CatiParametersNotificationEmailBcc
$backendParameters.ConfirmitLinkedServerName = $CatiDatabaseLinkedServerNameToConfirmit
$backendParameters.MsiInstallLocation = $CatiMsiParametersInstallLocation
$backendParameters.LoadBalancerIsAlivePageUrl = $CatiLoadBalancerIsAlivePageUrl
$backendParameters.LoadBalancerIsAlivePageRenameTimeout = $CatiLoadBalancerIsAlivePageRenameTimeout

Write-Host "Call VerifyBackendParameters"
$installationVerifier.VerifyBackendParameters($backendParameters)

$supervisorParameters = new-object CatiInstallation.SupervisorParameters
$supervisorParameters.CatiSqlServerName = $CatiDatabaseServerName 
$supervisorParameters.CatiSqlAdminUserName = $ConfirmitDatabaseUserSystemAdminName
$supervisorParameters.CatiSqlAdminPassword = $ConfirmitDatabaseUserSystemAdminPassword
$supervisorParameters.SessionStateServerName = $CatiSessionStateDatabaseServerName
$supervisorParameters.SessionStateUserName = $CatiSessionStateDatabaseUserName
$supervisorParameters.SessionStatePassword = $CatiSessionStateDatabasePassword
$supervisorParameters.SessionStateConnectionString = $sessionStateConnectionString
$supervisorParameters.SessionStateMode = $CatiSessionStateMode
$supervisorParameters.RedisHostName = $CatiSessionStateRedisHostName
$supervisorParameters.SessionStateCookieName = $CatiSessionStateCookieName
$supervisorParameters.SupervisorVirtualDirectoryName = $CatiParametersSupervisorVirtualDirectoryName
$supervisorParameters.SupervisorAppPoolName = $CatiParametersSupervisorAppPoolName
$supervisorParameters.SupervisorSiteName = $CatiParametersSupervisorSiteName

Write-Host "Call VerifySupervisorParameters"
$installationVerifier.VerifySupervisorParameters($supervisorParameters)

FinishVerification