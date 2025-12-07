# Push Images to ACR using Docker CLI - Complete Demo

## Overview

This guide provides an end-to-end workflow for:
1. Creating an Azure Container Registry
2. Pulling images from Microsoft Container Registry
3. Tagging and pushing images to ACR
4. Pulling and running images from ACR

## Step 1: Create Resource Group

```powershell
az group create -n mmdemo0303 --location centralindia
```

## Step 2: Create Azure Container Registry

```powershell
az acr create -g mmdemo0303 -n acrmmdemo0303 --sku Basic
```

## Step 3: Login to ACR

```powershell
az acr login -n acrmmdemo0303
```

## Step 4: Pull nginx Image from Microsoft Container Registry

```powershell
docker pull mcr.microsoft.com/oss/nginx/nginx:1.15.5-alpine
```

## Step 5: Test nginx Image Locally

```powershell
docker run -it --rm -p 8081:80 mcr.microsoft.com/oss/nginx/nginx:1.15.5-alpine
```

Open browser and navigate to `http://localhost:8081`

Press `Ctrl+C` to stop the container.

## Step 6: Tag Image for ACR

```powershell
docker tag mcr.microsoft.com/oss/nginx/nginx:1.15.5-alpine acrmmdemo0303.azurecr.io/samples/nginx
```

## Step 7: Push Image to ACR

```powershell
docker push acrmmdemo0303.azurecr.io/samples/nginx
```

## Step 8: Verify Image in ACR

```powershell
# List all repositories
az acr repository list --name acrmmdemo0303 --output table

# Show tags for nginx repository
az acr repository show-tags --name acrmmdemo0303 --repository samples/nginx --output table
```

## Step 9: Pull Image from ACR

```powershell
docker pull acrmmdemo0303.azurecr.io/samples/nginx
```

## Step 10: Run Container from ACR Image

```powershell
docker run -it --rm -p 8081:80 acrmmdemo0303.azurecr.io/samples/nginx
```

Open browser and navigate to `http://localhost:8081` to verify the image works.

Press `Ctrl+C` to stop the container.

## Cleanup Resources

```powershell
az group delete -n mmdemo0303 --yes
```

## Complete Workflow Summary

| Step | Command | Purpose |
|------|---------|---------|
| 1 | `az group create` | Create resource group |
| 2 | `az acr create` | Create container registry |
| 3 | `az acr login` | Authenticate to ACR |
| 4 | `docker pull` | Pull image from MCR |
| 5 | `docker run` | Test image locally |
| 6 | `docker tag` | Tag image for ACR |
| 7 | `docker push` | Push image to ACR |
| 8 | `az acr repository list` | Verify in ACR |
| 9 | `docker pull` | Pull from ACR |
| 10 | `docker run` | Test ACR image |
| 11 | `az group delete` | Cleanup |

## Key Points

- **Basic SKU** is sufficient for testing and learning
- **Always login** to ACR before pushing images
- **Tag format**: `<registry-name>.azurecr.io/<repository>:<tag>`
- **Use `--rm` flag** to automatically remove containers after stopping
- **Port mapping**: `-p 8081:80` maps host port 8081 to container port 80