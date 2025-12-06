# Working with OCI Artifacts using Azure Container Registry

## What is OCI?

**OCI (Open Container Initiative)** is a standard for container formats and runtimes. It defines specifications for:
- Container images
- Container runtime
- Distribution of container images and other artifacts

## What is ORAS?

**ORAS (OCI Registry As Storage)** is a tool that allows you to push and pull any type of file (not just Docker images) to/from OCI-compliant registries like Azure Container Registry.

**Use Cases:**
- Store Helm charts
- Store Kubernetes manifests
- Store configuration files
- Store binaries, scripts, or any artifacts
- Store application packages

## Prerequisites

### 1. Install ORAS CLI

**Windows (using winget):**
```powershell
winget install oras
```

**Or download from GitHub:**
```powershell
# Download the latest release
Invoke-WebRequest -Uri "https://github.com/oras-project/oras/releases/download/v1.1.0/oras_1.1.0_windows_amd64.zip" -OutFile "oras.zip"

# Extract
Expand-Archive -Path oras.zip -DestinationPath C:\oras

# Add to PATH (run as Administrator)
[Environment]::SetEnvironmentVariable("Path", $env:Path + ";C:\oras", "Machine")
```

### 2. Verify Installation

```powershell
oras version
```

## Step-by-Step Guide

### Step 1: Login to Azure Container Registry

**Option A: Using Azure CLI**
```powershell
az acr login --name myacr
```

**Option B: Using ORAS with Username/Password**
```powershell
# Get ACR credentials
$creds = az acr credential show --name myacr | ConvertFrom-Json

# Login with ORAS
oras login myacr.azurecr.io --username $creds.username --password $creds.passwords[0].value
```

**Option C: Using Azure AD Token**
```powershell
# Get access token
$token = az acr login --name myacr --expose-token --output tsv --query accessToken

# Login with token
oras login myacr.azurecr.io --username 00000000-0000-0000-0000-000000000000 --password $token
```

### Step 2: Create Sample Files to Push

```powershell
# Create a sample configuration file
Set-Content -Path "config.json" -Value '{"version": "1.0", "environment": "production"}'

# Create a sample text file
Set-Content -Path "readme.txt" -Value "This is a sample artifact stored in ACR"
```

### Step 3: Push Artifacts to ACR

**Push a single file:**
```powershell
oras push myacr.azurecr.io/samples/config:v1 config.json
```

**Push multiple files:**
```powershell
oras push myacr.azurecr.io/samples/myapp:v1 `
  config.json:application/json `
  readme.txt:text/plain
```

**Push with custom media type:**
```powershell
oras push myacr.azurecr.io/samples/helm-chart:v1 `
  chart.tgz:application/vnd.cncf.helm.chart.content.v1.tar+gzip
```

### Step 4: List Artifacts in ACR

**Using Azure CLI:**
```powershell
# List all repositories
az acr repository list --name myacr --output table

# List tags for a repository
az acr repository show-tags --name myacr --repository samples/config --output table
```

**Using ORAS:**
```powershell
oras repo tags myacr.azurecr.io/samples/config
```

### Step 5: Pull Artifacts from ACR

**Pull to current directory:**
```powershell
oras pull myacr.azurecr.io/samples/config:v1
```

**Pull to specific directory:**
```powershell
oras pull myacr.azurecr.io/samples/myapp:v1 --output ./artifacts/
```

### Step 6: Discover Artifact Information

**Show manifest:**
```powershell
oras manifest fetch myacr.azurecr.io/samples/config:v1
```

**Show artifact details:**
```powershell
oras discover myacr.azurecr.io/samples/config:v1
```

### Step 7: Copy Artifacts Between Registries

```powershell
oras copy myacr.azurecr.io/samples/config:v1 myacr2.azurecr.io/samples/config:v1
```

### Step 8: Delete Artifacts

**Delete a specific tag:**
```powershell
az acr repository delete --name myacr --image samples/config:v1
```

**Delete entire repository:**
```powershell
az acr repository delete --name myacr --repository samples/config
```

## Common Scenarios

### Scenario 1: Store Helm Charts

```powershell
# Package Helm chart
helm package ./my-chart

# Push to ACR
oras push myacr.azurecr.io/helm/my-chart:1.0.0 `
  my-chart-1.0.0.tgz:application/vnd.cncf.helm.chart.content.v1.tar+gzip

# Pull and install
oras pull myacr.azurecr.io/helm/my-chart:1.0.0
helm install my-release my-chart-1.0.0.tgz
```

### Scenario 2: Store Application Configuration

```powershell
# Push configuration files
oras push myacr.azurecr.io/configs/app:prod `
  appsettings.json:application/json `
  database.config:application/xml

# Pull on target environment
oras pull myacr.azurecr.io/configs/app:prod --output /app/config/
```

### Scenario 3: Store Build Artifacts

```powershell
# Push build artifacts
oras push myacr.azurecr.io/builds/myapp:1.2.3 `
  app.exe:application/octet-stream `
  app.dll:application/octet-stream `
  README.md:text/markdown

# Pull for deployment
oras pull myacr.azurecr.io/builds/myapp:1.2.3 --output ./deploy/
```

## Best Practices

1. **Use Semantic Versioning** for artifact tags (e.g., `v1.0.0`, `v1.0.1`)
2. **Add Annotations** to provide metadata about artifacts
3. **Use Descriptive Names** for repositories (e.g., `configs/prod`, `helm/charts`)
4. **Implement Access Control** using Azure RBAC
5. **Enable Content Trust** for production environments
6. **Use Service Principals** for automated workflows
7. **Tag artifacts** with environment indicators (e.g., `dev`, `staging`, `prod`)

## Troubleshooting

**Issue: Login Failed**
```powershell
# Ensure ACR admin user is enabled
az acr update --name myacr --admin-enabled true

# Or use Azure AD authentication
az acr login --name myacr
```

**Issue: Push Failed - Unauthorized**
```powershell
# Check if you have push permissions
az role assignment create --assignee <user-email> `
  --role AcrPush `
  --scope /subscriptions/<subscription-id>/resourceGroups/<rg-name>/providers/Microsoft.ContainerRegistry/registries/myacr
```

**Issue: Artifact Not Found**
```powershell
# List all repositories to verify
az acr repository list --name myacr --output table
```

## Additional Resources

- [ORAS Documentation](https://oras.land/)
- [OCI Distribution Spec](https://github.com/opencontainers/distribution-spec)
- [Azure ACR Documentation](https://learn.microsoft.com/en-us/azure/container-registry/)
- [Push and pull OCI artifacts](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-oci-artifacts)


=============================================================================

## DEMO
