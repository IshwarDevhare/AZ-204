# Create and Deploy Container Images to Azure Container Registry

## Prerequisites

- Docker Desktop installed and running
- Azure CLI installed
- Azure Container Registry created
- Sufficient permissions to push images

## Step 1: Login to Azure Container Registry

```powershell
az acr login -n deployment1 --expose-token
```

Replace `deployment1` with your ACR name.

## Step 2: Pull nginx Image from Microsoft Container Registry

```powershell
docker pull mcr.microsoft.com/oss/nginx/nginx:1.15.5-alpine
```

## Step 3: Test nginx Image Locally

```powershell
# Run nginx container
docker run -it --rm -p 8081:80 mcr.microsoft.com/oss/nginx/nginx:1.15.5-alpine
```

Open browser and navigate to `http://localhost:8081` to verify nginx is running.

Press `Ctrl+C` to stop the container.

## Step 4: Tag Image for ACR

```powershell
docker tag mcr.microsoft.com/oss/nginx/nginx:1.15.5-alpine deployment1.azurecr.io/samples/nginx
```

## Step 5: Push Image to ACR

```powershell
docker push deployment1.azurecr.io/samples/nginx
```

## Step 6: Pull Image from ACR

```powershell
docker pull deployment1.azurecr.io/samples/nginx
```

## Step 7: Run Container Using ACR Image

```powershell
docker run -it --rm -p 8081:80 deployment1.azurecr.io/samples/nginx
```

Open browser and navigate to `http://localhost:8081` to verify the image from ACR works correctly.

## Verify Image in ACR

```powershell
# List all repositories
az acr repository list --name deployment1 --output table

# Show tags for nginx repository
az acr repository show-tags --name deployment1 --repository samples/nginx --output table
```