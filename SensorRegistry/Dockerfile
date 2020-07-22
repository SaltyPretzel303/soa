FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN apt-get update && apt-get install nano

WORKDIR /sensor_registry

COPY ./SensorRegistry.csproj ./SensorRegistry.csproj

COPY ./src ./src
COPY ./appsettings.json ./appsettings.json
COPY ./service_config.json ./service_config.json

RUN dotnet restore
RUN dotnet build
RUN dotnet publish --output /registry_bin

WORKDIR /registry_bin
#WORKDIR /sensor_registry/bin/Debug/netcoreapp3.1/publish

# ENV ASPNETCORE_URLS http://+:5000

ENTRYPOINT ["dotnet", "SensorRegistry.dll"]
