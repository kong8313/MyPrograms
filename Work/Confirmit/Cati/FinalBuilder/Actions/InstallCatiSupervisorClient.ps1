try
{
    . ".\LoadParameters.ps1"

    if(!(Test-Path $CatiSupervisorClientPath))
    {
        throw "Path $CatiSupervisorClientPath is not found. Verify that confirmit.catisupervisor.client repository is cloned and that CatiSupervisorClientPath parameter in InstallationParameters.ps1 file is correct"
    }

    $currentLocation = (Get-Item -Path ".\" -Verbose).FullName
    $supervisorClientCodeLocation = (Get-Item -Path $CatiSupervisorClientPath -Verbose).FullName
    
    Write-Host "Location: $supervisorClientCodeLocation"
    Set-Location $supervisorClientCodeLocation

    npm i --legacy-peer-deps
    if($LASTEXITCODE -ne 0)
    {
        throw "Error during build of Confirmit.CatiSupervisor.Client"
    }

    npm run build:client:dev
    if($LASTEXITCODE -ne 0)
    {
        throw "Error during build of Confirmit.CatiSupervisor.Client"
    }

    npm run build:server
    if($LASTEXITCODE -ne 0)
    {
        throw "Error during build of Confirmit.CatiSupervisor.Client"
    }

    Copy-Item -Path "$supervisorClientCodeLocation/client/js/legacy/yui.js" -Destination "$supervisorClientCodeLocation/www/static/js"
    Copy-Item -Path "$supervisorClientCodeLocation/client/js/legacy/OverlayLightBox.js" -Destination "$supervisorClientCodeLocation/www/static/js"
    Copy-Item -Path "$supervisorClientCodeLocation/client/js/legacy/common.js" -Destination "$supervisorClientCodeLocation/www/static/js"

    Write-Host "Make IIS application for Supervisor Client"
    import-module webadministration
    
    Remove-WebApplication -Site $CatiParametersSupervisorSiteName -Name $CatiSupervisorClientApplicationName -ErrorAction SilentlyContinue

    New-WebApplication -Name $CatiSupervisorClientApplicationName -Site $CatiParametersSupervisorSiteName -PhysicalPath "$supervisorClientCodeLocation\www" -ApplicationPool "DefaultAppPool" -Force
	
    Restart-WebAppPool DefaultAppPool

    . "$supervisorClientCodeLocation\OctopusDeploy\Confirmit.Deployment.Deploy2IISWithOctopus\Content\DeployScripts\CatiSupervisorDeploy.ps1"
    AddHttpConnectionToAllowedServerVariables "Default Web Site"

    Set-Location $currentLocation
}
catch [Exception]
{ 
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}
