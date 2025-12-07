# Create Linux Container App Demo

## Overview

This guide demonstrates how to:
1. Clone a sample Linux application from GitHub
2. Build a Linux container image
3. Push the image to Azure Container Registry
4. Deploy and run the Linux container on Azure Container Instances

## Prerequisites

- Git installed
- Docker Desktop installed and running (with Linux containers enabled)
- Azure CLI installed
- Azure Container Registry created

## Step 1: Clone Sample Application from GitHub

```powershell
# Clone the sample repository
git clone https://github.com/Azure-Samples/aci-helloworld.git

# Navigate to the project directory
cd aci-helloworld
```

**Repository contains:**
- A simple Node.js web application
- Dockerfile for Linux containers
- Ready to deploy to ACI

## Step 2: Explore the Application

```powershell
# View the Dockerfile
Get-Content Dockerfile

# View the application code
Get-Content app.js
```

## Step 3: Login to Azure Container Registry

```powershell
az acr login --name deployment1
```

## Step 4: Build Linux Container Image

```powershell
# Build the Docker image locally
docker build -t deployment1.azurecr.io/linux/aci-helloworld:v1 .

# Verify the image was created
docker images | Select-String "aci-helloworld"
```

## Step 5: Test Container Locally (Optional)

```powershell
# Run container locally
docker run -d -p 8080:80 --name test-linux-app deployment1.azurecr.io/linux/aci-helloworld:v1

# Test in browser
Start-Process "http://localhost:8080"

# View logs
docker logs test-linux-app

# Stop and remove
docker stop test-linux-app
docker rm test-linux-app
```

## Step 6: Push Image to ACR

```powershell
docker push deployment1.azurecr.io/linux/aci-helloworld:v1
```

## Step 7: Verify Image in ACR

```powershell
# List repositories
az acr repository list --name deployment1 --output table

# Show tags for the repository
az acr repository show-tags --name deployment1 --repository linux/aci-helloworld --output table

# Get image details
az acr repository show --name deployment1 --image linux/aci-helloworld:v1
```

## Step 8: Deploy Linux Container to ACI

### Get ACR Credentials

```powershell
$acrUsername = az acr credential show --name deployment1 --query username --output tsv
$acrPassword = az acr credential show --name deployment1 --query "passwords[0].value" --output tsv
```

### Create Linux Container Instance

```powershell
az container create `
  --resource-group mmdemo0302 `
  --name linux-app-aci `
  --image deployment1.azurecr.io/linux/aci-helloworld:v1 `
  --registry-login-server deployment1.azurecr.io `
  --registry-username $acrUsername `
  --registry-password $acrPassword `
  --dns-name-label linux-demo-unique456 `
  --ports 80 `
  --os-type Linux `
  --cpu 1 `
  --memory 1
```

**Parameters:**
- `--os-type Linux` - Specifies Linux container
- `--cpu 1` - 1 CPU core
- `--memory 1` - 1 GB memory

## Step 9: Verify Deployment

```powershell
# Check container status
az container show --resource-group mmdemo0302 --name linux-app-aci --output table

# Get FQDN
$fqdn = az container show --resource-group mmdemo0302 --name linux-app-aci --query ipAddress.fqdn --output tsv
Write-Host "Application URL: http://$fqdn"

# Open in browser
Start-Process "http://$fqdn"
```

## Step 10: View Container Logs

```powershell
# View logs
az container logs --resource-group mmdemo0302 --name linux-app-aci

# Stream logs in real-time
az container logs --resource-group mmdemo0302 --name linux-app-aci --follow
```

## Step 11: Execute Commands in Linux Container

```powershell
# Open shell in container
az container exec --resource-group mmdemo0302 --name linux-app-aci --exec-command "/bin/bash"

# Or run a single command
az container exec --resource-group mmdemo0302 --name linux-app-aci --exec-command "ls -la"
```

## Alternative: Using Azure-Samples Service Fabric Containers

### Clone Service Fabric Sample

```powershell
# Clone repository
git clone https://github.com/Azure-Samples/service-fabric-containers.git

# Navigate to Linux container sample
cd service-fabric-containers\Linux\container-tutorial
```

### Build and Deploy

```powershell
# Build the image
docker build -t deployment1.azurecr.io/linux/voting-app:v1 .

# Push to ACR
docker push deployment1.azurecr.io/linux/voting-app:v1

# Deploy to ACI
az container create `
  --resource-group mmdemo0302 `
  --name voting-app-aci `
  --image deployment1.azurecr.io/linux/voting-app:v1 `
  --registry-login-server deployment1.azurecr.io `
  --registry-username $acrUsername `
  --registry-password $acrPassword `
  --dns-name-label voting-demo-unique789 `
  --ports 80 `
  --os-type Linux
```

## Manage Linux Container Instance

### Restart Container

```powershell
az container restart --resource-group mmdemo0302 --name linux-app-aci
```

### Stop Container

```powershell
az container stop --resource-group mmdemo0302 --name linux-app-aci
```

### Start Container

```powershell
az container start --resource-group mmdemo0302 --name linux-app-aci
```

### View Container Details

```powershell
az container show --resource-group mmdemo0302 --name linux-app-aci
```

## Cleanup

```powershell
# Delete container instance
az container delete --resource-group mmdemo0302 --name linux-app-aci --yes

# Delete image from ACR
az acr repository delete --name deployment1 --repository linux/aci-helloworld --yes
```

## Troubleshooting Linux Containers

### Check Container State

```powershell
az container show --resource-group mmdemo0302 --name linux-app-aci --query containers[0].instanceView.currentState
```

### View Container Events

```powershell
az container show --resource-group mmdemo0302 --name linux-app-aci --query containers[0].instanceView.events
```

### Check Provisioning State

```powershell
az container show --resource-group mmdemo0302 --name linux-app-aci --query provisioningState --output tsv
```

### Verify Docker is Running Linux Containers

```powershell
# Check Docker info
docker info | Select-String "OSType"
```

Should show: `OSType: linux`

## Key Points for Linux Containers

1. **Always specify `--os-type Linux`** when creating ACI
2. **Use Linux-compatible base images** (alpine, ubuntu, debian, etc.)
3. **Linux containers are more common** and have better support
4. **Linux containers are lighter** than Windows containers
5. **Most cloud-native apps** use Linux containers

## Additional Resources

- [Azure Container Instances - Linux containers](https://learn.microsoft.com/en-us/azure/container-instances/container-instances-overview)
- [Deploy container instances - Linux](https://learn.microsoft.com/en-us/azure/container-instances/container-instances-quickstart)
- [Azure-Samples/aci-helloworld](https://github.com/Azure-Samples/aci-helloworld)
