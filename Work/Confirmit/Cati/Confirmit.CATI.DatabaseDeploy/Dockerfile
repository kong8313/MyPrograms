FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine as builder
ARG NuGetPackageSourceKey
ARG NuGetPackageSourceUrl
ARG NuGetPackageSourceCredentials
ENV NuGetPackageSourceCredentials_$NuGetPackageSourceKey=$NuGetPackageSourceCredentials
RUN dotnet nuget add source --name $NuGetPackageSourceKey $NuGetPackageSourceUrl
RUN mkdir -p /artifacts/PublishOutput
ENV TEAMCITY_PROJECT_NAME=Confirmit.CATI.DatabaseDeploy
RUN mkdir -p /src/app/Confirmit.CATI.DatabaseDeploy
WORKDIR  /src/app/Confirmit.CATI.DatabaseDeploy

COPY NuGet.config code/Confirmit.CATI.DatabaseDeploy.sln ./code/
COPY code/src/Confirmit.CATI.DatabaseDeploy/Confirmit.CATI.DatabaseDeploy.csproj ./code/src/Confirmit.CATI.DatabaseDeploy/
COPY code/src/Confirmit.CATI.DatabaseUpdateLibraryCore/Confirmit.CATI.DatabaseUpdateLibraryCore.csproj ./code/src/Confirmit.CATI.DatabaseUpdateLibraryCore/
RUN dotnet restore ./code/
COPY . .
RUN dotnet publish --no-restore --configuration Release --output /artifacts/PublishOutput code/src/Confirmit.CATI.DatabaseDeploy/Confirmit.CATI.DatabaseDeploy.csproj

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine as app-production
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV Confirmit__ContainerEnvironment true
ENV Confirmit__Logging__EnableLogToStdStreams true
ARG Confirmit__ServiceVersion
ENV Confirmit__ServiceVersion ${Confirmit__ServiceVersion}
ARG Confirmit__InstallationDate
ENV Confirmit__InstallationDate ${Confirmit__InstallationDate}
WORKDIR /app
ENTRYPOINT ["dotnet", "Confirmit.CATI.DatabaseDeploy.dll"]
COPY --from=builder /artifacts/PublishOutput/ .

