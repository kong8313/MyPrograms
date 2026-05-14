
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8.1 as builder
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
ARG Confirmit__ServiceVersion
RUN mkdir /app
WORKDIR /app
RUN nuget install WebConfigTransformRunner -ExcludeVersion
COPY Docker.resources/App.k8s.config .
COPY artifacts/Confirmit.CATI.Backend.${Confirmit__ServiceVersion}.nupkg Confirmit.CATI.Backend.zip
RUN Expand-Archive -LiteralPath Confirmit.CATI.Backend.zip -DestinationPath nupkg
RUN rm -recurse -force nupkg/Installation; rm -force nupkg/*.ps1; rm -recurse -force nupkg/package; rm -recurse -force nupkg/_rels; rm -force nupkg/*.nuspec; rm -force nupkg/*Content_Types*.xml;
RUN .\WebConfigTransformRunner\Tools\WebConfigTransformRunner.exe .\nupkg\Confirmit.CATI.Backend.exe.config App.k8s.config .\nupkg\Confirmit.CATI.Backend.exe.config

FROM mcr.microsoft.com/dotnet/framework/runtime:4.8.1 as app-production
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
WORKDIR /app
ARG Confirmit__ServiceVersion
ARG Confirmit__InstallationDate
ENV Confirmit__ContainerEnvironment=true \
    Confirmit__Logging__EnableLogToStdStreams=true \
    Confirmit__ServiceVersion=${Confirmit__ServiceVersion} \
    Confirmit__InstallationDate=${Confirmit__InstallationDate}
COPY --from=builder /app/nupkg .
ENTRYPOINT ["C:\\app\\Confirmit.CATI.Backend.exe"]
