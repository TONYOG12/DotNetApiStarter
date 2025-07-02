# syntax=docker/dockerfile:1

# --------
# Stage 1: Build
# --------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

WORKDIR /app

# Copy everything
COPY . . 

RUN dotnet tool install -g dotnet-ef

RUN apt-get update && apt-get install -y --allow-unauthenticated libgdiplus

ENV PATH=$PATH:/root/.dotnet/tools

RUN dotnet restore

# Build the project (Release mode)
RUN dotnet publish API/API.csproj -c Release -o /app/out

# --------
# Stage 2: Runtime
# --------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

RUN apt-get update && apt-get install -y --allow-unauthenticated libgdiplus

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5001
ENV redisConnectionString="redis:6379,abortConnect=false"
ENV MINIO_ENDPOINT="minio"
ENV MINIO_PORT=9000
ENV REDIS_HOST="redis"
ENV REDIS_PORT=6379 
ENV Environment="dev"

# Copy the built output from build-env
COPY --from=build-env /app/out .

EXPOSE ${CONTAINER_PORT:-5001}

# Run the app
ENTRYPOINT ["dotnet", "API.dll"]