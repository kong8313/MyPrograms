param (
    [Parameter(mandatory=$false)]
    [string]$version = "$(dotnet-gitversion /showvariable NuGetVersionV2)"
)
docker stop cati-databasedeploy
docker rm cati-databasedeploy
docker run --rm --name cati-databasedeploy -it `
-e "Confirmit__ContainerCryptoKey=mycryptokey" `
-e "Confirmit__SQLServerName=co-osl-devhv14.firmglobal.com" `
-e "Confirmit__Database__User__SystemAdmin__Name=sa" `
-e "Confirmit__Database__User__SystemAdmin__Password=firm" `
-e "Confirmit__SurveyC=21A58108772CC20BBD51BEF49CDA37C17823367049C59E41304CCD304AD4B3E89B340E785F5ADE54980ABD36ADE9F585" `
-e "Confirmit__Authentication__ClientKeyGeneratorSecret=30020ECE-E3DE-4EA9-A57C-9265DEE06C83" `
-e "Confirmit__Logging__Level=Debug" `
dockerv2-confirmit-local.kube.firmglobal.com:30100/confirmit/cati-databasedeploy:$version