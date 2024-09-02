# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app
COPY . .

RUN dotnet tool install -g dotnet-ef

ENTRYPOINT ["dotnet", "watch", "run", "--urls=http://+:5001", "--project", "API/API.csproj"]
