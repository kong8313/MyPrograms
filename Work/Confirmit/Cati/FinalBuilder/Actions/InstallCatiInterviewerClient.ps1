try
{
    . ".\LoadParameters.ps1"

    if(!(Test-Path $CatiInterviewerClientPath))
    {
        throw "Path $CatiInterviewerClientPath is not found. Verify that confirmit.catiinterviewer.client repository is cloned and that CatiInterviewerClientPath parameter in InstallationParameters.ps1 file is correct"
    }

    $currentLocation = (Get-Item -Path ".\" -Verbose).FullName
    $interviewerClientCodeLocation = (Get-Item -Path $CatiInterviewerClientPath -Verbose).FullName
    
    Write-Host "Location: $interviewerClientCodeLocation"
    Set-Location $interviewerClientCodeLocation

    Write-Host "Run 'npm i --legacy-peer-deps' command"
    npm i --legacy-peer-deps
    if($LASTEXITCODE -ne 0)
    {
        throw "Error during build of Confirmit.CatiInterviewer.Client"
    }


    Write-Host "Run 'npm run bootstrap --legacy-peer-deps' command"
    npm run bootstrap --legacy-peer-deps
    if($LASTEXITCODE -ne 0)
    {
        throw "Error during build of Confirmit.CatiInterviewer.Client"
    }

    Write-Host "Run 'npm run build:client' command"
    npm run build:client
    if($LASTEXITCODE -ne 0)
    {
        throw "Error during build of Confirmit.CatiInterviewer.Client"
    }

    Write-Host "Run 'npm run build:gateway' command"
    npm run build:gateway
    if($LASTEXITCODE -ne 0)
    {
        throw "Error during build of Confirmit.CatiInterviewer.Client"
    }

    Write-Host "Make IIS application for Interviewer Client"
    import-module webadministration
    
    Remove-WebApplication -Site $CatiParametersSupervisorSiteName -Name $CatiInterviewerClientApplicationName -ErrorAction SilentlyContinue

    New-WebApplication -Name $CatiInterviewerClientApplicationName -Site $CatiParametersSupervisorSiteName -PhysicalPath "$interviewerClientCodeLocation" -ApplicationPool "DefaultAppPool" -Force
	
    Restart-WebAppPool DefaultAppPool
    
    . ".\OctopusDeploy\DeployScripts\CatiInterviewerDeploy.ps1"
    AddHttpConnectionToAllowedServerVariables "Default Web Site"

    Set-Location $currentLocation
}
catch [Exception]
{ 
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}
