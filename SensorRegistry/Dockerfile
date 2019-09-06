FROM mcr.microsoft.com/dotnet/core/sdk:2.2

RUN apt-get update && apt-get install nano

WORKDIR /sensor_registry

COPY ./SensorRegistry.csproj ./SensorRegistry.csproj
RUN dotnet restore

COPY ./src ./src
COPY ./appsettings.json ./appsettings.json
COPY ./service_config.json ./service_config.json

RUN dotnet build
RUN dotnet publish

WORKDIR /sensor_registry/bin/Debug/netcoreapp2.2/publish

# ENV ASPNETCORE_URLS http://+:5000

ENTRYPOINT ["dotnet", "SensorRegistry.dll"]
