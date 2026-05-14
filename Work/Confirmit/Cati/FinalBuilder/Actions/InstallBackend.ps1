function Get-NugetBuildVersion()
{
    $fileDefaultVersionPath = ".\..\..\DefaultVersion.targets"

    $versionData = Get-Content -Path $fileDefaultVersionPath | Where-Object {$_ -like '*<BuildNumber>*</BuildNumber>*'}
    $startIndex = $versionData.IndexOf('<BuildNumber>') + '<BuildNumber>'.Length
    $lastIndex = $versionData.IndexOf('.0</BuildNumber>')

    return $versionData.Substring($startIndex, $lastIndex - $startIndex)
}

function Set-DbOwnerForCatiDatabases()
{
	$connection = New-Object System.Data.SqlClient.SqlConnection
	$connection.ConnectionString = "Server=$CatiDatabaseServerName;uid=$ConfirmitDatabaseUserSystemAdminName;pwd=$ConfirmitDatabaseUserSystemAdminPassword"
	$connection.Open()

	$command = $connection.CreateCommand()
	$command.CommandText = "select [name] from sys.databases where [name] like 'ConfirmitCATIV15%' and state = 0"
	$reader = $command.ExecuteReader()

	$query = "exec sp_changedbowner '$ConfirmitDatabaseUserDeployCName'"
	while ($reader.Read()) {
		RunQuery $query $CatiDatabaseServerName $reader["name"] $ConfirmitDatabaseUserSystemAdminName $ConfirmitDatabaseUserSystemAdminPassword		
	}

	$reader.Close()
	$connection.Close()
}

function Set-SomeSettingsToDefaultCatiDatabase()
{
	$DefaultCatiDatabaseName = "ConfirmitCATIV15"
	
	Write-Host "Set Server.AccessAllowedIPAddresses to alow all IPs"
	$query = "UPDATE BvSystemSettings SET Value = '::0-ffff::ffff:ffff:ffff:ffff;0.0.0.0-255.255.255.255' WHERE SystemName = 'Server.AccessAllowedIPAddresses'"
	RunQuery $query $CatiDatabaseServerName $DefaultCatiDatabaseName $ConfirmitDatabaseUserSystemAdminName $ConfirmitDatabaseUserSystemAdminPassword	
	
	Write-Host "Set Toggle.EnableBBCCLogin to enable BBCC usage"	
	$query = "UPDATE BvSystemSettings SET Value = 'True' WHERE SystemName like 'Toggle.%' AND SystemName != 'Toggle.EnforceCatiHostNameForSurveys'"
	RunQuery $query $CatiDatabaseServerName $DefaultCatiDatabaseName $ConfirmitDatabaseUserSystemAdminName $ConfirmitDatabaseUserSystemAdminPassword	
}

try
{
    . ".\LoadParameters.ps1"

    $currentLocation = (Get-Item -Path ".\" -Verbose).FullName
    $supervisorNewCodeLocation = (Get-Item -Path ".\..\..\SupervisorNew\Confirmit.CATI.Supervisor" -Verbose).FullName
    
    Write-Host "Stop CATI services"
    Get-Service Confirmit.CATI.Backend.Rel* | Stop-Service

    Write-Host "Remove CATI services"
    Get-CimInstance -ClassName Win32_Service -Filter "Name LIKE 'Confirmit.CATI.Backend%'" | 
    ForEach-Object { 
        Invoke-CimMethod -InputObject $_ -MethodName Delete 
        Write-Host "Service $($_.Name) has been removed."
    }

    Write-Host "Wait until CATI services are removed"
    $maxRepeat = 60
    do 
    {
        sleep -Milliseconds 500
        $count = (Get-Service Confirmit.CATI.Backend.Rel*).count
        $maxRepeat--
        Write-Host "$count services left"
    } until ($count -eq 0 -or $maxRepeat -eq 0)

    Write-Host "Remove $CatiInstallLocation directory"
    if((Test-Path $CatiInstallLocation))
    {
        Remove-Item $CatiInstallLocation -Force -Recurse
    }
    
    Write-Host "Write needed variables to Confirmit database"
    $confirmDatabaseName = "Confirm"
    Write-Host "Change ConfirmitURL, DeployURL, WebServiceBaseUrl and RestApiURL parameters"
    $sqlQquery = "
        UPDATE CfgConfig SET ConfigValue = 'http://$ServerNameInConfirmitSetting/confirm'  WHERE ConfigName = 'ConfirmitURL'
        UPDATE CfgConfig SET ConfigValue = 'http://$ServerNameInConfirmitSetting'  WHERE ConfigName = 'DeployURL'
        UPDATE CfgConfig SET ConfigValue = 'http://$ServerNameInConfirmitSetting'  WHERE ConfigName = 'WebServiceBaseUrl'
        UPDATE CfgConfig SET ConfigValue = 'http://$ServerNameInConfirmitSetting/api'  WHERE ConfigName = 'RestApiURL'"	
	RunQuery $sqlQquery $ConfirmitDatabaseServerSystemServerName $confirmDatabaseName $ConfirmitDatabaseUserDeployCName $ConfirmitDatabaseUserDeployCPassword

    Write-Host "Change WebCATIConsoleDomain parameter"
    $sqlQquery = "UPDATE CfgConfig SET ConfigValue = '$WebCATIConsoleDomain'  WHERE ConfigName = 'WebCATIConsoleDomain'"
	RunQuery $sqlQquery $ConfirmitDatabaseServerSystemServerName $confirmDatabaseName $ConfirmitDatabaseUserDeployCName $ConfirmitDatabaseUserDeployCPassword

    Write-Host "Change MultimodeWebServiceURL and MultimodeBaseURL parameters"
    $sqlQquery = "
        UPDATE CfgConfig SET ConfigValue = 'http://$ServerNameInConfirmitSetting/Rel/ManagementMultimodeInstance'  WHERE ConfigName = 'MultimodeWebServiceURL'
        UPDATE CfgConfig SET ConfigValue = 'http://$ServerNameInConfirmitSetting/$CatiParametersSupervisorVirtualDirectoryName'  WHERE ConfigName = 'MultimodeBaseURL'"
	RunQuery $sqlQquery $ConfirmitDatabaseServerSystemServerName $confirmDatabaseName $ConfirmitDatabaseUserDeployCName $ConfirmitDatabaseUserDeployCPassword

	Write-Host "Set $ConfirmitDatabaseUserDeployCName as owner of CATI databases"
	Set-DbOwnerForCatiDatabases

    Set-Location $currentLocation

    Write-Host "Install Backend files to $CatiInstallLocation"
    $assembliesLocation = (Get-Item -Path ".\..\..\assemblies" -Verbose).FullName
    $buildVersion = Get-NugetBuildVersion
    cmd.exe /c ".\..\..\_3rdpart\7z\7z.exe x -o`"$CatiInstallLocation`" `"$assembliesLocation\NugetPackages\Confirmit.CATI.Backend.$buildVersion.nupkg`" -aoa"
    
    Set-Location $CatiInstallLocation
    
    Write-Host "Execute Deploy.ps1 script"
    $executionResult = & "$CatiInstallLocation\Deploy.ps1"
    if($LastExitCode -eq -1)
    {
        Write-Host "Exception=$executionResult"
        throw [System.Exception] "Exception occured in Deploy.ps1 script. Stop execution."
    }
    
    Write-Host "Execute PostDeploy.ps1 script"
    $executionResult = & "$CatiInstallLocation\PostDeploy.ps1"
    if($LastExitCode -eq -1)
    {
        Write-Host "Exception=$executionResult"
        throw [System.Exception] "Exception occured in PostDeploy.ps1 script. Stop execution."
    }

    Set-SomeSettingsToDefaultCatiDatabase

    Write-Host "Remove temporary files"
    if((Test-Path "$CatiInstallLocation\_rels"))
    {
        Remove-Item "$CatiInstallLocation\_rels" -Force -Recurse -ErrorAction SilentlyContinue -Confirm:$false
    }

    Remove-Item "$CatiInstallLocation\Deploy.ps1"
    Remove-Item "$CatiInstallLocation\PostDeploy.ps1"
    
    Write-Host "Make IIS application for Supervisor"
    import-module webadministration
    if(!(Test-Path IIS:\AppPools\$CatiParametersSupervisorAppPoolName))
    {
        $newAppPool = New-WebAppPool -Name $CatiParametersSupervisorAppPoolName -Force
        $newAppPool | Set-ItemProperty -Name "processModel.identityType" -Value "LocalSystem"
    }
    
    Remove-WebApplication -Site $CatiParametersSupervisorSiteName -Name $CatiParametersSupervisorVirtualDirectoryName -ErrorAction SilentlyContinue
    Remove-WebApplication -Site $CatiParametersSupervisorSiteName -Name $CatiParametersSupervisorNewVirtualDirectoryName -ErrorAction SilentlyContinue

    New-WebApplication -Name $CatiParametersSupervisorNewVirtualDirectoryName -Site $CatiParametersSupervisorSiteName -PhysicalPath $supervisorNewCodeLocation -ApplicationPool $CatiParametersSupervisorAppPoolName -Force    
}
catch [Exception]
{ 
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}
