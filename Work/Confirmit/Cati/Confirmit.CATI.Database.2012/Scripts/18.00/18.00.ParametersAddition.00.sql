DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
	UPDATE BvSystemSettings 
	SET  [Description] = 'Confirmit Authoring Web Service Url.'
	WHERE [SystemName] = 'WebServiceUrl.Authoring';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Confirmit SurveyData Web Service Url.'
	WHERE [SystemName] = 'WebServiceUrl.SurveyData';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Should metadata for external services be published or not. Possible values: True or False'
	WHERE [SystemName] = 'Debug.PublishMetadataForExternalWCFServices';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Should metadata for internal services be published or not. Possible values: True or False'
	WHERE [SystemName] = 'Debug.PublishMetadataForInternalWCFServices';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Supervisor web application name'
	WHERE [SystemName] = 'Setup.SupervisorVirtualDirectoryName';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Supervisor application pool name'
	WHERE [SystemName] = 'Setup.SupervisorAppPoolName';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'Supervisor web site name'
	WHERE [SystemName] = 'Setup.SupervisorSiteName';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Is database logging enabled or not. Possible values: 1 or empty'
	WHERE [SystemName] = 'Setup.IsDatabaseLoggingEnabled';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'Is eventlog logging enabled or not. Possible values: 1 or empty'
	WHERE [SystemName] = 'Setup.IsEventlogLoggingEnabled';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Min free space on disk in MB during a db update process for the database update utility (possible values: positive number). It shouldn''t be too small. Default: 1024'
	WHERE [SystemName] = 'Setup.MinFreeSpaceOnDiskInMb';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'Databases snapshot file path for the database update utility. Possible values: existed path on SQL server or empty'
	WHERE [SystemName] = 'Setup.DatabasesSnapshotFilePath';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Confirmit authoring server name'
	WHERE [SystemName] = 'Setup.ConfirmitAuthoringServer';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Confirmit deployment server name'
	WHERE [SystemName] = 'Setup.ConfirmitDeploymentServer';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Confirmit web service server name'
	WHERE [SystemName] = 'Setup.ConfirmitWebServiceServer';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'Session state mode. Possible values: SQLMode or InProc'
	WHERE [SystemName] = 'Setup.SessionStateMode';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Encrypted connection string to the session state database (use a special tool to change this setting)'
	WHERE [SystemName] = 'Setup.EncryptedSessionStateConnectionString';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Session state cookie name'
	WHERE [SystemName] = 'Setup.SessionStateCookieName';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'Encrypted connection string to confirm database (use a special tool to change this setting)'
	WHERE [SystemName] = 'Setup.EncryptedConfirmConnectionString';

	UPDATE BvSystemSettings 
	SET  [Description] = 'Encrypted connection string to confirmlog database (use a special tool to change this setting)'
	WHERE [SystemName] = 'Setup.EncryptedConfirmlogConnectionString';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'Is ssl accelerator use. Possible values: True or False'
	WHERE [SystemName] = 'Setup.IsSslAcceleratorUse';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'Certificate type. Possible values: Test or Real'
	WHERE [SystemName] = 'Setup.CertificateType';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'Certificate name'
	WHERE [SystemName] = 'Setup.CertificateName';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'Real certificate thumbprint. Make sense if ''CertificateType'' parameter is Real'
	WHERE [SystemName] = 'Setup.RealCertificateThumbprint';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'Confirmit linked server name. This value can be used in update scripts during DB update process'
	WHERE [SystemName] = 'Setup.ConfirmitLinkedServerName';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'A location of IsAlive.html file. Reqired if ''IsSslAcceleratorUse'' parameter is True'
	WHERE [SystemName] = 'Setup.IsAliveHtmlLocation';
	
	UPDATE BvSystemSettings 
	SET  [Description] = 'A root folder of CATI installation'
	WHERE [SystemName] = 'Setup.InstallLocation';

END

GO
PRINT N'Update complete.';
