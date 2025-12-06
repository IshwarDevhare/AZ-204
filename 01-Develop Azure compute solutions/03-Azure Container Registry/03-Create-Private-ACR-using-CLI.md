# Create Azure Container Registry using Azure CLI

## Create Resource Group

```bash
az login
```

```bash
az group create --name myResourceGroup --location eastus
```

## Create Azure Container Registry

```bash
az acr create --resource-group myResourceGroup --name myacr --sku Basic
```

**SKU Options:** `Basic`, `Standard`, `Premium`

## Login to ACR

```bash
az acr login --name myacr
```

## Push an Image to ACR

```bash
# Tag the image
docker tag myimage:latest myacr.azurecr.io/myimage:v1

# Push the image
docker push myacr.azurecr.io/myimage:v1
```

## List Images in ACR

```bash
az acr repository list --name myacr --output table
```

## Show Image Tags

```bash
az acr repository show-tags --name myacr --repository myimage --output table
```

## Delete ACR (Optional)

```bash
az acr delete --name myacr --resource-group myResourceGroup
```

### Delete Resource Group (deletes all resources)
```bash
az group delete --name myResourceGroup --yes --no-wait
```

## MissingSubscriptionRegistration Error (ACR)

# Error:
The subscription is not registered to use namespace 'Microsoft.ContainerRegistry'.

# Cause:
Your Azure subscription has not enabled the Microsoft.ContainerRegistry resource provider.

# Fix (Run these commands):

# Register ACR resource provider
az provider register --namespace Microsoft.ContainerRegistry

# Verify registration
az provider show --namespace Microsoft.ContainerRegistry --query "registrationState"

# Re-run ACR creation
az acr create --resource-group <rg-name> --name <acr-name> --sku Basic
