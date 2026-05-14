. ".\Utils\SqlQueryUtil.ps1"
. ".\..\InstallationParameters.ps1"

if(Test-Path ('.\..\InstallationParametersEx.ps1'))
{
    Write-Host "Overriden parameter file was found"
    . ".\..\InstallationParametersEx.ps1"
}