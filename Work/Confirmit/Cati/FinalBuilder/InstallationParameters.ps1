# If you need to change some installation parameters you should create InstallationParametersEx.ps1 file and override needed variables only
# Be careful: build woun't run validation for specified parameters 

$CatiInstallLocation = 'c:\Program Files\Confirmit CATI Rel'
$SimulatorInstallLocation = 'c:\Program Files\Confirmit CATI LTU Simulator (G) Dialer Web Service Rel'

$ServerName=(Get-WmiObject win32_computersystem).DNSHostName.ToLower()
$currentIPsV6andV4=(Get-WmiObject -Class Win32_NetworkAdapterConfiguration | where {$_.DefaultIPGateway -ne $null}).IPAddress -join ';'
$ServerNameInConfirmitSetting=$ServerName
$WebCATIConsoleDomain=$ServerName

$ConfirmitDatabaseUserSystemAdminName = 'sa'
$ConfirmitDatabaseUserSystemAdminPassword = 'firm'
$ConfirmitDatabaseServerSystemServerName = $ServerName
$CatiDatabaseLinkedServerNameToConfirmit = ''
$CatiDatabaseServerDataPath = ''
$CatiDatabaseServerLogPath = ''
$CatiDatabaseServerName = $ServerName + '\SYSTEM'
$ConfirmitDatabaseUserDeployCName = 'ConfirmitDeploy'
$ConfirmitDatabaseUserDeployCPassword = 'DeployConfirmit01'
$CatiDatabaseUseSimpleRecoveryMode = 'True'
$CatiLoadBalancerIgnoreIfIsAlivePageDoesNotExist = 'True'
$CatiLoadBalancerIsAlivePageRenameTimeout = '60'
$CatiLoadBalancerIsAlivePageUrl = ''
$CatiLoadBalancerUseLoadBalancer = 'False'
$CatiParametersNotificationEmailBcc = 'qwer@qwer.ru'
$CatiParametersSupervisorAppPoolName = 'CatiAppPool'
$CatiParametersSupervisorSiteName = 'Default Web Site'
$CatiParametersSupervisorVirtualDirectoryName = 'Supervisor.Rel'
$CatiParametersSupervisorNewVirtualDirectoryName = $CatiParametersSupervisorVirtualDirectoryName + '.New'
$CatiParametersValidWCFIpAddresses = "127.0.0.1;::1;$currentIPsV6andV4"
$CatiSessionStateCookieName = 'ConfirmitCati_CookieName'
$CatiSessionStateDatabaseServerName = $ServerName
$CatiSessionStateDatabasePassword = 'firm'
$CatiSessionStateDatabaseUserName = 'sa'
$CatiSessionStateMode = 'InProc'
$CatiSessionStateRedisHostName = ''
$CatiSSLCertificatePassword = ''
$CatiSSLCertificatePath = ''
$CatiSSLGenerateTestCertificate = 'True'
$CatiSSLTestCertificateName = $ServerName
$CatiSSLOverrideCertificateIfExist = 'False'

$publishMetadataForExternalWCFServices = 'True'
$publishMetadataForInternalWCFServices = 'True'

$ConfirmitSiteIdentityClientKeyGeneratorSecret = '30020ECE-E3DE-4EA9-A57C-9265DEE06C83'
$ConfirmitSiteLogPath = 'c:\confirmit_logs'

$CatiParametersSimulatorSiteName = 'Default Web Site'
$CatiParametersSimulatorAppPoolName = 'LTUSimulator(G)DialerAppPool'
$CatiParametersSimulatorVirtualDirectoryName = 'LTUSimulator(G)DialerService.Rel'
$CatiParametersSimulatorDialerId = '0'
$CatiParametersSimulatorWebApiUrl = 'http://*:3838/catidialersimulator'
$CatiParametersSimulatorLoggingPath = 'c:\DialerLogs\'
$CatiParametersSimulatorEndpointServerName = $ServerName

$CatiSupervisorClientPath = '../../../confirmit.catisupervisor.client'
$CatiSupervisorApiPath = '../../../confirmit.catisupervisor.api'
$CatiSupervisorClientApplicationName = 'catisupervisor'
$CatiSupervisorApiApplicationName = 'api/catisupervisor'
$CatiSupervisorApiApplicationPoolName = 'catisupervisor.api'

$CatiInterviewerClientPath = '../../../confirmit.catiinterviewer.client'
$CatiInterviewerApiPath = '../../../confirmit.catiinterviewer.api'
$CatiInterviewerClientApplicationName = 'catiinterviewer'
$CatiInterviewerApiApplicationName = 'api/catiinterviewer'
$CatiInterviewerApiApplicationPoolName = 'catiinterviewer.api'