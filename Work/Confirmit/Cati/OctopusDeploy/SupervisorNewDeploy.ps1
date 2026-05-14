try
{
	$CatiParametersSupervisorVirtualDirectoryName = $CatiParametersSupervisorVirtualDirectoryName + ".New"
	
    . "Installation\InitialInitializations.ps1" 
    . "Installation\DeployFunctions.ps1"
    . "Installation\ConfigurationFileUtils.ps1"

    # Load external C# dll's
    [Reflection.Assembly]::LoadFile("$installLocation\Installation\CatiInstallation.dll")
    [Reflection.Assembly]::LoadFile("$installLocation\bin\Telerik.Reporting.dll")

    $OctopusActionPackageNuGetPackageVersion = $OctopusActionPackageNuGetPackageVersion + ".0"
    Write-Host "OctopusActionPackageNuGetPackageVersion=$OctopusActionPackageNuGetPackageVersion"

    # Define dependent parameters
    $catiConnectionString = "Data Source=$CatiDatabaseServerName;Initial Catalog=ConfirmitCATIV15;User ID=$ConfirmitDatabaseUserSystemAdminName;Password=$ConfirmitDatabaseUserSystemAdminPassword;Connect Timeout=120;Max Pool Size=4096"
    $confirmConnectionString = "Data Source=$ConfirmitDatabaseServerSystemServerName;Initial Catalog=Confirm;User ID=$ConfirmitDatabaseUserDeployCName;Password=$ConfirmitDatabaseUserDeployCPassword;Connect Timeout=120"
    $sessionStateConnectionString = "Data Source=$CatiSessionStateDatabaseServerName;User ID=$CatiSessionStateDatabaseUserName;Password=$CatiSessionStateDatabasePassword"
    $newIgResFolderName = "ig_res_$OctopusActionPackageNuGetPackageVersion"

    $encryptedSessionStateConnectionString = RemoveAndLineFeed(& $installLocation\Installation\CryptographicUtility.exe "$sessionStateConnectionString" | Out-String)

    Write-Host "Start custom actions"
    $prereqChecker = new-object Confirmit.CATI.Installation.Common.PrereqChecker
    $confirmitCatiValidator = new-object Confirmit.CATI.Installation.Common.ConfirmitCATIValidator
    $certificateEngine = new-object Confirmit.CATI.Installation.Common.CertificateEngine(new-object Confirmit.CATI.Installation.Common.DialogService)
    $installationVerifier = new-object CatiInstallation.InstallationVerifier($logger, $prereqChecker, $confirmitCatiValidator, $certificateEngine)
    $databaseEngine = new-object CustomActionLibrary.DatabaseEngine($logger, $confirmConnectionString)
    $confirmitCatiEngine =  new-object Confirmit.CATI.Installation.Common.ConfirmitCatiEngine($logger);

    $installationFacade = new-object CatiInstallation.InstallationFacade($logger)

    # Get some parameters from confirm database
    $confirmitAuthoringSchemeAndHost = $installationFacade.GetSchemeAndHostFromConfirmDatabase($confirmitCatiEngine, $databaseEngine, "ConfirmitURL");

    $confirmitKeepSessionAspxUrl = "$confirmitAuthoringSchemeAndHost/confirm/authoring/KeepSession.aspx"

    Write-Host "Dependent parameters list"
    Write-Host "Cati Connection String=$catiConnectionString"
    Write-Host "Confirm Connection String=$confirmConnectionString"
    Write-Host "Session State Connection String=$sessionStateConnectionString"
    Write-Host "Encrypted Session State Connection String=$encryptedSessionStateConnectionString"
    Write-Host "New IgRes Folder Name=$newIgResFolderName"
    Write-Host "Confirmit Authoring Scheme And Host=$confirmitAuthoringSchemeAndHost"
    Write-Host "Confirmit Keep Session Aspx Url=$confirmitKeepSessionAspxUrl"

    # Start installation actions
    if(Test-Path -Path "$installLocation\ig_res" )
    {
        Write-Host "Rename ig_res folder"
        Rename-Item "$installLocation\ig_res" "$installLocation\$newIgResFolderName"
    }

    Write-Host "Call SetPermissionsForPdbFiles"
    $installationFacade.SetPermissionsForPdbFiles("$installLocation\bin")

    Write-Host "Call ConfigureSupervisorConfig"
    $installationFacade.ConfigureSupervisorConfig($installLocation, $catiConnectionString, $CatiSessionStateMode,
                $CatiSessionStateRedisHostName, $CatiSessionStateRedisPassword, $sessionStateConnectionString, $CatiSessionStateCookieName, 
                $CatiParametersSupervisorVirtualDirectoryName, $confirmitKeepSessionAspxUrl, $newIgResFolderName, $ConfirmitSiteLogPath)
	
    Write-Host 'Setting up Trusted Subsystem (change Supervisor config)'
    $pathToSupervisorConfig = Join-Path $installLocation Web.config
    Write-Host "Path to Supervisor config=$pathToSupervisorConfig"
    InsertClientSecretInConfigFile $pathToSupervisorConfig 'Confirmit.CATI.Service' $ConfirmitSiteIdentityClientKeyGeneratorSecret

    SetUpClientSecretForCustomGrantClient 'Confirmit.CatiSupervisor.Service' $pathToSupervisorConfig

    Write-Host "Call ConfigureIISApplication"
    $installationFacade.ConfigureIISApplication("", $CatiParametersSupervisorAppPoolName, $CatiParametersSupervisorSiteName, $CatiParametersSupervisorVirtualDirectoryName, $installLocation)

    Write-Host "Call SetContentExpiration"
    $installationFacade.SetContentExpiration($OctopusActionPackageNuGetPackageVersion, $CatiParametersSupervisorSiteName, $CatiParametersSupervisorVirtualDirectoryName)

    Write-Host "Custom actions have done"
}
catch [Exception]
{ 
    "Failed to run package script:"
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}