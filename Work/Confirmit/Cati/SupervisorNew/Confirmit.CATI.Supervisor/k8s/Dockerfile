ARG IMAGE_TAG=4.8.1

FROM mcr.microsoft.com/dotnet/framework/sdk:$IMAGE_TAG as builder
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
ARG Confirmit__AppVersion
RUN mkdir /app
WORKDIR /app
RUN nuget install WebConfigTransformRunner -ExcludeVersion
COPY Docker.resources/Web.k8s.config .
COPY artifacts/Confirmit.Cati.Supervisor.New.${Confirmit__AppVersion}.nupkg Confirmit.Cati.Supervisor.New.zip
RUN Expand-Archive -LiteralPath Confirmit.Cati.Supervisor.New.zip -DestinationPath nupkg
RUN rm -force nupkg/*.ps1; rm -force nupkg/Web.*.config; rm -recurse -force nupkg/package; rm -recurse -force nupkg/_rels; rm -force nupkg/*.nuspec; rm -force nupkg/*Content_Types*.xml;
RUN if(Test-Path nupkg/Database){rm -recurse -force nupkg/Database}; if(Test-Path nupkg/Confirmit.DatabaseUpdateUtility.exe){rm -force nupkg/Confirmit.DatabaseUpdateUtility.exe};
RUN ren .\nupkg\ig_res ig_res_$env:Confirmit__AppVersion
RUN (gc Web.k8s.config) -replace '\$NugetPackageVersion\$', $env:Confirmit__AppVersion | Out-File -encoding ASCII Web.k8s.config
RUN .\WebConfigTransformRunner\Tools\WebConfigTransformRunner.exe .\nupkg\Web.config Web.k8s.config .\nupkg\Web.config

FROM mcr.microsoft.com/dotnet/framework/aspnet:$IMAGE_TAG as app-production
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
COPY Docker.resources/Fonts/*.ttf /Windows/Fonts/
RUN New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts' -Name 'Arial (TrueType)' -PropertyType String -Value arial.ttf
RUN New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts' -Name 'Arial Black (TrueType)' -PropertyType String -Value ariblk.ttf
RUN New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts' -Name 'Arial Bold (TrueType)' -PropertyType String -Value arialbd.ttf
RUN New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts' -Name 'Arial Bold Italic (TrueType)' -PropertyType String -Value arialbi.ttf
RUN New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts' -Name 'Arial Italic (TrueType)' -PropertyType String -Value ariali.ttf
RUN New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts' -Name 'Verdana (TrueType)' -PropertyType String -Value verdana.ttf
RUN New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts' -Name 'Verdana Bold (TrueType)' -PropertyType String -Value verdanab.ttf
RUN New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts' -Name 'Verdana Bold Italic (TrueType)' -PropertyType String -Value verdanaz.ttf
RUN New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts' -Name 'Verdana Italic (TrueType)' -PropertyType String -Value verdanai.ttf
RUN mkdir /LogMonitor/; [Net.ServicePointManager]::SecurityProtocol = ([Net.SecurityProtocolType]::Tls12 -bor [Net.SecurityProtocolType]::Tls13); Invoke-WebRequest https://github.com/microsoft/windows-container-tools/releases/download/v1.1/LogMonitor.exe -OutFile /LogMonitor/LogMonitor.exe
RUN C:\windows\system32\inetsrv\appcmd.exe set AppPool DefaultAppPool ` \
    /processModel.identityType:NetworkService ` \
    /processModel.idleTimeout:00:00:00 ` \
    /processModel.shutdownTimeLimit:00:05:00 ` \
    /recycling.periodicRestart.time:00:00:00 ` \
    /failure.rapidFailProtection:true ` \    
    /enable32BitAppOnWin64:false ` \
    /autoStart:true /startMode:AlwaysRunning;
COPY Docker.resources/LogMonitor/ /LogMonitor/
RUN C:\windows\system32\inetsrv\appcmd.exe add app /site.name:'Default Web Site' /path:/supervisor /physicalPath:c:\app;
WORKDIR /app
ARG Confirmit__AppVersion
ARG Confirmit__InstallationDate
ENV Confirmit__ContainerEnvironment=true \
    Confirmit__Logging__EnableLogToStdStreams=false \
    Confirmit__ServiceVersion=${Confirmit__AppVersion} \
    Confirmit__InstallationDate=${Confirmit__InstallationDate}
COPY --from=builder /app/nupkg .
ENTRYPOINT ["C:\\LogMonitor\\LogMonitor.exe", "C:\\ServiceMonitor.exe", "w3svc"]
