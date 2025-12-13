# Package and Deploy aci-helloworld to Azure Container Instances

## Introduction and Architecture Flow

This guide demonstrates how to package a Node.js application and deploy it to Azure Container Instances using Azure Container Registry.

### Architecture Flow

```
GitHub (aci-helloworld)
    ↓
Clone Repository
    ↓
Create Dockerfile
    ↓
Build Docker Image Locally
    ↓
Test Locally (docker run)
    ↓
Create Azure Resources (RG, ACR)
    ↓
Tag and Push Image to ACR
    ↓
Deploy from ACR to ACI
    ↓
Access Application via Public FQDN
```

### Application Overview

The `aci-helloworld` is a simple Node.js web application that:
- Runs on a specified port (typically port 80)
- Displays a welcome message in the browser
- Serves static web content
- Demonstrates containerization best practices

---

## Prerequisites

Before you begin, ensure you have the following installed and configured:

### 1. Azure Subscription
- An active Azure subscription
- Access to create resources (Resource Groups, Container Registry, Container Instances)
- Verify: Run `az account show`

### 2. Azure CLI
- Download: https://docs.microsoft.com/cli/azure/install-azure-cli
- Verify installation:

```bash
az --version
```

- Login to Azure:

```bash
az login
```

### 3. Docker Desktop
- Download: https://www.docker.com/products/docker-desktop
- Verify installation:

```bash
docker --version
docker run hello-world
```

### 4. Git
- Download: https://git-scm.com/download
- Verify installation:

```bash
git --version
```

### 5. Text Editor or IDE (Optional)
- Visual Studio Code
- Notepad++
- Any text editor for viewing and editing files

---

## Step 1: Clone the aci-helloworld Sample Repository

Clone the official Microsoft sample application from GitHub:

```bash
git clone https://github.com/Azure-Samples/aci-helloworld.git
cd aci-helloworld
```

Explore the repository structure:

```bash
ls -la
```

Expected directory contents:

```
aci-helloworld/
├── Dockerfile
├── app.js
├── package.json
├── package-lock.json
├── public/
│   ├── index.html
│   ├── style.css
│   └── (static files)
├── README.md
└── .dockerignore
```

### Repository Files Overview

#### `package.json`
Defines the Node.js application dependencies and scripts:

```json
{
  "name": "aci-helloworld",
  "version": "1.0.0",
  "description": "Hello World Node.js application",
  "main": "app.js",
  "scripts": {
    "start": "node app.js"
  },
  "dependencies": {
    "express": "^4.16.2"
  }
}
```

#### `app.js`
Main Node.js application file:

```javascript
const express = require('express');
const app = express();
const port = process.env.PORT || 80;

app.use(express.static('public'));

app.listen(port, () => {
  console.log(`Server running at http://localhost:${port}/`);
});
```

---

## Step 2: Create a Dockerfile

A Dockerfile is already included in the repository. If you need to create or modify it, use the following content:

### Dockerfile Structure

Create or verify the `Dockerfile` in the project root:

**File: `Dockerfile`**

```dockerfile
FROM node:14-alpine

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

EXPOSE 80

CMD ["npm", "start"]
```

### Dockerfile Explanation

| Line | Explanation |
|------|-------------|
| `FROM node:14-alpine` | Use official Node.js 14 image based on Alpine Linux (lightweight) |
| `WORKDIR /app` | Set working directory inside container |
| `COPY package*.json ./` | Copy package.json and package-lock.json |
| `RUN npm install` | Install Node.js dependencies |
| `COPY . .` | Copy application files |
| `EXPOSE 80` | Expose port 80 (HTTP) |
| `CMD ["npm", "start"]` | Default command to start the application |

### .dockerignore File

Create or verify `.dockerignore` to exclude unnecessary files:

**File: `.dockerignore`**

```
node_modules
npm-debug.log
.git
.gitignore
README.md
.dockerignore
```

---

## Step 3: Build the Docker Image Locally

### Build the Image

In the project root directory, build the Docker image:

```bash
docker build -t aci-helloworld:1.0 .
```

**Output:**

```
Sending build context to Docker daemon  50.69kB
Step 1/7 : FROM node:14-alpine
 ---> a2fcc0e6f3b5
Step 2/7 : WORKDIR /app
 ---> Running in 1a2b3c4d5e6f
 ---> Removing intermediate container 1a2b3c4d5e6f
 ---> 2b3c4d5e6f7g
...
Successfully built 2b3c4d5e6f7g
Successfully tagged aci-helloworld:1.0
```

### Verify the Image

List Docker images to confirm it was created:

```bash
docker images | grep aci-helloworld
```

**Expected output:**

```
REPOSITORY          TAG       IMAGE ID       CREATED        SIZE
aci-helloworld      1.0       2b3c4d5e6f7g   10 seconds ago 73MB
```

---

## Step 4: Test the Container Image Locally

### Run the Container

Start the container locally to verify it works:

```bash
docker run -d -p 8080:80 --name hello-world-container aci-helloworld:1.0
```

**Command Explanation:**
- `-d`: Run in detached mode (background)
- `-p 8080:80`: Map port 8080 (host) to port 80 (container)
- `--name hello-world-container`: Assign a container name
- `aci-helloworld:1.0`: Image name and tag

### Verify Container is Running

```bash
docker ps | grep hello-world-container
```

**Expected output:**

```
CONTAINER ID   IMAGE                    COMMAND              CREATED        STATUS       PORTS                  NAMES
a1b2c3d4e5f6   aci-helloworld:1.0      "npm start"          5 seconds ago  Up 4 seconds 0.0.0.0:8080->80/tcp   hello-world-container
```

### View Container Logs

```bash
docker logs hello-world-container
```

**Expected output:**

```
Server running at http://localhost:80/
```

### Test the Application

#### Option 1: Using a Web Browser

Open your web browser and navigate to:

```
http://localhost:8080
```

You should see the aci-helloworld welcome page.

#### Option 2: Using curl Command

```bash
curl http://localhost:8080
```

**Expected output:**

```html
<!DOCTYPE html>
<html>
<head>
    <title>ACI Hello World</title>
    <link rel="stylesheet" href="style.css">
</head>
<body>
    <div class="container">
        <h1>Welcome to Azure Container Instances (ACI)</h1>
        <p>This application is running in a container!</p>
    </div>
</body>
</html>
```

### Stop and Remove the Container

Once testing is complete:

```bash
# Stop the container
docker stop hello-world-container

# Remove the container
docker rm hello-world-container
```

---

## Step 5: Create Azure Resources

### Step 5.1: Set Environment Variables

Define variables for your Azure resources:

```powershell
$resourceGroupName = "myResourceGroup"
$location = "eastus"
$acrName = "myacrregistry"
$aciName = "hello-world-aci"
```

**For Bash:**

```bash
export RESOURCE_GROUP_NAME="myResourceGroup"
export LOCATION="eastus"
export ACR_NAME="myacrregistry"
export ACI_NAME="hello-world-aci"
```

### Step 5.2: Create a Resource Group

Create an Azure resource group to contain all resources:

```bash
az group create \
  --name myResourceGroup \
  --location eastus
```

**Output:**

```json
{
  "id": "/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourceGroups/myResourceGroup",
  "location": "eastus",
  "managedBy": null,
  "name": "myResourceGroup",
  "properties": {
    "provisioningState": "Succeeded"
  },
  "tags": {}
}
```

### Step 5.3: Create an Azure Container Registry

Create an Azure Container Registry to store your container image:

```bash
az acr create \
  --resource-group myResourceGroup \
  --name myacrregistry \
  --sku Basic
```

**Output:**

```json
{
  "adminUserEnabled": false,
  "creationDate": "2024-12-13T10:00:00+00:00",
  "id": "/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourceGroups/myResourceGroup/providers/Microsoft.ContainerRegistry/registries/myacrregistry",
  "location": "eastus",
  "loginServer": "myacrregistry.azurecr.io",
  "name": "myacrregistry",
  "resourceGroup": "myResourceGroup",
  "sku": {
    "name": "Basic",
    "tier": "Basic"
  }
}
```

**Important:** Note the `loginServer` value: `myacrregistry.azurecr.io`

### Step 5.4: Enable Admin Account for ACR (Optional)

To push and pull images, enable the admin account:

```bash
az acr update \
  --name myacrregistry \
  --admin-enabled true
```

Retrieve the credentials:

```bash
az acr credential show \
  --name myacrregistry \
  --query passwords[0].value \
  --output tsv
```

---

## Step 6: Tag and Push the Image to Azure Container Registry

### Step 6.1: Authenticate Docker with ACR

Login to your Azure Container Registry:

```bash
az acr login --name myacrregistry
```

**Output:**

```
Login Succeeded
```

### Step 6.2: Tag the Docker Image for ACR

Tag your local image with the ACR login server:

```bash
docker tag aci-helloworld:1.0 myacrregistry.azurecr.io/aci-helloworld:1.0
```

**Verify the tagged image:**

```bash
docker images | grep myacrregistry
```

**Expected output:**

```
REPOSITORY                               TAG   IMAGE ID       CREATED       SIZE
myacrregistry.azurecr.io/aci-helloworld  1.0   2b3c4d5e6f7g   2 minutes ago 73MB
```

### Step 6.3: Push the Image to ACR

Push the tagged image to your Azure Container Registry:

```bash
docker push myacrregistry.azurecr.io/aci-helloworld:1.0
```

**Output:**

```
The push refers to repository [myacrregistry.azurecr.io/aci-helloworld]
a1b2c3d4e5f6: Pushed
b2c3d4e5f6g7: Pushed
...
1.0: digest: sha256:xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx size: yyyy
```

### Step 6.4: Verify Image in ACR

List repositories and images in your ACR:

```bash
# List all repositories
az acr repository list --name myacrregistry --output table

# List all tags for a repository
az acr repository show-tags \
  --name myacrregistry \
  --repository aci-helloworld \
  --output table
```

**Expected output:**

```
Result
--------
aci-helloworld

Catalog
--------
1.0
```

---

## Step 7: Deploy the Image from ACR to Azure Container Instances

### Step 7.1: Get ACR Credentials

Retrieve the login server and credentials needed for ACI:

```bash
# Get login server
az acr show --name myacrregistry --query loginServer --output tsv

# Get admin username
az acr credential show --name myacrregistry --query username --output tsv

# Get admin password
az acr credential show --name myacrregistry --query passwords[0].value --output tsv
```

**Save these values for the next step:**
- **Login Server:** `myacrregistry.azurecr.io`
- **Username:** (typically the registry name)
- **Password:** (the generated password)

### Step 7.2: Deploy to Azure Container Instances

Deploy the container image from ACR to ACI:

```bash
az container create \
  --resource-group myResourceGroup \
  --name hello-world-aci \
  --image myacrregistry.azurecr.io/aci-helloworld:1.0 \
  --cpu 1 \
  --memory 1 \
  --port 80 \
  --dns-name-label hello-world-aci \
  --registry-login-server myacrregistry.azurecr.io \
  --registry-username <username> \
  --registry-password <password>
```

**Command Explanation:**
- `--resource-group`: Resource group containing the resources
- `--name`: Name of the container instance
- `--image`: Full path to the image in ACR
- `--cpu`: Number of CPU cores (0.5 to 4)
- `--memory`: Memory in GB (minimum 1)
- `--port`: Port to expose (80 for HTTP)
- `--dns-name-label`: DNS name label (must be unique)
- `--registry-*`: ACR credentials for authentication

**Output:**

```json
{
  "containers": [
    {
      "environmentVariables": [],
      "image": "myacrregistry.azurecr.io/aci-helloworld:1.0",
      "name": "hello-world-aci",
      "ports": [
        {
          "port": 80,
          "protocol": "TCP"
        }
      ],
      "resources": {
        "limits": {
          "cpu": 1.0,
          "memoryInGb": 1.0
        },
        "requests": {
          "cpu": 1.0,
          "memoryInGb": 1.0
        }
      }
    }
  ],
  "id": "/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourceGroups/myResourceGroup/providers/Microsoft.ContainerInstances/containerGroups/hello-world-aci",
  "identity": null,
  "imageRegistryCredentials": [
    {
      "server": "myacrregistry.azurecr.io",
      "username": "myacrregistry"
    }
  ],
  "instanceView": {
    "events": [],
    "state": "Pending"
  },
  "location": "eastus",
  "name": "hello-world-aci",
  "osType": "Linux",
  "provisioningState": "Creating",
  "resourceGroup": "myResourceGroup",
  "restartPolicy": "Always",
  "state": "Pending",
  "tags": {}
}
```

### Step 7.3: Verify Deployment Status

Check the status of the container instance:

```bash
az container show \
  --resource-group myResourceGroup \
  --name hello-world-aci \
  --query "{State: instanceView.state, IPs: ipAddress.ip, FQDN: ipAddress.fqdn}" \
  --output table
```

Wait for the state to change from `Pending` to `Running` (typically takes 1-2 minutes).

**Expected output when ready:**

```
State    IPs             FQDN
-------  ---------------  ----------------------------------------
Running  20.193.XXX.XXX   hello-world-aci.eastus.azurecontainer.io
```

---

## Step 8: Verify and Access the Deployment

### Step 8.1: Get Container Logs

View the application logs from the container instance:

```bash
az container logs \
  --resource-group myResourceGroup \
  --name hello-world-aci
```

**Expected output:**

```
Server running at http://localhost:80/
```

### Step 8.2: Get the Public FQDN

Retrieve the fully qualified domain name to access the application:

```bash
az container show \
  --resource-group myResourceGroup \
  --name hello-world-aci \
  --query ipAddress.fqdn \
  --output tsv
```

**Output:**

```
hello-world-aci.eastus.azurecontainer.io
```

### Step 8.3: Access the Application

#### Option 1: Using a Web Browser

Open your web browser and navigate to:

```
http://hello-world-aci.eastus.azurecontainer.io
```

You should see the aci-helloworld welcome page.

#### Option 2: Using curl Command

```bash
curl http://hello-world-aci.eastus.azurecontainer.io
```

**Expected output:**

```html
<!DOCTYPE html>
<html>
<head>
    <title>ACI Hello World</title>
    <link rel="stylesheet" href="style.css">
</head>
<body>
    <div class="container">
        <h1>Welcome to Azure Container Instances (ACI)</h1>
        <p>This application is running in a container!</p>
    </div>
</body>
</html>
```

### Step 8.4: View Full Container Details

Get comprehensive information about the container instance:

```bash
az container show \
  --resource-group myResourceGroup \
  --name hello-world-aci
```

**Key information in output:**
- `ipAddress.ip`: Public IP address
- `ipAddress.fqdn`: Fully qualified domain name
- `instanceView.state`: Current state (Running/Stopped)
- `provisioningState`: Provisioning status
- `containers[0].instanceView.currentState.state`: Container state

### Step 8.5: Monitor Container Events

View events related to the container instance:

```bash
az container show \
  --resource-group myResourceGroup \
  --name hello-world-aci \
  --query "instanceView.events" \
  --output table
```

---

## Step 9: Clean Up Azure Resources

### Remove Container Instance

Delete the container instance:

```bash
az container delete \
  --resource-group myResourceGroup \
  --name hello-world-aci \
  --yes
```

### Remove Azure Container Registry

Delete the ACR (this will delete all images in the registry):

```bash
az acr delete \
  --name myacrregistry \
  --yes
```

### Remove Resource Group

Delete the entire resource group and all resources within it:

```bash
az group delete \
  --name myResourceGroup \
  --yes
```

**Output:**

```
Running operation...
```

The resource group will be deleted asynchronously.

### Verify Cleanup

Verify that resources have been deleted:

```bash
# Check if resource group exists
az group exists --name myResourceGroup

# Expected output: false
```

### Optional: Remove Local Docker Image

Remove the Docker image from your local machine:

```bash
# Remove tagged image
docker rmi myacrregistry.azurecr.io/aci-helloworld:1.0

# Remove original image
docker rmi aci-helloworld:1.0
```

---

## Summary of Key Concepts

### Container Image
A lightweight, standalone executable package containing:
- Application code
- Runtime environment
- Dependencies
- Configuration

### Azure Container Registry (ACR)
A managed Docker container registry service that:
- Stores container images securely
- Supports authentication and authorization
- Enables geo-replication for scalability
- Integrates with Azure services

### Azure Container Instances (ACI)
A serverless container orchestration service that:
- Runs containers without managing VMs or Kubernetes
- Supports automatic scaling
- Provides per-second billing
- Enables quick deployment of applications

### Dockerfile
A text file containing instructions to build a Docker image:
- Defines the base image
- Specifies dependencies and configuration
- Includes commands to execute during build
- Sets the default command to run

---

## Troubleshooting Common Issues

### Issue: Image Push Fails

**Error:** `denied: authentication required`

**Solution:**

```bash
# Login again to ACR
az acr login --name myacrregistry

# Or use explicit credentials
az acr login --name myacrregistry --username <username> --password <password>
```

### Issue: Container Fails to Deploy

**Error:** `Failed to pull image from registry`

**Solution:**

1. Verify the image exists in ACR:

```bash
az acr repository list --name myacrregistry
```

2. Check ACR credentials:

```bash
az acr credential show --name myacrregistry
```

3. Verify the image tag is correct in the deployment command

### Issue: Cannot Access Application

**Error:** `Cannot reach http://hello-world-aci.eastus.azurecontainer.io`

**Solution:**

1. Check container status:

```bash
az container show \
  --resource-group myResourceGroup \
  --name hello-world-aci \
  --query "instanceView.state"
```

2. Wait for state to change to `Running`

3. Check logs for errors:

```bash
az container logs \
  --resource-group myResourceGroup \
  --name hello-world-aci
```

4. Verify firewall and network settings allow port 80

### Issue: Local Build Fails

**Error:** `failed to solve with frontend dockerfile`

**Solution:**

```bash
# Verify Dockerfile is in the project root
ls -la Dockerfile

# Try building with verbose output
docker build --progress=plain -t aci-helloworld:1.0 .

# Check Docker daemon is running
docker info
```

### Issue: Authentication Error with ACR

**Error:** `unauthorized: authentication required`

**Solution:**

1. Enable admin account on ACR:

```bash
az acr update --name myacrregistry --admin-enabled true
```

2. Get credentials:

```bash
az acr credential show --name myacrregistry
```

3. Use username and password in deployment command

---

## Best Practices

### Security
- Use managed identities instead of admin credentials when possible
- Enable Azure Container Registry webhook for CI/CD
- Keep container images updated with latest base image patches
- Use private ACR with restricted access

### Performance
- Use lightweight base images (Alpine Linux)
- Minimize image size by removing unnecessary files
- Use .dockerignore to exclude files from build context
- Enable multi-stage builds for smaller final images

### Monitoring and Logging
- Monitor container logs regularly
- Use Azure Monitor for metrics and alerts
- Set up resource health alerts
- Log container startup and errors

### Cost Optimization
- Use lower CPU and memory allocations for non-critical apps
- Stop containers when not in use
- Clean up unused images and registries
- Consider Azure Container Instances for workloads with variable demand

---

## Additional Resources

- **Microsoft aci-helloworld Sample:** https://github.com/Azure-Samples/aci-helloworld
- **Azure Container Instances Documentation:** https://docs.microsoft.com/azure/container-instances/
- **Azure Container Registry Documentation:** https://docs.microsoft.com/azure/container-registry/
- **Docker Documentation:** https://docs.docker.com/
- **Azure CLI Reference:** https://docs.microsoft.com/cli/azure/reference-index

---

## Conclusion

You have successfully:

1. ✓ Cloned the official Microsoft `aci-helloworld` Node.js sample
2. ✓ Created and verified a Dockerfile
3. ✓ Built and tested the Docker image locally
4. ✓ Created Azure resources (Resource Group and ACR)
5. ✓ Tagged and pushed the image to Azure Container Registry
6. ✓ Deployed the image from ACR to Azure Container Instances
7. ✓ Accessed the application via a public FQDN
8. ✓ Verified deployment and monitored logs
9. ✓ Cleaned up Azure resources

The aci-helloworld application is now running in Azure Container Instances and accessible via the public FQDN. This process demonstrates the complete containerization and Azure deployment workflow.
