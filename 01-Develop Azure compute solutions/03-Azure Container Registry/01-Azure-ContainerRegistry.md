# Azure Container Registry (ACR)

## Definition
Azure Container Registry is a managed Docker registry service for storing and managing private container images and related artifacts. It is based on the open-source Docker Registry 2.0.

## Key Uses
- **Store container images** - Private repository for Docker and OCI images
- **CI/CD integration** - Automated image builds and deployments
- **Multi-region deployment** - Geo-replication for low-latency access
- **Security** - Image scanning, RBAC, and private endpoints
- **Azure integration** - Works with AKS, App Service, Container Instances

## Creating Azure Container Registry

### Using Azure CLI

```bash
# Create a resource group
az group create --name myResourceGroup --location eastus

# Create ACR (Basic tier)
az acr create --resource-group myResourceGroup \
  --name myContainerRegistry \
  --sku Basic

# Login to ACR
az acr login --name myContainerRegistry

# Push an image
docker tag myimage:latest myContainerRegistry.azurecr.io/myimage:latest
docker push myContainerRegistry.azurecr.io/myimage:latest

# Pull an image
docker pull myContainerRegistry.azurecr.io/myimage:latest
```

### Using Azure PowerShell

```powershell
# Create a resource group
New-AzResourceGroup -Name myResourceGroup -Location eastus

# Create ACR
New-AzContainerRegistry -ResourceGroupName myResourceGroup `
  -Name myContainerRegistry `
  -Sku Basic

# Get credentials
Get-AzContainerRegistryCredential -ResourceGroupName myResourceGroup `
  -Name myContainerRegistry
```

## SKU Tiers
- **Basic** - Cost-optimized for development
- **Standard** - Production workloads with higher storage
- **Premium** - High-volume scenarios, geo-replication, content trust

## Important Notes
- Registry name must be globally unique
- Name must be 5-50 alphanumeric characters
- Basic tier suitable for learning; Premium for production

====================================================================
## Orchestration systems

ACR integrates with these container orchestration platforms:

- **Azure Kubernetes Service (AKS)** - Deploy containers directly from ACR
- **Azure Container Instances (ACI)** - Run containers without managing servers
- **Azure App Service** - Deploy containerized web apps
- **Azure Red Hat OpenShift** - Enterprise Kubernetes platform
- **Docker Swarm** - Native Docker clustering
- **Kubernetes** - Any Kubernetes cluster can pull from ACR

## Advantages of ACR

- **Managed service** - No need to maintain registry infrastructure
- **Security** - Azure AD integration, RBAC, and encryption at rest
- **Scalability** - Automatic scaling based on demand
- **Geo-replication** - Replicate images across multiple Azure regions (Premium tier)
- **Automated builds** - ACR Tasks for CI/CD workflows
- **Image scanning** - Vulnerability scanning with Microsoft Defender
- **Private networking** - VNet integration and private endpoints
- **Performance** - Close proximity to Azure services reduces latency

## ACR Service Tiers

### Basic
- **Use case**: Development and testing
- **Storage**: 10 GB included
- **Throughput**: Low
- **Features**: Core registry capabilities only

### Standard
- **Use case**: Production workloads
- **Storage**: 100 GB included
- **Throughput**: Medium
- **Features**: Increased storage and throughput

### Premium
- **Use case**: High-volume production scenarios
- **Storage**: 500 GB included
- **Throughput**: High
- **Features**: Geo-replication, content trust, private link, availability zones, ACR Tasks