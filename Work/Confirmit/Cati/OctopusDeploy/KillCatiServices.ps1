# Run
# Set-ExecutionPolicy RemoteSigned
# if running powershell scripts is disabled on your system

$catiServicePrefixName = "Confirmit.CATI.Backend.Rel"
$catiProcessName = "Confirmit.CATI.Backend"

Write-Host "Disable instance services"
Get-Service $catiServicePrefixName* | Set-Service -PassThru -StartupType disabled
Write-Host "Instance services were disabled"

Write-Host "Kill instance processes"
Stop-Process -name $catiProcessName -PassThru -Force
while(Get-Process $catiProcessName -ErrorAction SilentlyContinue){}
Write-Host "Instance processes were killed"

Write-Host "Enable instance services"
Get-Service $catiServicePrefixName* | Set-Service -PassThru -StartupType auto
Write-Host "Instance services were enabled"
