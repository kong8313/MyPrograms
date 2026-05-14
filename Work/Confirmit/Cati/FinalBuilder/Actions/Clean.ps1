try
{
    . ".\LoadParameters.ps1"

    $currentLocation = (Get-Item -Path ".\" -Verbose).FullName
    $supervisorCodeLocation = (Get-Item -Path ".\..\..\Supervisor\Confirmit.CATI.Supervisor" -Verbose).FullName
    
    Write-Host "Stop CATI services"
    get-service Confirmit.CATI.Backend.Rel* | Stop-Service

    Write-Host "Remove CATI services"
    gwmi win32_service -filter "name like 'Confirmit.CATI.Backend%'" | ForEach-Object {$_.delete()}

    Write-Host "Remove $CatiInstallLocation directory"
    if((Test-Path $CatiInstallLocation))
    {
        Remove-Item $CatiInstallLocation -Force -Recurse -ErrorAction SilentlyContinue -Confirm:$false
    }

    Write-Host "Remove IIS application for Supervisor"
    Remove-WebApplication -Site $CatiParametersSupervisorSiteName -Name $CatiParametersSupervisorVirtualDirectoryName -ErrorAction SilentlyContinue

    Write-Host "Remove IIS application for Simulator"
    Remove-WebApplication -Site $CatiParametersSimulatorSiteName -Name $CatiParametersSimulatorVirtualDirectoryName -ErrorAction SilentlyContinue

    Write-Host "Remove $SimulatorInstallLocation directory"
    if((Test-Path $SimulatorInstallLocation))
    {
        Remove-Item $SimulatorInstallLocation -Force -Recurse -ErrorAction SilentlyContinue -Confirm:$false
    }
}
catch [Exception]
{ 
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}
