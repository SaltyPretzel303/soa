FROM mcr.microsoft.com/dotnet/core/sdk:2.2

RUN apt-get update && apt-get install nano

WORKDIR /collector_service

COPY ./CollectorService.csproj ./CollectorService.csproj
RUN dotnet restore

COPY ./src ./src
COPY ./appsettings.json ./appsettings.json
COPY ./service_config.json ./service_config.json

RUN dotnet build
RUN dotnet publish

WORKDIR /collector_service/bin/Debug/netcoreapp2.2/publish

# ENV ASPNETCORE_URLS http://+:5000

ENTRYPOINT ["dotnet", "/collector_service/bin/Debug/netcoreapp2.2/publish/CollectorService.dll"]
