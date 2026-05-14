param (
    [Parameter(mandatory=$true)]
    [string]$version
)
docker stop cati-dialer-simulator
docker rm cati-dialer-simulator
mkdir -Force C:\confirmit_logs\container\DialerLogs
docker run --rm --name cati-dialer-simulator -it `
-e "Confirmit__DefaultHostname=localhost" --entrypoint powershell -it `
-p 8080:80 `
-p 3838:3838 `
-v C:\confirmit_logs\container\DialerLogs:c:\DialerLogs `
confirmithorizonsdev.azurecr.io/confirmit/cati-dialer-simulator:$version