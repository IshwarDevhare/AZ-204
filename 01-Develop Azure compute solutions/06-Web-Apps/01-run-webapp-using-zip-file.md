# Deploying .NET Web App to Azure App Service Using ZIP File

## Overview

This guide demonstrates how to deploy a .NET web application to Azure App Service using a ZIP package. This is a quick and efficient deployment method without requiring CI/CD pipelines.

## Prerequisites

- Azure CLI installed and configured
- .NET SDK installed (version 8.0+)
- PowerShell (for ZIP creation)
- Active Azure subscription

## Step-by-Step Deployment Guide

### Step 1: Authenticate with Azure

```powershell
az login
```

**Details:**
- Opens browser for interactive login
- Authenticates your Azure account
- Required before any Azure operations

---

### Step 2: Create Resource Group

```powershell
az group create -n mmdemo0101 --location centralindia
```

**Parameters:**
- `-n mmdemo0101`: Resource group name (must be unique within subscription)
- `--location centralindia`: Azure region (change based on your location)

**Important Notes:**
- Resource group organizes all related resources
- All resources created in this group will be grouped together
- Deleting resource group deletes all contained resources

**Common Regions:**
- `eastus`: East US
- `westus`: West US
- `centralindia`: Central India
- `westeurope`: West Europe
- `southeastasia`: Southeast Asia

---

### Step 3: Create App Service Plan

```powershell
az appservice plan create -g mmdemo0101 -n mmdemo0101 --sku F1
```

**Parameters:**
- `-g mmdemo0101`: Resource group name
- `-n mmdemo0101`: App Service Plan name
- `--sku F1`: Pricing tier (Free tier)

**Available SKUs:**
- `F1`: Free tier (1 shared instance, no SLA)
- `B1`: Basic (dedicated instance, manual scaling)
- `S1`: Standard (auto-scaling, staging slots)
- `P1V2`: Premium V2 (higher performance)

**Important:**
- **Free tier (F1)** limitations:
  - Shared compute with other apps
  - Max 60 minutes/day running time
  - No SSL/TLS certificates
  - No staging slots
  - Suitable for development only

- **Basic tier (B1)** recommended for production

---

### Step 4: Create Web App

```powershell
az webapp create -g mmdemo0101 -p mmdemo0101 -n mmdemo0101
```

**Parameters:**
- `-g mmdemo0101`: Resource group
- `-p mmdemo0101`: App Service Plan (must exist)
- `-n mmdemo0101`: Web app name (must be globally unique, becomes `<name>.azurewebsites.net`)

**Important Notes:**
- App name becomes part of public URL
- Must be unique across all Azure (not just your subscription)
- Name can only contain alphanumeric characters and hyphens
- Cannot be changed after creation

---

### Step 5: Create .NET Web Application

```powershell
dotnet new webapp -n mmdemo0101 -f net8.0
```

**Parameters:**
- `-n mmdemo0101`: Project/directory name
- `-f net8.0`: .NET framework version

**What It Creates:**
- ASP.NET Core web application template
- Default pages (Home, Privacy)
- Program.cs with startup configuration
- appsettings.json for configuration
- wwwroot folder for static files

**Alternative Templates:**
```powershell
dotnet new console     # Console app
dotnet new classlib    # Class library
dotnet new api         # Web API
dotnet new mvc         # MVC application
dotnet new blazorserver # Blazor app
```

---

### Step 6: Navigate to Project Directory

```powershell
cd mmdemo0101
```

**Details:**
- Enter the newly created project folder
- All subsequent commands run in this directory

---

### Step 7: Build and Publish Application

```powershell
dotnet publish -o out
```

**Parameters:**
- `-o out`: Output directory for published files

**What It Does:**
- Compiles .NET application
- Resolves all dependencies
- Optimizes for production
- Creates ready-to-run files in `out` folder

**Important Notes:**
- **Release vs Debug**: Add `-c Release` for optimized builds
- **Output includes:**
  - Compiled assemblies (.dll files)
  - Dependencies
  - Runtime configuration
  - Static files

```powershell
# For optimized release build
dotnet publish -c Release -o out
```

---

### Step 8: Navigate to Published Output

```powershell
cd out
```

**Details:**
- Enter the output folder
- Contains all files to be deployed

---

### Step 9: Configure App Service for ZIP Deployment

```powershell
az webapp config appsetting set -g mmdemo0101 -n mmdemo0101 --settings WEBSITE_RUN_FROM_PACKAGE="1"
```

**Parameters:**
- `-g mmdemo0101`: Resource group
- `-n mmdemo0101`: Web app name
- `--settings WEBSITE_RUN_FROM_PACKAGE="1"`: Enable running from ZIP package

**What This Does:**
- Tells App Service to run directly from deployed ZIP
- No unzipping on server (App Service runs package directly)
- Improves startup performance
- Read-only file system in production

**Important:**
- Must be set **before** deploying ZIP
- Reduces deployment size and startup time
- Better for immutable deployments

---

### Step 10: Create ZIP Package

```powershell
Compress-Archive -Path * -DestinationPath mmdemo0101.zip
```

**Parameters:**
- `-Path *`: All files in current directory
- `-DestinationPath mmdemo0101.zip`: Output ZIP filename

**Important Notes:**
- **Run from `out` directory** (where published files are)
- Includes all published application files
- ZIP must be created from the `out` folder, not from project root
- File size affects deployment time

**Alternative - Include Specific Patterns:**
```powershell
# Compress only specific file types
Compress-Archive -Path *.dll, *.json, wwwroot -DestinationPath app.zip

# Exclude certain files
Compress-Archive -Path * -DestinationPath app.zip -Force
```

---

### Step 11: Deploy ZIP to App Service

```powershell
az webapp deployment source config-zip -g mmdemo0101 -n mmdemo0101 --src .\mmdemo0101.zip
```

**Parameters:**
- `-g mmdemo0101`: Resource group
- `-n mmdemo0101`: Web app name
- `--src .\mmdemo0101.zip`: ZIP file path

**What It Does:**
- Uploads ZIP package to App Service
- App Service extracts and runs the application
- May take 1-2 minutes depending on size

**Response:**
```json
{
  "active": true,
  "author": "N/A",
  "deploymentId": "...",
  "endTime": "...",
  "kind": "onedeploy",
  "message": "Created with status 'Succeeded'.",
  "startTime": "...",
  "status": "Succeeded"
}
```

**Important Notes:**
- Status must be "Succeeded"
- If deployment fails, check:
  - ZIP file integrity
  - All dependencies included
  - .NET version compatibility
  - Startup code issues

---

### Step 12: Test the Deployment

Open browser and navigate to:

```
https://mmdemo0101.azurewebsites.net
```

**What to Check:**
- ✓ Page loads successfully
- ✓ No 404 or 500 errors
- ✓ CSS and images load properly
- ✓ Links and navigation work

**If Issues Occur:**
```powershell
# View application logs
az webapp log tail -g mmdemo0101 -n mmdemo0101

# Check deployment logs
az webapp deployment list -g mmdemo0101 -n mmdemo0101

# Enable diagnostic logging
az webapp log config -g mmdemo0101 -n mmdemo0101 --web-server-logging filesystem
```

---

### Step 13: Clean Up Resources (When Done)

```powershell
az group delete -n mmdemo0101
```

**Parameters:**
- `-n mmdemo0101`: Resource group name to delete

**Important Notes:**
- **Destructive operation** - cannot be undone
- Deletes all resources in the group:
  - Web app
  - App Service Plan
  - Any databases or related services
- Avoid accidental deletion with confirmation:

```powershell
# Add --yes flag to skip confirmation (risky!)
az group delete -n mmdemo0101 --yes

# Safe approach - will prompt for confirmation
az group delete -n mmdemo0101
```

---

## Complete Command Sequence

```powershell
# Authentication
az login

# Create infrastructure
az group create -n mmdemo0101 --location centralindia
az appservice plan create -g mmdemo0101 -n mmdemo0101 --sku F1
az webapp create -g mmdemo0101 -p mmdemo0101 -n mmdemo0101

# Create and build application
dotnet new webapp -n mmdemo0101 -f net8.0
cd mmdemo0101
dotnet publish -o out
cd out

# Configure and deploy
az webapp config appsetting set -g mmdemo0101 -n mmdemo0101 --settings WEBSITE_RUN_FROM_PACKAGE="1"
Compress-Archive -Path * -DestinationPath mmdemo0101.zip
az webapp deployment source config-zip -g mmdemo0101 -n mmdemo0101 --src .\mmdemo0101.zip

# Test
# Navigate to: https://mmdemo0101.azurewebsites.net

# Cleanup
az group delete -n mmdemo0101
```

---

## Important Considerations

### Security
- ✓ Always use HTTPS (automatic with App Service)
- ✓ Never store secrets in ZIP or code
- ✓ Use Key Vault for sensitive configuration
- ✓ Enable authentication if needed

### Performance
- ✓ Use Release build for optimized performance
- ✓ Enable compression in App Service
- ✓ Consider staging slots for testing
- ✓ Monitor CPU and memory usage

### Reliability
- ✓ Always test in staging environment first
- ✓ Keep backups of application code
- ✓ Document deployment process
- ✓ Use version control (Git)

### Cost Optimization
- ✓ Use Free/Basic for development
- ✓ Scale up only when needed
- ✓ Delete unused resources
- ✓ Monitor spending regularly

---

## Troubleshooting

### Common Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| **404 Error** | Wrong URL or app not running | Check app name, verify deployment succeeded |
| **500 Error** | Runtime error in application | Check logs with `az webapp log tail` |
| **ZIP not found** | Wrong file path | Verify ZIP location with `dir` or `ls` |
| **Deployment timeout** | Large ZIP file or slow upload | Check file size, try smaller ZIP |
| **App won't start** | .NET version mismatch | Verify .NET version in project file |
| **File permission errors** | WEBSITE_RUN_FROM_PACKAGE not set | Reconfigure with Step 9 commands |

### Enable Logging

```powershell
# Enable file system logging
az webapp log config -g mmdemo0101 -n mmdemo0101 --web-server-logging filesystem

# View real-time logs
az webapp log tail -g mmdemo0101 -n mmdemo0101

# Enable Application Insights (advanced)
az monitor app-insights component create -g mmdemo0101 -a myappinsights
```

---

## Summary

This deployment method is ideal for:
- ✓ Quick deployments during development
- ✓ One-time application uploads
- ✓ Simple applications without CI/CD
- ✓ Testing and proof-of-concept projects

For production applications, consider:
- Azure Pipelines for automated CI/CD
- GitHub Actions for continuous deployment
- Infrastructure as Code (Bicep/Terraform)
- Application Insights for monitoring
