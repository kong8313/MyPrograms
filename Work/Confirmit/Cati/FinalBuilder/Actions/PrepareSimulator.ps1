try
{
    function MakeDumpCreatorCmdFile($assemblyVersion)
    {
        #New-Item -ItemType file -Path "$SimulatorInstallLocation\dumpCreator.cmd"

        #Set-Content $consoleConsfigPath

        "@if `"%_echo%`"==`"`" echo off
setlocal
set ROOT_DUMP_FOLDER_PATH=<ROOT_DUMP_FOLDER_PATH>
set PROCDUMP_PATH=`"<PROCDUMP_PATH>`"
set ADDITIONAL_PARAMETERS=<ADDITIONAL_PARAMETERS>
set CURRENT_VERSION=`"$assemblyVersion`"
for /f `"delims=`" %%x in ('cscript /nologo get_date_time.vbs') do %%x
set TIME_FOR_FOLDER=%YEAR%_%MONTH%_%DAY% %HOUR%_%MIN%_%SEC%
set DUMP_FOLDER_PATH=%ROOT_DUMP_FOLDER_PATH%%TIME_FOR_FOLDER%
set LOG_FILE_PATH=`"%DUMP_FOLDER_PATH%\dump_log.txt`"
set DUMP_FOLDER_PATH=`"%DUMP_FOLDER_PATH%`"
set COMMAND=%PROCDUMP_PATH% %1 %DUMP_FOLDER_PATH% %ADDITIONAL_PARAMETERS%

mkdir %DUMP_FOLDER_PATH%

echo Current version=%CURRENT_VERSION%>>%LOG_FILE_PATH%
echo %DATE% %TIME%:%COMMAND% >>%LOG_FILE_PATH%
%COMMAND% >>%LOG_FILE_PATH%
echo ____________________________________________________________________________________________ >>%LOG_FILE_PATH%


Endlocal" | out-file "$SimulatorInstallLocation\dumpCreator.cmd"

        "Wscript.Echo(`"set YEAR=`" & DatePart(`"yyyy`", Now))
Wscript.Echo(`"set MONTH=`" & DatePart(`"m`", Now))
Wscript.Echo(`"set DAY=`" & DatePart(`"d`", Now))
Wscript.Echo(`"set HOUR=`" & DatePart(`"h`", Now))
Wscript.Echo(`"set MIN=`" & DatePart(`"n`", Now))
Wscript.Echo(`"set SEC=`" & DatePart(`"s`", Now))" | out-file "$SimulatorInstallLocation\get_date_time.vbs"

    }

    . ".\LoadParameters.ps1"
    
    $dialerServicePath = (Get-Item -Path ".\..\..\assemblies\Telephony\DialerService" -Verbose).FullName
    $simulatorServicePath = (Get-Item -Path ".\..\..\assemblies\Telephony\SimulatorDialerDriver" -Verbose).FullName
    $dialerClientPath = (Get-Item -Path ".\..\..\Confirmit.CATI.Telephony\Simulator.Client\build\" -Verbose).FullName
    $audioTemplatesPath = (Get-Item -Path ".\..\..\Confirmit.CATI.Telephony\SimulatorDialerDriver\AudioTemplates\" -Verbose).FullName
    $binLocation = "$SimulatorInstallLocation\bin"    

    if(!(Test-Path($binLocation)))
    {
        Write-Host "Make $binLocation directory"
        New-Item -ItemType directory -Path "$binLocation"
    }  

    if(Test-Path("$SimulatorInstallLocation\Web.config"))
    {
        Write-Host "Backup Web.config file"
        $assembly = [Reflection.Assembly]::Loadfile("$dialerServicePath\Confirmit.CATI.Common.dll")    
        $assemblyVersion = $assembly.GetName().version
        $currentDateTime = Get-Date -Format "yyyy-MM-dd_hh-mm-ss"
        $configBackupsPath = "$SimulatorInstallLocation\ConfigBackups"
        $backupWebConfigPath = "$configBackupsPath\"+$assemblyVersion+"_"+$currentDateTime+"_Web.config"

        New-Item -ItemType directory -Path "$configBackupsPath" -Force
        Move-Item -Path "$SimulatorInstallLocation\Web.config" -Destination $backupWebConfigPath -Force
    }

    Write-Host "Copy files to $SimulatorInstallLocation"
    Get-ChildItem -Path $simulatorServicePath -File | Copy-Item -Destination $binLocation -Force

    $excludeFiles = "Web.config", "DialerService.svc"
    Get-ChildItem $dialerServicePath -File |         
        Where-Object{$_.Name -notin $excludeFiles} | 
        Copy-Item -Destination $binLocation -Force

    Copy-Item -Path "$dialerServicePath\Web.config" -Destination "$SimulatorInstallLocation\Web.config"
    Copy-Item -Path "$dialerServicePath\DialerService.svc" -Destination "$SimulatorInstallLocation\DialerService.svc" -Force

    if(!(Test-Path("$SimulatorInstallLocation\dumpCreator.cmd")))
    {
        Write-Host "Make dumpCreator.cmd file"
        MakeDumpCreatorCmdFile $assemblyVersion
    }

    $appDataLocation = "$SimulatorInstallLocation\App_Data"
    if(!(Test-Path($appDataLocation)))
    {
        New-Item -ItemType directory -Path "$appDataLocation"
    }

    if(!(Test-Path("$appDataLocation\SimulatorScenario.xml")))
    {
        Write-Host "Copy SimulatorScenario.xml to App_Data folder"
        Copy-Item -Path "$binLocation\SimulatorScenario.xml" -Destination "$appDataLocation\SimulatorScenario.xml"
    }

    Move-Item -Path "$binLocation\SimulatorScenario.xml" -Destination "$SimulatorInstallLocation\SimulatorScenario.xml"  -Force
    Move-Item -Path "$binLocation\SoftphoneSimulatorClient.html" -Destination "$SimulatorInstallLocation\SoftphoneSimulatorClient.html"  -Force

    $clientLocation = "$appDataLocation\www"
    if(!(Test-Path($clientLocation)))
    {
        New-Item -ItemType directory -Path "$clientLocation"
    }

    Copy-Item -Path "$dialerClientPath\*" -Destination "$clientLocation" -Recurse

    $audioTemplatesLocation = "$appDataLocation\AudioTemplates"
    if(!(Test-Path($audioTemplatesLocation)))
    {
        New-Item -ItemType directory -Path "$audioTemplatesLocation"
    }

    Copy-Item -Path "$audioTemplatesPath\*" -Destination "$audioTemplatesLocation" -Recurse

    Write-Host "Change config file"
    $xml=New-Object XML
    $xml.Load("$SimulatorInstallLocation\Web.config")

    $node = $xml.SelectSingleNode("/configuration/system.serviceModel/client/endpoint[@name='DialerEventsHandlerServiceEndpoint']");
    $node.SetAttribute("address", "https://$CatiParametersSimulatorEndpointServerName/DialerMultimodeInstance");

    $node = $xml.SelectSingleNode("/configuration/system.serviceModel/client/endpoint[@name='ErrorReportingServiceEndpoint']");
    $node.SetAttribute("address", "https://$CatiParametersSimulatorEndpointServerName/ErrorReportingMultimodeInstance");

    $node = $xml.SelectSingleNode("/configuration/applicationSettings/Confirmit.CATI.Telephony.DialerService.Settings/setting[@name='DialerId']/value");
    $node.InnerText = $CatiParametersSimulatorDialerId;

    $node = $xml.SelectSingleNode("/configuration/system.diagnostics/sharedListeners/add[@name='DialerLogFileListener']");
    $node.SetAttribute("LoggingFileName", "LTUSimulator(G)WS_%datetime%.log");
    $node.SetAttribute("LoggingPath", $CatiParametersSimulatorLoggingPath);

    $node = $xml.SelectSingleNode("/configuration/configSections/sectionGroup")
    $sectionNode = $xml.CreateNode("element", "section", "")
    $sectionNode.SetAttribute("name", "SimulatorDialerDriver.Settings")
    $sectionNode.SetAttribute("type", "System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
    $sectionNode.SetAttribute("requirePermission", "false")
    $node.AppendChild($sectionNode)
    
    $node = $xml.SelectSingleNode("/configuration/applicationSettings")
    $simulatorDialerDriverSettingsNode = $xml.CreateNode("element", "SimulatorDialerDriver.Settings", "")
    $settingNode = $xml.CreateNode("element", "setting", "")
    $settingNode.SetAttribute("name", "WebApiUrl")
    $settingNode.SetAttribute("serializeAs", "String")
    $valueNode = $xml.CreateNode("element", "value", "")
    $valueNode.InnerText = $CatiParametersSimulatorWebApiUrl
    $settingNode.AppendChild($valueNode)
    $simulatorDialerDriverSettingsNode.AppendChild($settingNode)
    $node.AppendChild($simulatorDialerDriverSettingsNode)

    $xml.Save("$SimulatorInstallLocation\Web.config")

    Write-Host "Make IIS application for Simulator"
    import-module webadministration
    if(!(Test-Path IIS:\AppPools\$CatiParametersSimulatorAppPoolName))
    {
        $newAppPool = New-WebAppPool -Name $CatiParametersSimulatorAppPoolName -Force
        $newAppPool | Set-ItemProperty -Name "processModel.identityType" -Value "LocalSystem"
        $newAppPool | Set-ItemProperty -Name "recycling.periodicRestart" -Value "0"
    }
    
    Remove-WebApplication -Site $CatiParametersSimulatorSiteName -Name $CatiParametersSimulatorVirtualDirectoryName -ErrorAction SilentlyContinue
    New-WebApplication -Name $CatiParametersSimulatorVirtualDirectoryName -Site $CatiParametersSimulatorSiteName -PhysicalPath $SimulatorInstallLocation -ApplicationPool $CatiParametersSimulatorAppPoolName -Force    
    Set-ItemProperty "IIS:\Sites\$CatiParametersSimulatorSiteName\$CatiParametersSimulatorVirtualDirectoryName" -Name preloadEnabled -Value True

    Restart-WebAppPool $CatiParametersSimulatorAppPoolName
}
catch [Exception]
{ 
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}
