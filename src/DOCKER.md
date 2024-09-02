
# Docker Setup Instructions

## Prerequisites

- Ensure you have Docker and Docker Compose installed on your machine.
    - [Install Docker](https://www.docker.com/get-started)

## Building and Running the Docker Container

### Step 1: Clone the Repository

If you haven't already, clone the repository and navigate to the project directory:
```bash
git clone https://github.com/kumateck/oryx-backend.git
cd oryx-backend
```

### Step 2: Create a `.env` File

Create a `.env` file in the root directory of your project with the following environment variables:

```plaintext
DB_USERNAME=your_db_username
DB_PASSWORD=your_db_password
ACCESS_KEY=your_access_key
SECRET_KEY=your_secret_key
GOOGLE_API_KEY=your_google_api_key
DEFAULT_PASSWORD=your_default_password
```

### Step 3: Build and Run the Docker Container

Use Docker Compose to build and run the container:
```bash
docker-compose up --build
```

### Step 4: Access the Application

Once the container is running, you can access the application in your web browser at:
```
http://localhost:8080
```

## Dockerfile

The Dockerfile is used to define the container image for the web application. It includes instructions to set up the environment, install dependencies, and start the application.

### Example `Dockerfile`:
```Dockerfile
# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app
COPY . .

RUN dotnet tool install -g dotnet-ef

# Define build arguments
ARG DB_USERNAME
ARG DB_PASSWORD
ARG ACCESS_KEY
ARG SECRET_KEY
ARG GOOGLE_API_KEY
ARG DEFAULT_PASSWORD

# Set environment variables
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV PATH=$PATH:/root/.dotnet/tools
ENV chillSeekDbConnectionString="Host=postgres_db;Port=5432;Username=${DB_USERNAME};Password=${DB_PASSWORD};Database=chillseekdb"
ENV redisConnectionString="redis:6379,abortConnect=false"
ENV RUN_TEST_SEEDS="ON"
ENV MINIO_ENDPOINT="minio"
ENV MINIO_ACCESS_KEY="${ACCESS_KEY}"
ENV MINIO_SECRET_KEY="${SECRET_KEY}"
ENV GOOGLE_API_KEY="${GOOGLE_API_KEY}"
ENV MINIO_PORT=9000
ENV REDIS_HOST="redis"
ENV REDIS_PORT=6379 
ENV DEFAULT_USER_PASSWORD='${DEFAULT_PASSWORD}'

ENTRYPOINT ["dotnet", "watch", "run", "--urls=http://+:5001", "--project", "ChillSeek.API/ChillSeek.API.csproj"]
```

## Docker Compose

The `docker-compose.yml` file is used to manage multi-container applications. It defines the services, networks, and volumes required for the application.

### Example `docker-compose.yml`:
```yaml
services:
  app:
    build:
      context: .
      dockerfile: Dockerfile
    restart: always
    ports:
      - "8080:5001"
    environment:
      - chillSeekDbConnectionString=Host=postgres_db;Port=5432;Username='${DB_USERNAME}';Password='${DB_PASSWORD}';Database='${DB_DATABASE}'
      - redisConnectionString=redis:6379,abortConnect=false
 
    volumes:
      - ~/.aspnet/https:/https:ro
      - .:/app
    networks:
      - sail
    container_name: chill_seek_api

networks:
  sail:
    external: true  # This indicates the network is already created and shared
  shared:
    driver: bridge
```

## .dockerignore

The `.dockerignore` file is used to exclude files and directories from the Docker build context, optimizing the build process.

### Example `.dockerignore`:
```plaintext
# Ignore node_modules directory
node_modules

# Ignore log files
npm-debug.log
yarn-debug.log
yarn-error.log

# Ignore environment variable files
.env
.env.local
.env.development.local
.env.test.local
.env.production.local

# Ignore Docker-related files
Dockerfile
docker-compose.yml

# Ignore build and dependency directories
dist
build
coverage

# Ignore temporary files and directories
*.swp
*.temp
*.tmp
*.bak
*.backup
*.log
*.pid
*.seed
*.pid.lock
*.coverage

# Ignore VCS directories and files
.git
.gitignore
.gitattributes

# Ignore OS generated files
.DS_Store
Thumbs.db

# Ignore IDE and editor-specific files
.idea
.vscode
*.sublime-project
*.sublime-workspace

# Ignore miscellaneous
*.log
*.lock
```

## Troubleshooting

If you encounter any issues while building or running the Docker container, check the Docker logs for more information:
```bash
docker logs <container_id>
```

Ensure all environment variables are correctly set in the Dockerfile and Docker Compose configuration.

## Additional Resources
- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
