# Deploy Web App Using ZIP File

## Overview

This guide provides step-by-step instructions to deploy a web application to Azure App Service using a ZIP package. This method is ideal for quick deployments without setting up CI/CD pipelines.

## Prerequisites

- Azure CLI installed and configured
- .NET SDK installed (version 8.0+)
- PowerShell (for ZIP file creation)
- Active Azure subscription
- Source code of your web application

## Deployment Process

### Phase 1: Setup Azure Resources

#### Step 1: Login to Azure

```powershell
az login
```

**Purpose:**
- Authenticate with your Azure account
- Opens browser window for interactive login
- Verifies subscription access

---

#### Step 2: Create Resource Group

```powershell
az group create -n <resource-group-name> --location <region>
```

**Example:**
```powershell
az group create -n mmdemo0101 --location centralindia
```

**Parameters:**
- `-n`: Resource group name (must be unique within subscription)
- `--location`: Azure region for resources

**Important:**
- Resource groups organize related Azure resources
- All resources (web app, storage, databases) live in a resource group
- Deleting a resource group deletes all contained resources
- Choose region closest to your users

**Common Regions:**
- `eastus` - East US
- `westus` - West US
- `centralindia` - Central India
- `westeurope` - West Europe
- `southeastasia` - Southeast Asia

---

#### Step 3: Create App Service Plan

```powershell
az appservice plan create -g <resource-group> -n <plan-name> --sku <sku>
```

**Example:**
```powershell
az appservice plan create -g mmdemo0101 -n mmdemo0101 --sku F1
```

**Parameters:**
- `-g`: Resource group name
- `-n`: App Service Plan name
- `--sku`: Pricing tier

**Available SKUs and Features:**

| SKU | Compute | Cost | Use Case | Scaling |
|-----|---------|------|----------|---------|
| **F1** | Shared | Free | Development | Manual |
| **B1** | Dedicated | Low | Small production | Manual |
| **S1** | Dedicated | Medium | Production | Auto-scale |
| **P1V2** | Dedicated | High | High-traffic | Auto-scale |
| **P2V2** | Dedicated | Very High | Enterprise | Auto-scale |

**Free Tier (F1) Limitations:**
- Shared compute resources
- 60 minutes/day running time
- No SSL/TLS certificates
- No custom domains
- No staging slots
- No SLA

**Recommendation:** Use **B1** or higher for production applications

---

#### Step 4: Create Web App

```powershell
az webapp create -g <resource-group> -p <app-service-plan> -n <app-name>
```

**Example:**
```powershell
az webapp create -g mmdemo0101 -p mmdemo0101 -n mmdemo0101
```

**Parameters:**
- `-g`: Resource group name
- `-p`: App Service Plan name
- `-n`: Web app name (globally unique)

**Important Notes:**
- App name becomes part of public URL: `<app-name>.azurewebsites.net`
- Must be globally unique across all Azure (not just your subscription)
- Can only contain alphanumeric characters and hyphens
- Cannot be changed after creation
- Azure automatically provides HTTPS certificate

---

### Phase 2: Build Application

#### Step 5: Create .NET Web Application (if starting from scratch)

```powershell
dotnet new webapp -n <project-name> -f net8.0
```

**Example:**
```powershell
dotnet new webapp -n mmdemo0101 -f net8.0
```

**Parameters:**
- `-n`: Project/directory name
- `-f`: .NET framework version

**What This Creates:**
- ASP.NET Core web application
- Default Home and Privacy pages
- Program.cs with startup configuration
- appsettings.json for configuration
- wwwroot folder for static files (CSS, JS, images)
- Properties folder with launchSettings.json

**Alternative Templates:**
```powershell
dotnet new console     # Console application
dotnet new classlib    # Class library
dotnet new api         # Web API project
dotnet new mvc         # MVC application
dotnet new blazorserver # Blazor Server application
dotnet new blazorwasm  # Blazor WebAssembly
```

**Note:** Skip this step if you already have an existing application

---

#### Step 6: Navigate to Project Directory

```powershell
cd <project-directory>
```

**Example:**
```powershell
cd mmdemo0101
```

**Purpose:**
- Enter the project folder
- All subsequent commands run in this directory
- Required for build and publish operations

---

#### Step 7: Build and Publish Application

```powershell
dotnet publish -o out
```

**Parameters:**
- `-o out`: Output directory for published files

**What It Does:**
- Compiles your .NET application
- Resolves all NuGet dependencies
- Optimizes code for deployment
- Copies static files
- Creates ready-to-run executable files

**Output Directory Structure:**
```
out/
├── appsettings.json
├── appsettings.Development.json
├── web.config
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
├── [ProjectName].dll (compiled application)
├── [ProjectName].deps.json
├── [ProjectName].runtimeconfig.json
└── [other assemblies]
```

**For Production Builds (Recommended):**
```powershell
dotnet publish -c Release -o out
```

**Parameters:**
- `-c Release`: Build configuration (Release vs Debug)
  - **Release**: Optimized, smaller, faster (recommended for production)
  - **Debug**: Larger, slower, with debugging symbols

**Build Output Includes:**
- Compiled assemblies (.dll files)
- All dependencies
- Configuration files
- Static web files (CSS, JavaScript, images)
- Runtime configuration (.runtimeconfig.json)

---

#### Step 8: Navigate to Published Output

```powershell
cd out
```

**Purpose:**
- Enter the output folder containing published files
- This folder will be compressed into a ZIP
- Contains everything needed to run the application

---

### Phase 3: Deploy to Azure App Service

#### Step 9: Configure App Service for ZIP Deployment

```powershell
az webapp config appsetting set -g <resource-group> -n <app-name> --settings WEBSITE_RUN_FROM_PACKAGE="1"
```

**Example:**
```powershell
az webapp config appsetting set -g mmdemo0101 -n mmdemo0101 --settings WEBSITE_RUN_FROM_PACKAGE="1"
```

**Parameters:**
- `-g`: Resource group name
- `-n`: Web app name
- `--settings WEBSITE_RUN_FROM_PACKAGE="1"`: Enable running from ZIP

**What This Does:**
- Configures App Service to run directly from the deployed ZIP
- No unzipping on the server (App Service runs package directly)
- File system becomes read-only
- Improves startup performance and reliability

**Important:**
- Must be set **BEFORE** deploying the ZIP file
- Reduces deployment size and startup time
- Enables immutable deployments (better for production)
- Can be disabled later if needed

**Alternative (without WEBSITE_RUN_FROM_PACKAGE):**
```powershell
# App Service will extract ZIP to file system
# Allows file modifications during runtime
# Not recommended for production
```

---

#### Step 10: Create ZIP Package

```powershell
Compress-Archive -Path * -DestinationPath <zip-filename>.zip
```

**Example:**
```powershell
Compress-Archive -Path * -DestinationPath mmdemo0101.zip
```

**Parameters:**
- `-Path *`: All files in current directory
- `-DestinationPath`: Output ZIP filename

**Critical Notes:**
- **Must run from `out` directory** (step 8)
- **Not** from project root
- Includes all published application files
- File size affects deployment time and bandwidth

**ZIP Contents Example:**
```
mmdemo0101.zip
├── appsettings.json
├── appsettings.Development.json
├── web.config
├── mmdemo0101.dll
├── mmdemo0101.deps.json
├── mmdemo0101.runtimeconfig.json
├── wwwroot/
│   ├── css/style.css
│   ├── js/site.js
│   └── favicon.ico
└── [other files]
```

**Advanced Options:**

**Include Specific Files Only:**
```powershell
Compress-Archive -Path *.dll, *.json, wwwroot -DestinationPath app.zip
```

**Exclude Certain Patterns:**
```powershell
# Create ZIP with all files (includes everything)
Compress-Archive -Path * -DestinationPath app.zip -Force
```

**Check ZIP Size:**
```powershell
(Get-Item mmdemo0101.zip).Length / 1MB  # Size in MB
```

---

#### Step 11: Deploy ZIP to App Service

```powershell
az webapp deploy \
  --resource-group <resource-group> \
  --name <app-name> \
  --src-path <path-to-zip-file> \
  --type zip
```

**Example (Single Line):**
```powershell
az webapp deploy --resource-group mmdemo0101 --name mmdemo0101 --src-path .\mmdemo0101.zip --type zip
```

**Example (Multi-line - Recommended):**
```powershell
az webapp deploy `
  --resource-group mmdemo0101 `
  --name mmdemo0101 `
  --src-path .\mmdemo0101.zip `
  --type zip
```

**Parameters:**
- `--resource-group` or `-g`: Resource group name
- `--name` or `-n`: Web app name
- `--src-path`: Path to ZIP file (absolute or relative)
- `--type`: Deployment type (`zip` for ZIP files)

**Important Notes:**
- This is the **modern, recommended command** by Microsoft
- Replaces the older `az webapp deployment source config-zip` command
- Use **absolute path** or **correct relative path** from where command is executed
- Example with absolute path: `--src-path "C:\path\to\mmdemo0101.zip"`
- Example with relative path: `--src-path .\mmdemo0101.zip` (if in same directory as ZIP)
- Multi-line format uses PowerShell backtick (`) for line continuation

**Other Deployment Types Supported:**
```powershell
--type zip      # ZIP file deployment
--type jar      # Java JAR file
--type war      # Java WAR file
--type static   # Static content
```

**Deployment Process:**
1. Validates ZIP file structure
2. Uploads ZIP file to Azure
3. Extracts ZIP to kudu/wwwroot (unless WEBSITE_RUN_FROM_PACKAGE=1)
4. App Service receives and processes deployment
5. Application restarts (cold start)
6. Application becomes available

**Typical Timeline:**
- Upload: 30 seconds - 5 minutes (depends on ZIP size and internet)
- Extraction: 30 - 60 seconds
- App restart: 10 - 30 seconds

**Expected Response (Success):**
```json
{
  "active": true,
  "author": "N/A",
  "deploymentId": "abc123def456",
  "endTime": "2024-12-14T10:30:00Z",
  "kind": "onedeploy",
  "message": "Created with status 'Succeeded'.",
  "startTime": "2024-12-14T10:25:00Z",
  "status": "Succeeded"
}
```

**Important Points:**
- Status must be **"Succeeded"** for successful deployment
- Response includes deployment metadata and timing
- `deploymentId` is useful for troubleshooting and rollback
- No error in response indicates successful deployment

**If Deployment Fails:**
- **PATH ISSUE**: `--src-path` parameter requires valid path to ZIP file
  ```powershell
  # Check ZIP exists in current directory
  dir .\mmdemo0101.zip
  
  # Or use absolute path
  az webapp deploy `
    --resource-group mmdemo0101 `
    --name mmdemo0101 `
    --src-path "E:\Learn\AZ-204\AZ-204\01-Develop Azure compute solutions\06-Web-Apps\out\mmdemo0101.zip" `
    --type zip
  ```
- **ZIP CORRUPTION**: Verify ZIP integrity
  ```powershell
  Expand-Archive -Path .\mmdemo0101.zip -DestinationPath test-extract -Force
  dir test-extract
  ```
- **MISSING FILES**: Check ZIP contains all application files
  ```powershell
  # Verify ZIP contains essential files
  Expand-Archive -Path .\mmdemo0101.zip -DestinationPath verify-zip -Force
  dir verify-zip
  # Should contain: .dll, .json, web.config, wwwroot, etc.
  ```
- **VERSION MISMATCH**: Verify .NET version in csproj matches runtime
- **PERMISSION DENIED**: Check WEBSITE_RUN_FROM_PACKAGE is properly set (Step 9)

---

### Phase 4: Verify and Test

#### Step 12: Test the Deployed Application

**Navigate to the Web App:**
```
https://<app-name>.azurewebsites.net
```

**Example:**
```
https://mmdemo0101.azurewebsites.net
```

**Verification Checklist:**
- ✓ Page loads successfully (no timeout)
- ✓ No 404 (Page Not Found) errors
- ✓ No 500 (Internal Server Error) errors
- ✓ CSS and images load properly
- ✓ Navigation links work
- ✓ Forms submit correctly (if applicable)
- ✓ Database connections work (if applicable)
- ✓ External API calls succeed (if applicable)

**Common Test Scenarios:**
1. **Test Home Page**: Load default landing page
2. **Test Routing**: Navigate between different pages
3. **Test Forms**: Submit test data if your app has forms
4. **Test API Endpoints**: Call API endpoints if it's an API app
5. **Test Database**: Verify database connections if applicable
6. **Test Authentication**: Login if your app uses auth (if applicable)

**Check Application Logs:**
```powershell
# View real-time logs
az webapp log tail -g mmdemo0101 -n mmdemo0101 --provider Application

# View all logs
az webapp log tail -g mmdemo0101 -n mmdemo0101
```

**View Deployment Details:**
```powershell
# List deployments
az webapp deployment list -g mmdemo0101 -n mmdemo0101

# Get specific deployment info
az webapp deployment show -g mmdemo0101 -n mmdemo0101 -d <deploymentId>
```

---

#### Step 13: Enable Diagnostic Logging (Optional but Recommended)

```powershell
az webapp log config -g <resource-group> -n <app-name> --web-server-logging filesystem
```

**Example:**
```powershell
az webapp log config -g mmdemo0101 -n mmdemo0101 --web-server-logging filesystem
```

**What This Does:**
- Enables application logging
- Logs written to file system
- Accessible via log stream or download

**View Logs in Real-Time:**
```powershell
az webapp log tail -g mmdemo0101 -n mmdemo0101
```

**Enable Application Insights (Advanced):**
```powershell
az monitor app-insights component create -g mmdemo0101 -a myappinsights
```

---

### Phase 5: Cleanup (When Done)

#### Step 14: Delete Resources

```powershell
az group delete -n <resource-group-name>
```

**Example:**
```powershell
az group delete -n mmdemo0101
```

**⚠️ Warning:**
- **DESTRUCTIVE OPERATION** - Cannot be undone
- Deletes ALL resources in the group:
  - Web app
  - App Service Plan
  - Any databases
  - Storage accounts
  - Any other resources

**Safe Deletion:**
```powershell
# With confirmation (recommended)
az group delete -n mmdemo0101

# Without confirmation (use with caution!)
az group delete -n mmdemo0101 --yes
```

---

## Complete Deployment Script

Copy and run this complete sequence:

```powershell
# ============ PHASE 1: SETUP ============
az login
az group create -n mmdemo0101 --location centralindia
az appservice plan create -g mmdemo0101 -n mmdemo0101 --sku F1
az webapp create -g mmdemo0101 -p mmdemo0101 -n mmdemo0101

# ============ PHASE 2: BUILD (if new app) ============
dotnet new webapp -n mmdemo0101 -f net8.0
cd mmdemo0101
dotnet publish -c Release -o out
cd out

# ============ PHASE 3: CONFIGURE & DEPLOY ============
# Step 9: Configure App Service for ZIP deployment
az webapp config appsetting set -g mmdemo0101 -n mmdemo0101 --settings WEBSITE_RUN_FROM_PACKAGE="1"

# Step 10: Create ZIP package
Compress-Archive -Path * -DestinationPath mmdemo0101.zip

# Step 11: Deploy ZIP using modern command (az webapp deploy)
az webapp deploy `
  --resource-group mmdemo0101 `
  --name mmdemo0101 `
  --src-path .\mmdemo0101.zip `
  --type zip

# ============ PHASE 4: VERIFY ============
# Open browser and visit: https://mmdemo0101.azurewebsites.net
# Check if page loads successfully

# View logs if issues occur:
az webapp log tail -g mmdemo0101 -n mmdemo0101

# ============ PHASE 5: CLEANUP (when done) ============
# az group delete -n mmdemo0101
```

---

## Troubleshooting Guide

### Common Issues and Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| **404 Not Found** | URL incorrect or app not running | Verify app name, check deployment succeeded |
| **500 Internal Server Error** | Runtime error in application | Check logs: `az webapp log tail` |
| **ZIP file not found** | Incorrect file path | Verify ZIP exists: `dir .\mmdemo0101.zip` |
| **"Deployment failed" error** | ZIP corruption or missing files | Recreate ZIP from `out` folder |
| **App won't start** | .NET version mismatch | Verify .NET version in project file |
| **"Permission denied" error** | WEBSITE_RUN_FROM_PACKAGE not set | Rerun Step 9 configuration |
| **Timeout during deployment** | Large ZIP file or slow upload | Check ZIP size, use faster internet |
| **"Invalid zip file" error** | ZIP file is corrupted | Delete ZIP and recreate it |

### Diagnostic Commands

**View Application Logs:**
```powershell
az webapp log tail -g mmdemo0101 -n mmdemo0101
```

**Check Deployment History:**
```powershell
az webapp deployment list -g mmdemo0101 -n mmdemo0101
```

**Enable Detailed Logging:**
```powershell
az webapp log config -g mmdemo0101 -n mmdemo0101 --web-server-logging filesystem --detailed-error-messages true
```

**Check App Service Plan:**
```powershell
az appservice plan show -g mmdemo0101 -n mmdemo0101
```

**Check Web App Configuration:**
```powershell
az webapp config show -g mmdemo0101 -n mmdemo0101
```

---

## Best Practices

### Before Deployment

✅ **Test Locally First**
```powershell
dotnet run
```

✅ **Use Release Configuration**
```powershell
dotnet publish -c Release -o out
```

✅ **Verify Dependencies**
- Check all NuGet packages install correctly
- Ensure all required files are included

✅ **Document Configuration**
- Note all settings required in appsettings.json
- Document environment-specific values

### During Deployment

✅ **Verify ZIP Integrity**
```powershell
# Check ZIP file size
(Get-Item mmdemo0101.zip).Length

# Test extraction
Expand-Archive -Path mmdemo0101.zip -DestinationPath test-extract -Force
```

✅ **Monitor Deployment**
- Watch deployment response
- Check for "Succeeded" status
- Note deployment ID for troubleshooting

✅ **Test Immediately After**
- Visit the application URL
- Check for errors
- Verify all features work

### After Deployment

✅ **Monitor Application**
```powershell
# Enable Application Insights
az monitor app-insights component create -g mmdemo0101 -a myappinsights
```

✅ **Setup Alerts**
- Configure error rate alerts
- Monitor response time
- Track availability

✅ **Regular Backups**
- Download application regularly
- Backup database if applicable
- Version control your code

✅ **Keep Logs**
```powershell
# Export logs regularly
az webapp log download -g mmdemo0101 -n mmdemo0101 --log-file app-logs.zip
```

---

## Performance Optimization

### Configuration Settings

**Enable Compression:**
```powershell
az webapp config set -g mmdemo0101 -n mmdemo0101 --web-server-logging filesystem
```

**Configure Static File Caching:**
- Handled automatically by Azure
- Configure in application code if needed

**Always On (Recommended for production):**
- Available in Basic and higher tiers
- Prevents app from unloading
- Reduces cold start time

---

## Security Considerations

✅ **HTTPS Enabled by Default**
- Azure provides free SSL/TLS certificate
- Automatic renewal

✅ **Never Store Secrets**
- Don't put passwords in code
- Use Azure Key Vault instead

✅ **Secure Configuration**
```powershell
# Use Key Vault for secrets
az keyvault secret set --vault-name mykeyvault --name DbPassword --value "secure-password"
```

✅ **Enable Authentication**
- If app is public-facing
- Use Azure AD, Microsoft Account, or custom auth

---

## Summary

**ZIP deployment is ideal for:**
- ✓ Quick development deployments
- ✓ One-time uploads
- ✓ Simple applications
- ✓ Testing and prototyping
- ✓ Small to medium projects

**Consider alternatives for:**
- Continuous deployment (Azure Pipelines)
- Large-scale projects (Kubernetes)
- Serverless functions
- Complex CI/CD workflows

**Key Takeaways:**
- Publish from `out` folder, not project root
- Set WEBSITE_RUN_FROM_PACKAGE before deploying
- Always test after deployment
- Enable logging for troubleshooting
- Delete resource group when done to avoid charges
