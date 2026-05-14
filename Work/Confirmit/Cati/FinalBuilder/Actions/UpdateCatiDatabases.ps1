try
{  
    function RemoveAndLineFeed($text)
    {
        return $text.TrimEnd("`r", "`n")
    }

    . ".\LoadParameters.ps1"
        
    $assembliesLocation = (Get-Item -Path ".\..\..\assemblies" -Verbose).FullName
    Write-Host "assembliesLocation=$assembliesLocation"

    # Load external C# dll's
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Installation\CatiInstallation.dll")
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Installation\CustomActionLibrary.dll")
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Installation\BootstrapperLibrary.dll")
    
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Confirmit.CATI.Installation.Common.dll")

    [Reflection.Assembly]::LoadFile("$assembliesLocation\Confirmit.Configuration.Bootstrap.dll")
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Confirmit.Configuration.dll")
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Confirmit.Databases.dll")
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Confirmit.DataServices.RDataAccess.dll")
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Confirmit.Security.Crypto.dll")
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Confirmit.CATI.DatabaseUpdateLibrary.dll")
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Confirmit.CATI.Common.dll")
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Confirmit.CATI.Core.dll")
    [Reflection.Assembly]::LoadFile("$assembliesLocation\Confirmit.Security.Crypto.Web.dll")

    $currentLocation = (Get-Item -Path ".\..\" -Verbose).FullName
    $logFilePath = [System.IO.Path]::Combine($currentLocation, "InstallationLog.txt")

    $logger = new-object Confirmit.CATI.Installation.Common.FileAndConsoleLogger($logFilePath)
    $installationFacade = new-object CatiInstallation.InstallationFacade($logger)
    
    $confirmlogConnectionString = "Data Source=$ConfirmitDatabaseServerSystemServerName;Initial Catalog=Confirmlog;User ID=$ConfirmitDatabaseUserDeployCName;Password=$ConfirmitDatabaseUserDeployCPassword;Connect Timeout=120"
    $encryptedConfirmlogConnectionString = RemoveAndLineFeed(& $assembliesLocation\Installation\CryptographicUtility.exe "$confirmlogConnectionString" | Out-String)

    Write-Host "Start updating CATI databases"
    
    $installationFacade.UpdateDatabases()
}
catch [Exception]
{ 
    "Failed to run package script:"
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}  