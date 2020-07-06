FROM ubuntu:16.04
FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN apt-get update && apt-get install nano

WORKDIR /sensor_service

COPY ./SensorService.csproj ./SensorService.csproj
RUN dotnet restore

COPY ./src ./src
COPY ./appsettings.json ./appsettings.json

# data is going to be accessed from mounted volume
# used to speed up build process
# COPY ./data /data

RUN dotnet build
RUN dotnet publish

COPY ./DistributionProgram/ /DistributionProgram
COPY ./service_config.json /DistributionProgram/template

RUN ln -s /sensor_service/bin/Debug/netcoreapp2.2/publish /sensor_program

WORKDIR /DistributionProgram
RUN /DistributionProgram/DistributionScript.sh
# ENTRYPOINT ["sh", "/DistributionScript/DistributionScript.sh"]
ENTRYPOINT [ "/bin/bash" ]

# ENV ASPNETCORE_URLS http://+:5000