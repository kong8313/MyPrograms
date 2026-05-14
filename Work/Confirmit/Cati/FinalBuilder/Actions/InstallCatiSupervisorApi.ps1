try
{
    . ".\LoadParameters.ps1"

    if(!(Test-Path $CatiSupervisorApiPath))
    {
        throw "Path $CatiSupervisorApiPath is not found. Verify that confirmit.catisupervisor.api repository is cloned and that CatiSupervisorApiPath parameter in InstallationParameters.ps1 file is correct"
    }

    $currentLocation = (Get-Item -Path ".\" -Verbose).FullName
    $supervisorApiCodeLocation = (Get-Item -Path $CatiSupervisorApiPath -Verbose).FullName
    $webconfigFileFullPath = "$supervisorApiCodeLocation\code\src\confirmit.catisupervisor.api\web.config"
    $appSettingsFileFullPath = "$supervisorApiCodeLocation\code\src\confirmit.catisupervisor.api\appsettings.json"
    $appSettingsDevelopmentFileFullPath = "$supervisorApiCodeLocation\code\src\confirmit.catisupervisor.api\appsettings.Development.json"
    
    Write-Host "Location: $supervisorApiCodeLocation"
    Set-Location $supervisorApiCodeLocation

    dotnet build .\code\Confirmit.CatiSupervisor.Api.sln	
    if($LASTEXITCODE -ne 0)
    {
        throw "Error during build of Confirmit.CatiSupervisor.Api.sln"
    }

    Write-Host "Make IIS application for Supervisor Api"
    import-module webadministration
    
    if(!(Test-Path IIS:\AppPools\$CatiSupervisorApiApplicationPoolName))
    {
        $newAppPool = New-WebAppPool -Name $CatiSupervisorApiApplicationPoolName -Force
        $newAppPool | Set-ItemProperty -Name "processModel.identityType" -Value "LocalSystem"
        $newAppPool | Set-ItemProperty -Name "managedRuntimeVersion" -Value ""
    }
	
    Remove-WebApplication -Site $CatiParametersSupervisorSiteName -Name $CatiSupervisorApiApplicationName -ErrorAction SilentlyContinue

    New-WebApplication -Site $CatiParametersSupervisorSiteName -Name $CatiSupervisorApiApplicationName -PhysicalPath "$supervisorApiCodeLocation\code\src\confirmit.catisupervisor.api" -ApplicationPool $CatiSupervisorApiApplicationPoolName -Force
	
    Restart-WebAppPool $CatiSupervisorApiApplicationPoolName

    Set-Location $currentLocation
}
catch [Exception]
{ 
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}
