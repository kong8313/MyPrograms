try
{
    . ".\LoadParameters.ps1"

    if(!(Test-Path $CatiInterviewerApiPath))
    {
        throw "Path $CatiInterviewerApiPath is not found. Verify that confirmit.catiinterviewer.api repository is cloned and that CatiInterviewerApiPath parameter in InstallationParameters.ps1 file is correct"
    }

    $currentLocation = (Get-Item -Path ".\" -Verbose).FullName
    $interviewerApiCodeLocation = (Get-Item -Path $CatiInterviewerApiPath -Verbose).FullName
    $webconfigFileFullPath = "$interviewerApiCodeLocation\code\src\confirmit.catiinterviewer.api\web.config"
    $appSettingsFileFullPath = "$interviewerApiCodeLocation\code\src\confirmit.catiinterviewer.api\appsettings.json"
    $appSettingsDevelopmentFileFullPath = "$interviewerApiCodeLocation\code\src\confirmit.catiinterviewer.api\appsettings.Development.json"
    
    Write-Host "Location: $interviewerApiCodeLocation"
    Set-Location $interviewerApiCodeLocation

    dotnet build .\code\Confirmit.CatiInterviewer.Api.sln	
    if($LASTEXITCODE -ne 0)
    {
        throw "Error during build of Confirmit.CatiInterviewer.Api.sln"
    }

	Write-Host "Make IIS application for Interviewer Api"
    import-module webadministration
    
    if(!(Test-Path IIS:\AppPools\$CatiInterviewerApiApplicationPoolName))
    {
        $newAppPool = New-WebAppPool -Name $CatiInterviewerApiApplicationPoolName -Force
        $newAppPool | Set-ItemProperty -Name "processModel.identityType" -Value "LocalSystem"
        $newAppPool | Set-ItemProperty -Name "managedRuntimeVersion" -Value ""
    }
	
    Remove-WebApplication -Site $CatiParametersSupervisorSiteName -Name $CatiInterviewerApiApplicationName -ErrorAction SilentlyContinue

    New-WebApplication -Site $CatiParametersSupervisorSiteName -Name $CatiInterviewerApiApplicationName -PhysicalPath "$interviewerApiCodeLocation\code\src\confirmit.catiinterviewer.api" -ApplicationPool $CatiInterviewerApiApplicationPoolName -Force
	
    Restart-WebAppPool $CatiInterviewerApiApplicationPoolName

    Set-Location $currentLocation
}
catch [Exception]
{ 
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}
