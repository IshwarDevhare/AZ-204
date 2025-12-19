# Deploy .NET Web App Using Azure Pipeline

## Overview

Azure Pipelines automates building and deploying your .NET application to Azure App Service. Each code commit triggers an automated build and deployment pipeline.

## Prerequisites

- Azure DevOps project created
- GitHub or Azure Repos repository with .NET code
- Azure App Service created
- .NET project file (`.csproj`)
- Service connection between DevOps and Azure

## Step 1: Create Service Connection

Connect Azure DevOps to your Azure subscription.

**In Azure DevOps:**
1. Go to Project Settings → Service connections
2. Click "New service connection"
3. Select "Azure Resource Manager"
4. Choose "Service principal (automatic)"
5. Select subscription and resource group
6. Enter service connection name: `AzureServiceConnection`
7. Click "Save"

**Or use Azure CLI:**
```powershell
# Create service principal
az ad sp create-for-rbac --name "AzurePipelinesServicePrincipal" `
  --role contributor `
  --scopes /subscriptions/<subscription-id>
```

---

## Step 2: Create Pipeline YAML File

Create `.github/workflows/azure-pipeline.yml` or `azure-pipelines.yml` in repository root.

**For Azure Repos (Recommended):**
```yaml
# File: azure-pipelines.yml
trigger:
  - main

pool:
  vmImage: 'windows-latest'

variables:
  buildConfiguration: 'Release'
  projectPath: '$(Build.SourcesDirectory)/MyProject.csproj'
  publishPath: '$(Build.ArtifactStagingDirectory)/publish'

stages:
  - stage: Build
    displayName: Build
    jobs:
      - job: BuildJob
        displayName: Build .NET Application
        steps:
          - task: UseDotNet@2
            displayName: Install .NET SDK
            inputs:
              version: '8.0.x'
              packageType: sdk

          - task: DotNetCoreCLI@2
            displayName: Restore Dependencies
            inputs:
              command: 'restore'
              projects: '$(projectPath)'

          - task: DotNetCoreCLI@2
            displayName: Build Application
            inputs:
              command: 'build'
              projects: '$(projectPath)'
              arguments: '--configuration $(buildConfiguration)'

          - task: DotNetCoreCLI@2
            displayName: Run Tests (Optional)
            inputs:
              command: 'test'
              projects: '$(projectPath)'
            continueOnError: true

          - task: DotNetCoreCLI@2
            displayName: Publish Application
            inputs:
              command: 'publish'
              projects: '$(projectPath)'
              arguments: '--configuration $(buildConfiguration) --output $(publishPath)'

          - task: PublishBuildArtifacts@1
            displayName: Publish Artifacts
            inputs:
              pathToPublish: '$(publishPath)'
              artifactName: 'drop'
              publishLocation: 'Container'

  - stage: Deploy
    displayName: Deploy
    dependsOn: Build
    condition: succeeded()
    jobs:
      - deployment: DeployJob
        displayName: Deploy to App Service
        environment: 'production'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: DownloadBuildArtifacts@1
                  displayName: Download Artifacts
                  inputs:
                    buildType: 'current'
                    downloadType: 'single'
                    artifactName: 'drop'
                    downloadPath: '$(Agent.BuildDirectory)'

                - task: AzureWebApp@1
                  displayName: Deploy to App Service
                  inputs:
                    azureSubscription: 'AzureServiceConnection'
                    appType: 'webAppLinux'
                    appName: 'mmdemo0101'
                    deploymentMethod: 'zipDeploy'
                    package: '$(Agent.BuildDirectory)/drop/**/*.zip'
```

**For GitHub (Using GitHub Actions):**
```yaml
# File: .github/workflows/deploy.yml
name: Build and Deploy to Azure

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --configuration Release --no-restore
      
      - name: Test
        run: dotnet test --configuration Release --no-build --verbosity normal
        continue-on-error: true
      
      - name: Publish
        run: dotnet publish -c Release -o ${{ github.workspace }}/publish
      
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'mmdemo0101'
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: ${{ github.workspace }}/publish
```

---

## Step 3: Configure Pipeline Variables

Set pipeline variables for different environments.

**In Azure DevOps:**
1. Go to Pipelines → Your Pipeline → Edit
2. Click "Variables" button
3. Add variables:

| Variable | Value | Scope |
|----------|-------|-------|
| `buildConfiguration` | `Release` | Pipeline |
| `dotnetVersion` | `8.0.x` | Pipeline |
| `appName` | `mmdemo0101` | Pipeline |
| `resourceGroup` | `mmdemo0101` | Pipeline |

---

## Step 4: Create Pipeline from YAML

**In Azure DevOps:**

1. Go to Pipelines → Create Pipeline
2. Select repository source (Azure Repos or GitHub)
3. Select your repository
4. Choose "Existing Azure Pipelines YAML file"
5. Select branch and file path (`azure-pipelines.yml`)
6. Click "Continue"
7. Review and click "Save and run"

**Pipeline starts automatically:**
- Builds your .NET application
- Runs tests (if configured)
- Publishes artifacts
- Deploys to App Service

---

## Step 5: Monitor Pipeline Execution

**View Pipeline Runs:**
```powershell
# List recent pipeline runs
az pipelines runs list --project <project-name>
```

**In Azure DevOps UI:**
1. Go to Pipelines → Your Pipeline
2. View build/deploy status in real-time
3. Click run to see detailed logs
4. Check Build, Test, and Deployment stages

**Expected Stages:**
- ✓ Build: Compiles .NET code
- ✓ Test: Runs unit tests (optional)
- ✓ Publish: Creates artifact package
- ✓ Deploy: Uploads to App Service

---

## Step 6: Configure Continuous Integration Triggers

**Automatic Deployment:**
Pipeline runs automatically on every push to main branch (default).

**Manual Trigger:**
```powershell
az pipelines run --id <pipeline-id> --project <project-name>
```

**Pull Request Validation:**
Add to `azure-pipelines.yml`:
```yaml
trigger:
  - main
  - develop

pr:
  - main
```

---

## Complete Setup Script

```powershell
# Create Azure resources
az group create -n mmdemo0101 --location centralindia
az appservice plan create -g mmdemo0101 -n mmdemo0101 --sku B1
az webapp create -g mmdemo0101 -p mmdemo0101 -n mmdemo0101 `
  --runtime "DOTNET|8.0"

# Create service principal for pipeline
az ad sp create-for-rbac --name "AzurePipelinesServicePrincipal" `
  --role contributor `
  --scopes /subscriptions/<subscription-id>

# In Azure DevOps: Add service connection with above credentials
# In repository: Add azure-pipelines.yml file
# In Azure DevOps: Create pipeline from YAML file
```

---

## Pipeline Configuration Examples

### Build Only (No Deploy)

```yaml
trigger:
  - main

pool:
  vmImage: 'windows-latest'

steps:
  - task: UseDotNet@2
    inputs:
      version: '8.0.x'

  - task: DotNetCoreCLI@2
    inputs:
      command: 'restore'

  - task: DotNetCoreCLI@2
    inputs:
      command: 'build'
      arguments: '--configuration Release'

  - task: DotNetCoreCLI@2
    inputs:
      command: 'test'
```

### Deploy to Staging Slot First

```yaml
- task: AzureWebApp@1
  displayName: Deploy to Staging Slot
  inputs:
    azureSubscription: 'AzureServiceConnection'
    appName: 'mmdemo0101'
    deploymentSlot: 'staging'
    deploymentMethod: 'zipDeploy'
    package: '$(Agent.BuildDirectory)/drop/**/*.zip'

- task: PowerShell@2
  displayName: Test Staging Deployment
  inputs:
    targetType: 'inline'
    script: |
      $response = Invoke-WebRequest -Uri "https://mmdemo0101-staging.azurewebsites.net" -MaximumRedirection 0
      Write-Host "Response: $($response.StatusCode)"

- task: AzureAppServiceManage@0
  displayName: Swap Slots
  inputs:
    azureSubscription: 'AzureServiceConnection'
    appName: 'mmdemo0101'
    action: 'Swap Slots'
    sourceSlot: 'staging'
    swapWithProduction: true
```

### Run Database Migrations Before Deploy

```yaml
- task: SqlAzureDacpacDeployment@1
  displayName: Run Database Migration
  inputs:
    azureSubscription: 'AzureServiceConnection'
    ServerName: 'myserver.database.windows.net'
    DatabaseName: 'mydb'
    SqlUsername: 'dbadmin'
    SqlPassword: '$(DbPassword)'
    DacpacFile: '$(Build.ArtifactStagingDirectory)/database.dacpac'
```

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| **Pipeline not triggering** | Verify trigger branch in YAML matches repository branch |
| **Build fails with "project not found"** | Check `projectPath` variable points to correct `.csproj` file |
| **Deployment fails "Invalid app name"** | Verify app name exists in Azure; check service connection permissions |
| **"Dotnet not found"** | Ensure `UseDotNet@2` task is before build tasks |
| **Test failures block deploy** | Add `continueOnError: true` to test task |
| **Authentication failed** | Regenerate service connection in Azure DevOps |
| **Timeout during deployment** | Increase timeout: add `timeout: 600` to step |

---

## Best Practices

✅ **Use Separate Pipelines for Build and Deploy**
- Build artifacts once
- Deploy same artifact to multiple environments

✅ **Enable Branch Protection**
- Require successful pipeline run before merge
- Prevents broken code deployment

✅ **Use Deployment Approvals**
```yaml
environment: 'production'
strategy:
  runOnce:
    deploy:
      steps:
        # Requires manual approval before these steps run
```

✅ **Store Secrets in Variable Groups**
- Don't hardcode passwords
- Use Key Vault integration

✅ **Archive Build Artifacts**
```yaml
- task: PublishBuildArtifacts@1
  inputs:
    artifactName: 'drop'
    publishLocation: 'Container'
```

✅ **Monitor Pipeline Health**
- Review failed runs
- Set up notifications
- Track deployment frequency

---

## Comparison: Deployment Methods

| Method | Setup Time | Speed | Control | Automation |
|--------|-----------|-------|---------|-----------|
| **ZIP Upload** | 5 min | Fast | Manual | None |
| **GitHub Sync** | 10 min | Fast | Automatic | Push-triggered |
| **Azure Pipeline** | 20 min | Fastest | Full | Complete CI/CD |

**Use Azure Pipeline when:**
- ✓ Multiple environments (dev, staging, prod)
- ✓ Need automated testing
- ✓ Team collaboration
- ✓ Approval workflows required
- ✓ Complex deployment scenarios

---

## Summary

Azure Pipeline provides:
- Automated build and test on every commit
- Secure artifact storage
- Multi-stage deployments
- Environment approvals
- Complete audit trail
- Rollback capability

**Key steps:**
1. Create service connection
2. Add `azure-pipelines.yml` to repository
3. Create pipeline in Azure DevOps
4. Push code to trigger automatic deployment
