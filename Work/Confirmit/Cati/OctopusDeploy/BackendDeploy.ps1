try
{    
    . "Installation\InitialInitializations.ps1"
    . "Installation\DeployFunctions.ps1"

    # Load external C# dll's
    [Reflection.Assembly]::LoadFile("$installLocation\Installation\CatiInstallation.dll")
    [Reflection.Assembly]::LoadFile("$installLocation\Confirmit.Configuration.Bootstrap.dll")
    [Reflection.Assembly]::LoadFile("$installLocation\Confirmit.Configuration.dll")
    [Reflection.Assembly]::LoadFile("$installLocation\Confirmit.Databases.dll")
    [Reflection.Assembly]::LoadFile("$installLocation\Confirmit.DataServices.RDataAccess.dll")
    [Reflection.Assembly]::LoadFile("$installLocation\Confirmit.Security.Crypto.dll")
    [Reflection.Assembly]::LoadFile("$installLocation\Installation\Confirmit.CATI.DatabaseUpdateLibrary.dll")

    [Reflection.Assembly]::LoadFile("$installLocation\Confirmit.CATI.Common.dll")
    [Reflection.Assembly]::LoadFile("$installLocation\Confirmit.CATI.Core.dll")
    [Reflection.Assembly]::LoadFile("$installLocation\Confirmit.Security.Crypto.Web.dll")
    [Reflection.Assembly]::LoadFile("$installLocation\Confirmit.CATI.WindowsServiceTools.dll")

    $OctopusActionPackageNuGetPackageVersion = $OctopusActionPackageNuGetPackageVersion + ".0"

    # Convert boolean values to 'True' or 'False'
    $CatiDatabaseUseSimpleRecoveryMode = CapitalizeTheFirstCharacter $CatiDatabaseUseSimpleRecoveryMode
    $CatiSSLGenerateTestCertificate = CapitalizeTheFirstCharacter $CatiSSLGenerateTestCertificate
    $CatiLoadBalancerUseLoadBalancer = CapitalizeTheFirstCharacter $CatiLoadBalancerUseLoadBalancer
    $CatiLoadBalancerIgnoreIfIsAlivePageDoesNotExist = CapitalizeTheFirstCharacter $CatiLoadBalancerIgnoreIfIsAlivePageDoesNotExist

    # Define dependent parameters
    $catiConnectionString = "Data Source=$CatiDatabaseServerName;Initial Catalog=ConfirmitCATIV15;User ID=$ConfirmitDatabaseUserDeployCName;Password=$ConfirmitDatabaseUserDeployCPassword;Connect Timeout=120;Max Pool Size=4096"
    $masterConnectionString = "Data Source=$CatiDatabaseServerName;Initial Catalog=master;User ID=$ConfirmitDatabaseUserSystemAdminName;Password=$ConfirmitDatabaseUserSystemAdminPassword"
    $confirmConnectionString = "Data Source=$ConfirmitDatabaseServerSystemServerName;Initial Catalog=Confirm;User ID=$ConfirmitDatabaseUserDeployCName;Password=$ConfirmitDatabaseUserDeployCPassword;Connect Timeout=120"
    $confirmlogConnectionString = "Data Source=$ConfirmitDatabaseServerSystemServerName;Initial Catalog=Confirmlog;User ID=$ConfirmitDatabaseUserDeployCName;Password=$ConfirmitDatabaseUserDeployCPassword;Connect Timeout=120"

    $encryptedCatiConnectionString = RemoveAndLineFeed(& $installLocation\Installation\CryptographicUtility.exe "$catiConnectionString" | Out-String)
    $encryptedConfirmConnectionString = RemoveAndLineFeed(& $installLocation\Installation\CryptographicUtility.exe "$confirmConnectionString" | Out-String)
    $encryptedConfirmlogConnectionString = RemoveAndLineFeed(& $installLocation\Installation\CryptographicUtility.exe "$confirmlogConnectionString" | Out-String)
    if($CatiSSLCertificatePassword)
    {
        $encryptedCatiSSLCertificatePassword = RemoveAndLineFeed(& $installLocation\Installation\CryptographicUtility.exe "$CatiSSLCertificatePassword" | Out-String)
    }

    if(!$publishMetadataForExternalWCFServices)
    {
        $publishMetadataForExternalWCFServices = "False"
    }

    if(!$publishMetadataForInternalWCFServices)
    {
        $publishMetadataForInternalWCFServices = "False"
    }

    if((!$CatiSSLOverrideCertificateIfExist) -or ($CatiSSLOverrideCertificateIfExist.ToLower() -ne "false"))
    {
        $CatiSSLOverrideCertificateIfExist = "True"
    }

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

    Write-Host "Start custom actions"
    $isAliveHtmEngine = new-object Confirmit.CATI.Installation.Common.IsAliveHtmEngine($logger, [bool]$CatiLoadBalancerIgnoreIfIsAlivePageDoesNotExist)
    $confirmitDatabaseEngine = new-object CustomActionLibrary.DatabaseEngine($logger, $confirmConnectionString)
    $confirmitCatiEngine =  new-object Confirmit.CATI.Installation.Common.ConfirmitCatiEngine($logger);

    $installationFacade = new-object CatiInstallation.InstallationFacade($logger)

    Write-Host "Get some parameters from confirm database"
    $confirmitAuthoringSchemeAndHost = $installationFacade.GetSchemeAndHostFromConfirmDatabase($confirmitCatiEngine, $confirmitDatabaseEngine, "ConfirmitURL");
    $confirmitDeploymentSchemeAndHost = $installationFacade.GetSchemeAndHostFromConfirmDatabase($confirmitCatiEngine, $confirmitDatabaseEngine, "DeployURL");
    $confirmitWebServiceSchemeAndHost = $installationFacade.GetSchemeAndHostFromConfirmDatabase($confirmitCatiEngine, $confirmitDatabaseEngine, "WebServiceBaseUrl");
    
    $startSurveyUrl = "$confirmitDeploymentSchemeAndHost/wix/cati_"
    $authoringWebServiceUrl = "$confirmitWebServiceSchemeAndHost/Confirmit/InternalWebServices/14.0/FusionAuthoring.asmx"
    $surveyDataWebServiceUrl = "$confirmitWebServiceSchemeAndHost/confirmit/InternalWebServices/14.0/FusionSurveyData.asmx"
    $catiReviewerSessionUrlTemplate = "$confirmitAuthoringSchemeAndHost/reviewer/{0}"

    #Tempopary line. Need to remove in future
    $catiReviewerSessionUrlTemplate = $catiReviewerSessionUrlTemplate.Replace("http://", "https://");

    Write-Host "Call GetTypeOfActionWithDatabase"
    $typeOfActionWithDatabase = $installationFacade.GetTypeOfActionWithDatabase($CatiDatabaseServerName, $ConfirmitDatabaseUserSystemAdminName, $ConfirmitDatabaseUserSystemAdminPassword) #UseExistingDB or CreateNewDB
  
    Write-Host "Dependent parameters list"
    Write-Host "Cati.Msi.Parameters.InstallLocation=$CatiMsiParametersInstallLocation"
    Write-Host "Cati Connection String=$catiConnectionString"
    Write-Host "Master Connection String=$masterConnectionString"
    Write-Host "Confirm Connection String=$confirmConnectionString"
    Write-Host "Confirmlog Connection String=$confirmlogConnectionString"
    Write-Host "Encrypted Cati Connection String=$encryptedCatiConnectionString"    
    Write-Host "Encrypted Confirm Connection String=$encryptedConfirmConnectionString"
    Write-Host "Encrypted Confirmlog Connection String=$encryptedConfirmlogConnectionString"
    Write-Host "Encrypted Cati SSL Certificate Password=$encryptedCatiSSLCertificatePassword"
    Write-Host "Cati Default Db Recovery Model=$catiDefaultDbRecoveryModel"
    Write-Host "Certificate Type=$certificateType"

    Write-Host "Confirmit Deployment Scheme And Host=$confirmitDeploymentSchemeAndHost"
    Write-Host "Confirmit Web Service Scheme And Host=$confirmitWebServiceSchemeAndHost"
    Write-Host "Start Survey Url=$startSurveyUrl"
    Write-Host "Authoring Web Service Url=$authoringWebServiceUrl"
    Write-Host "Survey Data Web Service Url=$surveyDataWebServiceUrl"
    Write-Host "Type Of Action With Database=$typeOfActionWithDatabase"
    Write-Host "Reviewer Session Url Template=$catiReviewerSessionUrlTemplate"
    Write-Host "Override SSL Ceritifacte For HTTP Listener If It Exists=$CatiSSLOverrideCertificateIfExist"

    WriteToConfirmitLog $confirmitDatabaseEngine "Start installation actions"
    if($typeOfActionWithDatabase -eq "CreateNewDB")
    {
        WriteToConfirmitLog $confirmitDatabaseEngine "Create default CATI database"
        $installationFacade.CreateDefaultDatabase($CatiDatabaseServerName, $ConfirmitDatabaseUserSystemAdminName, $ConfirmitDatabaseUserSystemAdminPassword, $ConfirmitDatabaseUserDeployCName, $ConfirmitDatabaseUserDeployCPassword, 
                $confirmlogConnectionString, $CatiDatabaseLinkedServerNameToConfirmit, $CatiDatabaseServerDataPath, $CatiDatabaseServerLogPath, $catiDefaultDbRecoveryModel, "") 
    }
    else
    {
        if($CatiLoadBalancerUseLoadBalancer -eq 'True')
        {
            WriteToConfirmitLog $confirmitDatabaseEngine "Rename IsAlive.htm file"
            $installationFacade.BackupIsAliveHtmFile($isAliveHtmEngine, $CatiLoadBalancerIsAlivePageUrl, $CatiLoadBalancerIsAlivePageRenameTimeout)
        }

        WriteToConfirmitLog $confirmitDatabaseEngine "Stop local CATI services"
        $installationFacade.StopAllCatiServices($CatiDatabaseServerName, $sideBySideName, $OctopusActionPackageNuGetPackageVersion)

        try 
        {
            WriteToConfirmitLog $confirmitDatabaseEngine "Start database update"
            $installationFacade.UpdateDatabases()
        }
        catch [CatiInstallation.DatabaseUpdatePossibilityException] 
        {
            WriteToConfirmitLog $confirmitDatabaseEngine "Database update has failed. CATI services were started. Wait until they become operational"
            $installationFacade.StartAllCatiServicesAndWaitUntilTheyStarted($sideBySideName, $CatiLoadBalancerUseLoadBalancer)

            if($CatiLoadBalancerUseLoadBalancer -eq 'True')
            {
                WriteToConfirmitLog $confirmitDatabaseEngine "Restore IsAlive.htm file"
                $isAliveHtmEngine.RestoreIsAliveHtmFile($CatiLoadBalancerIsAlivePageUrl)
            }

            WriteToConfirmitLog $confirmitDatabaseEngine "A server was returned to the loop. Installation was stopped because database cannot be updated"
            throw
        }

        WriteToConfirmitLog $confirmitDatabaseEngine "Remove local CATI services"
        $installationFacade.RemoveCatiServices($sideBySideName)
    }

    Write-Host "Configure compatibility level SQL Agent Job"
    Add-CompatibilityLevelSQLAgentJob $masterConnectionString

    WriteToConfirmitLog $confirmitDatabaseEngine "Configure Backend"

    Write-Host "Call SetPermissionsForPdbFiles"
    $installationFacade.SetPermissionsForPdbFiles($installLocation)

    Write-Host "Call InstallTestCertificatesAndConfiguringHttpListenerProgressStatusIfNeeded"
    $installationFacade.InstallTestCertificatesAndConfiguringHttpListenerProgressStatusIfNeeded($CatiLoadBalancerUseLoadBalancer, $installLocation, 
                   $certificateType, $CatiSSLTestCertificateName, $CatiSSLCertificatePath, $CatiSSLCertificatePassword, "", $CatiSSLOverrideCertificateIfExist)

    Write-Host "Call ConfigureBackendConfig"
    $installationFacade.ConfigureBackendConfig($installLocation, $CatiLoadBalancerUseLoadBalancer, $ConfirmitSiteLogPath)

    $settings = [System.Collections.Generic.Dictionary`2[System.String,System.String]]@{}
    $settings.Add("WebServiceUrl.Authoring", $authoringWebServiceUrl)
    $settings.Add("WebServiceUrl.SurveyData", $surveyDataWebServiceUrl)
    $settings.Add("Debug.PublishMetadataForExternalWCFServices", $publishMetadataForExternalWCFServices)
    $settings.Add("Debug.PublishMetadataForInternalWCFServices", $publishMetadataForInternalWCFServices)
    $settings.Add("SQLServer.SqlServerDataPath", $CatiDatabaseServerDataPath)
    $settings.Add("SQLServer.SqlServerLogPath", $CatiDatabaseServerLogPath)
    $settings.Add("Site.StartSurveyURL", $startSurveyUrl)
    $settings.Add("Email.NotificationEmailBCC", $CatiParametersNotificationEmailBcc)
    $settings.Add("Setup.EncryptedConfirmConnectionString", $encryptedConfirmConnectionString)
    $settings.Add("Setup.EncryptedConfirmlogConnectionString", $encryptedConfirmlogConnectionString)
    $settings.Add("Reviewer.SessionUrlTemplate", $catiReviewerSessionUrlTemplate)

    # Parameters for support the msi setup
    $settings.Add("Setup.IsLoadBalancedEnvironment", $CatiLoadBalancerUseLoadBalancer)
    $settings.Add("Setup.CertificateType", $certificateType)
    $settings.Add("Setup.TestCertificateName", $CatiSSLTestCertificateName)
    $settings.Add("Setup.CertificatePath", $CatiSSLCertificatePath)
    $settings.Add("Setup.EncryptedCertificatePassword", $encryptedCatiSSLCertificatePassword)
    $settings.Add("Setup.ConfirmitLinkedServerName", $CatiDatabaseLinkedServerNameToConfirmit)
    $settings.Add("Setup.LoadBalancerIsAlivePageUrl", $CatiLoadBalancerIsAlivePageUrl)
    $settings.Add("Setup.LoadBalancerIsAlivePageRenameTimeout", $CatiLoadBalancerIsAlivePageRenameTimeout)
    if($CatiMsiParametersInstallLocation)
    {
        $settings.Add("Setup.InstallLocation", $CatiMsiParametersInstallLocation)
    }
    if($typeOfActionWithDatabase -eq "CreateNewDB")
    {
        $settings.Add("Server.AccessAllowedIPAddresses", $CatiParametersValidWCFIpAddresses)
    }

    Write-Host "Call ConfigureBackendSettings"
	Write-Host "Call $CatiDatabaseServerName"
    $installationFacade.ConfigureBackendAndSupervisorSettings($CatiDatabaseServerName, $ConfirmitDatabaseUserDeployCName, $ConfirmitDatabaseUserDeployCPassword, $settings, $true)

    Write-Host 'Setting up Trusted Subsystem (change Backend config)'
    $pathToBackendConfig = Join-Path $installLocation Confirmit.CATI.Backend.exe.config
    Write-Host "Path to Backend config=$pathToBackendConfig"
    InsertClientSecretInConfigFile $pathToBackendConfig 'Confirmit.CATI.Service' $ConfirmitSiteIdentityClientKeyGeneratorSecret

    Set-SiteConfigValue $confirmConnectionString 623 $CatiDatabaseServerName

    WriteToConfirmitLog $confirmitDatabaseEngine "Default CATI service was created and started. Wait until CATI services become operational"
    $installationFacade.CreateAndRunDefaultCatiService($installLocation, $sideBySideName, $CatiLoadBalancerUseLoadBalancer)

    if($TypeOfActionWithDatabase -eq "UseExistingDB" -and $CatiLoadBalancerUseLoadBalancer -eq 'True')
    {
        WriteToConfirmitLog $confirmitDatabaseEngine "Restore IsAlive.htm file"
        $isAliveHtmEngine.RestoreIsAliveHtmFile($CatiLoadBalancerIsAlivePageUrl)
    }

    WriteToConfirmitLog $confirmitDatabaseEngine "Finish installation actions"
}
catch [Exception]
{ 
    "Failed to run package script:"
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}