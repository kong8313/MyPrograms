function Get-NormalizedVersion {
	param (
        [Parameter(Mandatory=$true)] [string] [ValidateNotNullOrEmpty()] $Version
    )
	
    $arr = $Version.Split('.')
    $major = $arr[0]
    $minor = $arr[1]
    $build = $arr[2]

    if($build -like "*-*"){
        $arr = $build.Split('-')
        $normalBuild = $arr[0]
    }
    else {
        $normalBuild = $build
    }

	return "$major.$minor.$normalBuild.0"
}

$installLocation = (Get-Item -Path ".\" -Verbose).FullName
$sideBySideName = "Rel"

[Reflection.Assembly]::LoadFile("$installLocation\Installation\Confirmit.CATI.Installation.Common.dll")

[Confirmit.CATI.Installation.Common.TopMostMessageBox]::IsQuietMode = $true

$logFilePath = [System.IO.Path]::Combine($installLocation, "InstallationLog.txt")
$logger = new-object Confirmit.CATI.Installation.Common.FileAndConsoleLogger($logFilePath)
Write-Host "Log file name: '$logFilePath'"