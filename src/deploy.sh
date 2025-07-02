#!/bin/bash

# Exit immediately on error
set -e

# Constants
CERT_DIR="./https"
CERT_FILE="$CERT_DIR/aspnetapp.pfx"
CERT_PASSWORD="Password12345"

# Step 1: Create https directory if it doesn't exist
mkdir -p "$CERT_DIR"

# Step 2: Generate a new self-signed dev cert
echo "Generating HTTPS certificate..."
dotnet dev-certs https -ep "$CERT_FILE" -p "$CERT_PASSWORD" --force

# Step 3: Export environment variable (optional if you're injecting via compose)
export ASPNETCORE_Kestrel__Certificates__Default__Password="$CERT_PASSWORD"
export ASPNETCORE_Kestrel__Certificates__Default__Path="/https/aspnetapp.pfx"

# Step 4: Run Docker Compose
echo "Starting Docker containers..."
docker compose up --build -d

echo "✅ Deployment complete. Certificate created, container is up."
