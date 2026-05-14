$executionPath = (Get-Item -Path ".\" -Verbose).FullName;

$installationFolder = "$executionPath\Installation"
Write-Host "Remove Installation catalog $installationFolder"
if((Test-Path $installationFolder))
{
    Get-ChildItem -Path $installationFolder -Recurse | Remove-Item -Force -Recurse -ErrorAction SilentlyContinue -Confirm:$false
    Remove-Item $installationFolder -Force -Recurse -ErrorAction SilentlyContinue -Confirm:$false
}
