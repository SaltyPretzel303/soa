FROM ubuntu:16.04
FROM mcr.microsoft.com/dotnet/core/sdk:2.2

RUN apt-get update && apt-get install nano

WORKDIR /sensor_service

COPY ./SensorService.csproj ./SensorService.csproj
RUN dotnet restore

COPY ./src ./src
COPY ./appsettings.json ./appsettings.json
COPY ./service_config.json ./service_config.json

COPY ./data /data

RUN dotnet build
RUN dotnet publish

# ENV ASPNETCORE_URLS http://+:5000

WORKDIR /sensor_service/bin/Debug/netcoreapp2.2/publish

# ENTRYPOINT ["dotnet", "./SensorService.dll"]
ENTRYPOINT [ "/bin/bash" ]