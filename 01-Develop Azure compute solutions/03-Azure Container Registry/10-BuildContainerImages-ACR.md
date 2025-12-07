# Build Container Images and Deploy to Azure Container Instances

## Overview

This guide demonstrates how to:
1. Build a Docker image locally
2. Push the image to Azure Container Registry (ACR)
3. Deploy the image to Azure Container Instances (ACI)
4. Manage and interact with the running container

## Prerequisites

- Docker Desktop installed and running
- Azure CLI installed
- Azure Container Registry created
- Sufficient permissions (AcrPush, ACI Contributor roles)

## Part 1: Build and Push Image to ACR

### Step 1: Create Dockerfile

Create a Dockerfile using Microsoft's Node.js hello-world sample:

```powershell
# Create project directory
mkdir aci-demo
cd aci-demo

# Create Dockerfile
@"
FROM mcr.microsoft.com/azuredocs/aci-helloworld
"@ | Out-File -FilePath Dockerfile -Encoding utf8
```

**Note:** We use `mcr.microsoft.com/azuredocs/aci-helloworld` which is a Node.js web application that runs on port 80 and displays a welcome page. This is perfect for testing ACI deployments.

### Step 2: Login to Azure Container Registry

```powershell
az acr login --name deployment1
```

### Step 3: Build Docker Image Locally

```powershell
docker build -t deployment1.azurecr.io/sample/aci-helloworld:v1 .
```

### Step 4: Verify Image Locally

```powershell
# List images
docker images | Select-String "aci-helloworld"

# Test run locally (optional)
docker run -d -p 8080:80 --name test-hello deployment1.azurecr.io/sample/aci-helloworld:v1

# Test in browser
Start-Process "http://localhost:8080"

# Stop and remove test container
docker stop test-hello
docker rm test-hello
```

### Step 5: Push Image to ACR

```powershell
docker push deployment1.azurecr.io/sample/aci-helloworld:v1
```

### Step 6: Verify Image in ACR

```powershell
# List repositories
az acr repository list --name deployment1 --output table

# Show tags
az acr repository show-tags --name deployment1 --repository sample/aci-helloworld --output table

# Get image details
az acr repository show --name deployment1 --image sample/aci-helloworld:v1
```

## Part 2: Deploy to Azure Container Instances

### Step 1: Enable Admin User on ACR (If Not Already Enabled)

```powershell
az acr update --name deployment1 --admin-enabled true
```

### Step 2: Get ACR Credentials

```powershell
# Get ACR login server
$acrLoginServer = az acr show --name deployment1 --query loginServer --output tsv

# Get ACR credentials
$acrUsername = az acr credential show --name deployment1 --query username --output tsv
$acrPassword = az acr credential show --name deployment1 --query "passwords[0].value" --output tsv

# Display credentials
Write-Host "Login Server: $acrLoginServer"
Write-Host "Username: $acrUsername"
```

### Step 3: Create Azure Container Instance

**If you get registration error:**

```powershell
az provider register --namespace Microsoft.ContainerInstance
az provider show --namespace Microsoft.ContainerInstance --query "registrationState"
```

```powershell
az container create `
  --resource-group mmdemo0302 `
  --name helloworld-aci `
  --image deployment1.azurecr.io/sample/aci-helloworld:v1 `
  --registry-login-server deployment1.azurecr.io `
  --registry-username $acrUsername `
  --registry-password $acrPassword `
  --dns-name-label helloworld-demo-unique789 `
  --ports 80 `
  --os-type Linux `
  --cpu 1 `
  --memory 1.5
```

**Note:** This Node.js application runs a web server on port 80 and displays an Azure Container Instances welcome page.

**Parameters explained:**
- `--resource-group` - Your resource group name
- `--name` - Name for the container instance
- `--image` - Full image path from ACR
- `--registry-*` - ACR authentication details
- `--dns-name-label` - Must be globally unique
- `--ports` - Ports to expose

### Step 4: Verify Container Instance Creation

```powershell
az container show --resource-group mmdemo0302 --name helloworld-aci --output table
```

### Step 5: Get Container Instance Details and Access

```powershell
# Get FQDN (Fully Qualified Domain Name)
$fqdn = az container show --resource-group mmdemo0302 --name helloworld-aci --query ipAddress.fqdn --output tsv
Write-Host "Container URL: http://$fqdn"

# Get IP address
$ip = az container show --resource-group mmdemo0302 --name helloworld-aci --query ipAddress.ip --output tsv
Write-Host "Container IP: $ip"

# Get container state
az container show --resource-group mmdemo0302 --name helloworld-aci --query containers[0].instanceView.currentState

# Test the web server in browser
Start-Process "http://$fqdn"

# Or test with curl
curl http://$fqdn
```

## Part 3: Interact with Azure Container Instances

### View Container Logs

```powershell
az container logs --resource-group mmdemo0302 --name helloworld-aci
```

### Stream Container Logs

```powershell
az container logs --resource-group mmdemo0302 --name helloworld-aci --follow
```

### Execute Commands in Container

```powershell
# Execute a command
az container exec --resource-group mmdemo0302 --name helloworld-aci --exec-command "/bin/sh"

# Run specific command
az container exec --resource-group mmdemo0302 --name helloworld-aci --exec-command "echo Hello from ACI"
```

### Attach to Running Container

```powershell
az container attach --resource-group mmdemo0302 --name helloworld-aci
```

### Restart Container

```powershell
az container restart --resource-group mmdemo0302 --name helloworld-aci
```

### Stop Container

```powershell
az container stop --resource-group mmdemo0302 --name helloworld-aci
```

### Start Container

```powershell
az container start --resource-group mmdemo0302 --name helloworld-aci
```

## Part 4: Deploy nginx from ACR to ACI

### Step 1: Build and Push nginx Image

```powershell
# Pull nginx from Microsoft Container Registry
docker pull mcr.microsoft.com/oss/nginx/nginx:1.15.5-alpine

# Tag for ACR
docker tag mcr.microsoft.com/oss/nginx/nginx:1.15.5-alpine deployment1.azurecr.io/samples/nginx:latest

# Push to ACR
docker push deployment1.azurecr.io/samples/nginx:latest
```

### Step 2: Create ACI with nginx

```powershell
az container create `
  --resource-group mmdemo0302 `
  --name nginx-aci `
  --image deployment1.azurecr.io/samples/nginx:latest `
  --registry-login-server deployment1.azurecr.io `
  --registry-username $acrUsername `
  --registry-password $acrPassword `
  --dns-name-label nginx-demo-unique123 `
  --ports 80 `
  --cpu 1 `
  --memory 1
```

### Step 3: Access nginx

```powershell
# Get FQDN
$nginxFqdn = az container show --resource-group mmdemo0302 --name nginx-aci --query ipAddress.fqdn --output tsv
Write-Host "nginx URL: http://$nginxFqdn"

# Test with curl
curl http://$nginxFqdn
```

## Part 5: Advanced ACI Configurations

### Deploy with Environment Variables

```powershell
az container create `
  --resource-group mmdemo0302 `
  --name app-with-env `
  --image deployment1.azurecr.io/sample/aci-helloworld:v1 `
  --registry-login-server deployment1.azurecr.io `
  --registry-username $acrUsername `
  --registry-password $acrPassword `
  --environment-variables 'NODE_ENV=production' 'DEBUG=false' `
  --ports 80
```

### Deploy with Volume Mount (Azure File Share)

```powershell
az container create `
  --resource-group mmdemo0302 `
  --name app-with-volume `
  --image deployment1.azurecr.io/sample/aci-helloworld:v1 `
  --registry-login-server deployment1.azurecr.io `
  --registry-username $acrUsername `
  --registry-password $acrPassword `
  --azure-file-volume-account-name mystorageaccount `
  --azure-file-volume-account-key <storage-key> `
  --azure-file-volume-share-name myshare `
  --azure-file-volume-mount-path /mnt/data `
  --ports 80
```

### Deploy with Multiple Containers (Container Group)

```powershell
az container create `
  --resource-group mmdemo0302 `
  --name multi-container-group `
  --image deployment1.azurecr.io/samples/nginx:latest `
  --registry-login-server deployment1.azurecr.io `
  --registry-username $acrUsername `
  --registry-password $acrPassword `
  --ports 80 `
  --file container-group.yaml
```

### Deploy with Managed Identity

```powershell
# Create user-assigned managed identity
az identity create --resource-group mmdemo0302 --name myACIIdentity

# Get identity resource ID
$identityId = az identity show --resource-group mmdemo0302 --name myACIIdentity --query id --output tsv

# Grant identity access to ACR
az role assignment create --assignee $identityId --scope /subscriptions/<sub-id>/resourceGroups/mmdemo0302/providers/Microsoft.ContainerRegistry/registries/deployment1 --role AcrPull

# Create ACI with managed identity
az container create `
  --resource-group mmdemo0302 `
  --name app-with-identity `
  --image deployment1.azurecr.io/sample/aci-helloworld:v1 `
  --acr-identity $identityId `
  --assign-identity $identityId `
  --ports 80
```

## Part 6: Monitor and Manage ACI

### List All Container Instances

```powershell
az container list --resource-group mmdemo0302 --output table
```

### Get Container Metrics

```powershell
# Get CPU usage
az monitor metrics list --resource /subscriptions/<sub-id>/resourceGroups/mmdemo0302/providers/Microsoft.ContainerInstance/containerGroups/helloworld-aci --metric CPUUsage --output table

# Get memory usage
az monitor metrics list --resource /subscriptions/<sub-id>/resourceGroups/mmdemo0302/providers/Microsoft.ContainerInstance/containerGroups/helloworld-aci --metric MemoryUsage --output table
```

### Export Container Instance Definition

```powershell
az container export --resource-group mmdemo0302 --name helloworld-aci --file exported-aci.yaml
```

### Update Container Instance

```powershell
# Update environment variables
az container update --resource-group mmdemo0302 --name helloworld-aci --set containers[0].environmentVariables[0].value="new-value"
```

## Part 7: Cleanup Resources

### Delete Container Instance

```powershell
az container delete --resource-group mmdemo0302 --name helloworld-aci --yes
```

### Delete Multiple Container Instances

```powershell
az container delete --resource-group mmdemo0302 --name helloworld-aci --yes
az container delete --resource-group mmdemo0302 --name nginx-aci --yes
az container delete --resource-group mmdemo0302 --name app-with-env --yes
```

### Delete Images from ACR

```powershell
az acr repository delete --name deployment1 --image sample/aci-helloworld:v1 --yes
```

## Troubleshooting

### Issue: Image Pull Failed

```powershell
# Verify ACR credentials
az acr credential show --name deployment1

# Test ACR login
az acr login --name deployment1

# Verify image exists
az acr repository show --name deployment1 --image sample/aci-helloworld:v1
```

### Issue: Container Creation Failed

```powershell
# Check container events
az container show --resource-group mmdemo0302 --name helloworld-aci --query containers[0].instanceView.events

# View provisioning state
az container show --resource-group mmdemo0302 --name helloworld-aci --query provisioningState
```

### Issue: DNS Name Already Taken

```powershell
# Use a more unique DNS label
az container create `
  --resource-group mmdemo0302 `
  --name helloworld-aci `
  --image deployment1.azurecr.io/sample/aci-helloworld:v1 `
  --registry-login-server deployment1.azurecr.io `
  --registry-username $acrUsername `
  --registry-password $acrPassword `
  --dns-name-label helloworld-$(Get-Random -Maximum 99999) `
  --ports 80
```

### Issue: Container Not Responding

```powershell
# Check container logs
az container logs --resource-group mmdemo0302 --name helloworld-aci

# Check container state
az container show --resource-group mmdemo0302 --name helloworld-aci --query containers[0].instanceView.currentState

# Restart container
az container restart --resource-group mmdemo0302 --name helloworld-aci
```

## Best Practices

1. **Use Managed Identity** instead of admin credentials for production
2. **Enable diagnostic logging** for monitoring and troubleshooting
3. **Use specific image tags** instead of `latest` for reproducibility
4. **Set resource limits** (CPU and memory) appropriately
5. **Use container groups** for multi-container deployments
6. **Implement health probes** for production workloads
7. **Use virtual networks** for secure communication
8. **Tag resources** for better organization and cost management
9. **Enable Azure Monitor** for comprehensive monitoring
10. **Use restart policies** appropriately (Always, OnFailure, Never)

## Cost Optimization

- **Stop containers** when not in use
- **Right-size resources** (CPU and memory)
- **Use appropriate restart policies**
- **Delete unused container instances**
- **Monitor resource usage** regularly

## Additional Resources

- [Azure Container Instances Documentation](https://learn.microsoft.com/en-us/azure/container-instances/)
- [Deploy from Azure Container Registry](https://learn.microsoft.com/en-us/azure/container-instances/container-instances-using-azure-container-registry)
- [Container Groups in ACI](https://learn.microsoft.com/en-us/azure/container-instances/container-instances-container-groups)
- [Troubleshoot ACI](https://learn.microsoft.com/en-us/azure/container-instances/container-instances-troubleshooting)
