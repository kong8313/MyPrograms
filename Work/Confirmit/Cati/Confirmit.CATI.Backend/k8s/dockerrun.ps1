param (
    [Parameter(mandatory=$true)]
    [string]$version
)
docker stop cati-backend
docker rm cati-backend
docker run --rm --name cati-backend -it `
-e "Confirmit__ContainerCryptoKey=mycryptokey" `
-e "Confirmit__SQLServerName=co-osl-devhv14.firmglobal.com" `
-e "Confirmit__SurveyC=21A58108772CC20BBD51BEF49CDA37C17823367049C59E41304CCD304AD4B3E89B340E785F5ADE54980ABD36ADE9F585" `
-e "Confirmit__Authentication__ClientKeyGeneratorSecret=30020ECE-E3DE-4EA9-A57C-9265DEE06C83" `
-e "Confirmit__Logging__Level=Debug" `
-p 8080:80 `
dockerv2-confirmit-local.kube.firmglobal.com:30100/confirmit/cati-backend:$version /Instance 1