#!/bin/bash

dotnet clean

dotnet ef migrations remove --project ../INFRASTRUCTURE/INFRASTRUCTURE.csproj --startup-project ../API/API.csproj --context INFRASTRUCTURE.Context.ApplicationDbContext
