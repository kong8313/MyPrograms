ARG IMAGE_TAG=4.8.1

FROM mcr.microsoft.com/dotnet/framework/sdk:$IMAGE_TAG as builder
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
ARG Confirmit__AppVersion
RUN mkdir /app
WORKDIR /app
RUN nuget install WebConfigTransformRunner -ExcludeVersion
COPY Docker.resources/Web.k8s.config .
COPY artifacts/Confirmit.CATI.Dialer.AWSConnect.${Confirmit__AppVersion}.nupkg Confirmit.CATI.Dialer.AWSConnect.zip
RUN Expand-Archive -LiteralPath Confirmit.CATI.Dialer.AWSConnect.zip -DestinationPath nupkg -Force
RUN rm -force nupkg/Web.*.config; rm -recurse -force nupkg/package; rm -recurse -force nupkg/_rels; rm -force nupkg/*.nuspec; rm -force nupkg/*Content_Types*.xml;
RUN .\WebConfigTransformRunner\Tools\WebConfigTransformRunner.exe .\nupkg\Web.config Web.k8s.config .\nupkg\Web.config

FROM mcr.microsoft.com/dotnet/framework/wcf:$IMAGE_TAG as app-production
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
RUN mkdir /LogMonitor/; [Net.ServicePointManager]::SecurityProtocol = ([Net.SecurityProtocolType]::Tls12 -bor [Net.SecurityProtocolType]::Tls13); Invoke-WebRequest https://github.com/microsoft/windows-container-tools/releases/download/v2.0.2/LogMonitor.exe -OutFile /LogMonitor/LogMonitor.exe
RUN C:\windows\system32\inetsrv\appcmd.exe set AppPool DefaultAppPool ` \
    /processModel.identityType:LocalSystem ` \
    /processModel.idleTimeout:00:00:00 ` \
    /processModel.shutdownTimeLimit:00:05:00 ` \
    /recycling.periodicRestart.time:00:00:00 ` \
    /failure.rapidFailProtection:true ` \    
    /enable32BitAppOnWin64:true ` \
    /autoStart:true /startMode:AlwaysRunning;
COPY Docker.resources/LogMonitor/ /LogMonitor/
RUN C:\windows\system32\inetsrv\appcmd.exe set vdir 'Default Web Site/' /physicalPath:c:\app;
WORKDIR /app
ARG Confirmit__AppVersion
ARG Confirmit__InstallationDate
ENV Confirmit__ContainerEnvironment=true \
    Confirmit__Logging__EnableLogToStdStreams=false \
    Confirmit__ServiceVersion=${Confirmit__AppVersion} \
    Confirmit__InstallationDate=${Confirmit__InstallationDate}
COPY --from=builder /app/nupkg .
ENTRYPOINT ["C:\\LogMonitor\\LogMonitor.exe", "C:\\ServiceMonitor.exe", "w3svc"]
