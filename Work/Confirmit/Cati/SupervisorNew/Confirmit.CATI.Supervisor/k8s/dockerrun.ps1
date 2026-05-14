param (
    [Parameter(mandatory=$true)]
    [string]$version
)
docker stop cati-supervisor-legacy-client
docker rm cati-supervisor-legacy-client
mkdir -Force C:\confirmit_logs\container\services
mkdir -Force c:\inetpub\mailroot\pickup
docker run --rm --name cati-supervisor-legacy-client -it `
-e "Confirmit__ContainerEnvironment=true" `
-e "Confirmit__ContainerCryptoKey=mycryptokey" `
-e "Confirmit__SQLServerName=$env:COMPUTERNAME.firmglobal.com" `
-e "Confirmit__SurveyC=21A58108772CC20BBD51BEF49CDA37C17823367049C59E41304CCD304AD4B3E89B340E785F5ADE54980ABD36ADE9F585" `
-e "Confirmit__Authentication__ClientKeyGeneratorSecret=30020ECE-E3DE-4EA9-A57C-9265DEE06C83" `
-e "Confirmit__Logging__Level=Debug" `
-p 8080:80 `
-v C:\confirmit_logs\container:c:\confirmit_logs `
-v c:\inetpub\mailroot\pickup:c:\inetpub\mailroot\pickup `
confirmithorizonsdev.azurecr.io/confirmit/cati-supervisor-legacy-client:$version