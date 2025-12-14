# Create and Deploy Custom Container to Azure Web App

## Prerequisites
- Azure CLI installed and configured
- Docker installed locally
- Azure subscription
- Container registry (Azure Container Registry or Docker Hub)

## Step 1: Create a Dockerfile
```dockerfile
FROM node:18-alpine
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
EXPOSE 3000
CMD ["npm", "start"]
```

## Step 2: Build and Test Container Locally
```bash
# Build the image
docker build -t myapp:latest .

# Run locally to test
docker run -p 3000:3000 myapp:latest
```

## Step 3: Create Azure Container Registry
```bash
# Create resource group
az group create --name myResourceGroup --location eastus

# Create container registry
az acr create --resource-group myResourceGroup --name myregistry --sku Basic
```

## Step 4: Push Image to Registry
```bash
# Login to ACR
az acr login --name myregistry

# Tag image
docker tag myapp:latest myregistry.azurecr.io/myapp:latest

# Push image
docker push myregistry.azurecr.io/myapp:latest
```

## Step 5: Create App Service Plan
```bash
az appservice plan create --name myAppServicePlan --resource-group myResourceGroup --is-linux --sku B1
```

## Step 6: Create Web App with Custom Container
```bash
az webapp create --resource-group myResourceGroup --plan myAppServicePlan --name myWebApp --deployment-container-image-name myregistry.azurecr.io/myapp:latest
```

## Step 7: Configure Container Registry Authentication
```bash
# Enable admin user on ACR
az acr update -n myregistry --admin-enabled true

# Get credentials
az acr credential show --name myregistry

# Configure web app to use ACR
az webapp config container set --name myWebApp --resource-group myResourceGroup --docker-custom-image-name myregistry.azurecr.io/myapp:latest --docker-registry-server-url https://myregistry.azurecr.io --docker-registry-server-user myregistry --docker-registry-server-password <password>
```

## Step 8: Configure Application Settings (Optional)
```bash
az webapp config appsettings set --resource-group myResourceGroup --name myWebApp --settings WEBSITES_PORT=3000
```

## Step 9: Verify Deployment
```bash
# Get web app URL
az webapp show --name myWebApp --resource-group myResourceGroup --query defaultHostName --output tsv

# Check deployment status
az webapp log tail --name myWebApp --resource-group myResourceGroup
```

## Step 10: Enable Continuous Deployment (Optional)
```bash
# Configure webhook for automatic deployment
az webapp deployment container config --name myWebApp --resource-group myResourceGroup --enable-cd true
```