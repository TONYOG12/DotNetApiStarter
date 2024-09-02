#!/bin/bash

dotnet ef database update --project ./INFRASTRUCTURE/INFRASTRUCTURE.csproj --startup-project ./API/API.csproj --context INFRASTRUCTURE.Context.ApplicationDbContext

