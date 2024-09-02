#!/bin/bash

function usage() {
  echo ""
  echo "A helper script for running add migrations."
  echo ""
  echo "usage: dotnet ef migrations add <name>"
  echo ""
  echo "args:"
  echo "    <name>"
  echo "        name of the migrations"
  echo "examples:"
  echo "    ./add-migrations AddInitialMigrations"
  echo "       Creates an initial migration"
}

if [ -z "$1" ]; then
  echo "Please provide the name"
  usage
  exit 1
fi

# Clean the project
dotnet clean

# Generate a SQL script for the current model
MIGRATION_OUTPUT=$(dotnet ef migrations script --idempotent --project ./INFRASTRUCTURE/INFRASTRUCTURE.csproj --startup-project ./API/API.csproj --context INFRASTRUCTURE.Context.ApplicationDbContext)

# Check if there are pending model changes
if [[ "$MIGRATION_OUTPUT" == *"No migrations were applied"* ]]; then
  echo "No pending model changes detected."
else
  echo "Pending model changes detected. Adding new migration..."
  dotnet ef migrations add "$1" --project ./INFRASTRUCTURE/INFRASTRUCTURE.csproj --startup-project ./API/API.csproj --context INFRASTRUCTURE.Context.ApplicationDbContext
fi
