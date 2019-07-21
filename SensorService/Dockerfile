FROM mcr.microsoft.com/dotnet/core/sdk:2.2

RUN apt-get update && apt-get install nano

WORKDIR /sensor_service

COPY ./SensorService.csproj ./SensorService.csproj
RUN dotnet restore

COPY ./src ./src
COPY ./appsettings.json ./appsettings.json
COPY ./docker_network.json ./docker_network.json

RUN dotnet build
RUN dotnet publish

ENV ASPNETCORE_URLS http://+:5000

ENTRYPOINT ["dotnet", "./bin/Debug/netcoreapp2.2/publish/SensorService.dll"]
