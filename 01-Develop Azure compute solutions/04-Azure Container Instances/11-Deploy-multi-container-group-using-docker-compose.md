# Deploy Multi-Container Application using Docker Compose

## Overview

This guide demonstrates how to deploy a multi-container voting application using Docker Compose. The application consists of:

- **Frontend (Vote)**: A web application for voting
- **Backend (Worker)**: A service that processes voting requests
- **Redis**: An in-memory data store for caching

You will learn how to:
1. Deploy and run the application locally using Docker Compose with locally built images
2. Push container images to Azure Container Registry (ACR)
3. Deploy the application using images hosted in ACR

---

## Part 1: Local Deployment Using Docker Compose

### Prerequisites

Before you begin, ensure you have the following installed:

- **Docker Desktop** (includes Docker Engine and Docker Compose)
  - Download: https://www.docker.com/products/docker-desktop
  - Verify: Run `docker --version` and `docker-compose --version`
- **Git** (for cloning repositories)
  - Download: https://git-scm.com/download
- **Text editor** or IDE (VS Code, Notepad++, etc.)

### Project Folder Structure

Create the following folder structure for your project:

```
voting-app/
├── docker-compose.yml
├── vote/
│   ├── Dockerfile
│   ├── app.py
│   ├── requirements.txt
│   └── templates/
│       └── (template files)
├── worker/
│   ├── Dockerfile
│   ├── worker.py
│   └── requirements.txt
└── .gitignore
```

### Step 1: Clone the Microsoft Voting App Sample

The Microsoft voting app sample provides the source code for the frontend and worker components.

1. **Clone the repository:**

```bash
git clone https://github.com/Azure-Samples/azure-voting-app-redis.git
cd azure-voting-app-redis
```

2. **Explore the directory structure:**

```bash
ls -la
```

You should see:
- `vote/` - Frontend application code
- `worker/` - Backend worker service code
- `docker-compose.yml` - Existing compose file (optional reference)

### Step 2: Create the Dockerfile for the Frontend (Vote)

Navigate to the `vote/` directory and create or verify the `Dockerfile`:

**File: `vote/Dockerfile`**

```dockerfile
FROM python:3.9-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

EXPOSE 80

CMD ["gunicorn", "--bind", "0.0.0.0:80", "app:app"]
```

### Step 3: Create the Dockerfile for the Worker

Navigate to the `worker/` directory and create or verify the `Dockerfile`:

**File: `worker/Dockerfile`**

```dockerfile
FROM python:3.9-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

CMD ["python", "worker.py"]
```

### Step 4: Build Docker Images Locally

In the project root directory (`azure-voting-app-redis/`), build the Docker images:

```bash
docker build -t voting-app:1.0 ./vote
docker build -t worker-app:1.0 ./worker
```

**Verify the images were created:**

```bash
docker images | grep -E "voting-app|worker-app"
```

Expected output:

```
voting-app    1.0    <image-id>    <time>    <size>
worker-app    1.0    <image-id>    <time>    <size>
```

### Step 5: Create docker-compose.yml for Local Deployment

Create a `docker-compose.yml` file in the project root with the following content:

**File: `docker-compose.yml` (Local Version)**

```yaml
version: '3.8'

services:
  redis:
    image: redis:7-alpine
    container_name: redis-cache
    ports:
      - "6379:6379"
    networks:
      - voting-network
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 5s
      timeout: 3s
      retries: 5

  vote:
    image: voting-app:1.0
    container_name: voting-app
    ports:
      - "8080:80"
    depends_on:
      redis:
        condition: service_healthy
    environment:
      - REDIS_HOST=redis
      - REDIS_PORT=6379
      - FLASK_ENV=production
    networks:
      - voting-network
    restart: unless-stopped

  worker:
    image: worker-app:1.0
    container_name: worker-app
    depends_on:
      redis:
        condition: service_healthy
    environment:
      - REDIS_HOST=redis
      - REDIS_PORT=6379
    networks:
      - voting-network
    restart: unless-stopped

networks:
  voting-network:
    driver: bridge
```

### Step 6: Run the Application Locally

1. **Start all containers:**

```bash
docker-compose up -d
```

**Output:**

```
Creating redis-cache ... done
Creating voting-app ... done
Creating worker-app ... done
```

2. **Verify containers are running:**

```bash
docker-compose ps
```

**Expected output:**

```
NAME              COMMAND                      STATE           PORTS
redis-cache       "redis-server"               Up (healthy)    6379/tcp
voting-app        "gunicorn --bind 0.0.0..."  Up              0.0.0.0:8080->80/tcp
worker-app        "python worker.py"           Up              
```

3. **View application logs:**

```bash
# View logs for all services
docker-compose logs -f

# View logs for a specific service
docker-compose logs -f vote
docker-compose logs -f worker
docker-compose logs -f redis
```

### Step 7: Verify the Application

#### Test the Frontend Application

1. **Open a browser and navigate to:**

```
http://localhost:8080
```

You should see the voting application interface where you can vote for different options (e.g., "Cats" vs "Dogs").

2. **Check Redis data:**

```bash
docker exec redis-cache redis-cli keys "*"
docker exec redis-cache redis-cli get "vote:total"
```

#### Test Container Communication

1. **Verify vote and worker can communicate:**

```bash
docker exec voting-app ping redis
docker exec worker-app ping redis
```

2. **Check network:**

```bash
docker network inspect voting-network
```

### Step 8: Stop and Remove Containers

To stop the application:

```bash
# Stop all containers
docker-compose stop

# Stop and remove all containers, networks, and volumes
docker-compose down

# Remove containers and volumes (data loss)
docker-compose down -v
```

---

## Part 2: Deploy Using Azure Container Registry (ACR)

### Prerequisites

Before you proceed, ensure you have:

- **Azure CLI** installed
  - Download: https://docs.microsoft.com/cli/azure/install-azure-cli
  - Verify: Run `az --version`
- **An active Azure subscription**
- **Docker Desktop** (from Part 1)
- **Container images built locally** (from Part 1)

### Step 1: Create an Azure Container Registry

1. **Set environment variables:**

```powershell
$resourceGroupName = "myResourceGroup"
$acrName = "myacrname"  # Must be lowercase, 5-50 characters, alphanumeric only
$location = "eastus"
```

2. **Create a resource group:**

```bash
az group create --name $resourceGroupName --location $location
```

3. **Create an Azure Container Registry:**

```bash
az acr create --resource-group $resourceGroupName --name $acrName --sku Basic
```

**Output:**

```json
{
  "adminUserEnabled": false,
  "creationDate": "2024-12-13T10:00:00+00:00",
  "id": "/subscriptions/...",
  "location": "eastus",
  "loginServer": "myacrname.azurecr.io",
  "name": "myacrname",
  "resourceGroup": "myResourceGroup",
  "sku": {
    "name": "Basic",
    "tier": "Basic"
  }
}
```

**Note the `loginServer` value** - you'll need it for the next steps.

### Step 2: Authenticate with Azure Container Registry

1. **Login to ACR using Azure CLI:**

```bash
az acr login --name $acrName
```

**Output:**

```
Login Succeeded
```

2. **Alternatively, login with Docker CLI:**

```bash
az acr credential show --name $acrName --query passwords[0].value --output tsv | docker login $acrName.azurecr.io -u 00000000-0000-0000-0000-000000000000 --password-stdin
```

### Step 3: Tag Docker Images for ACR

Tag the locally built images with the ACR login server:

```powershell
$acrLoginServer = "$acrName.azurecr.io"

docker tag voting-app:1.0 "$acrLoginServer/voting-app:1.0"
docker tag worker-app:1.0 "$acrLoginServer/worker-app:1.0"
```

**Verify the tagged images:**

```bash
docker images | grep $acrLoginServer
```

**Expected output:**

```
myacrname.azurecr.io/voting-app        1.0    <image-id>    <time>    <size>
myacrname.azurecr.io/worker-app        1.0    <image-id>    <time>    <size>
```

### Step 4: Push Images to Azure Container Registry

Push the tagged images to ACR:

```bash
docker push "$acrLoginServer/voting-app:1.0"
docker push "$acrLoginServer/worker-app:1.0"
```

**Output for each push:**

```
The push refers to repository [myacrname.azurecr.io/voting-app]
<layer-id>: Pushed
<layer-id>: Pushed
1.0: digest: sha256:... size: ...
```

### Step 5: Verify Images in ACR

List repositories and images in your ACR:

```bash
# List all repositories
az acr repository list --name $acrName --output table

# List all tags for a repository
az acr repository show-tags --name $acrName --repository voting-app --output table
az acr repository show-tags --name $acrName --repository worker-app --output table
```

**Expected output:**

```
Result
--------
voting-app
worker-app
```

### Step 6: Create docker-compose.yml for ACR Deployment

Create a new `docker-compose-acr.yml` file with ACR image names:

**File: `docker-compose-acr.yml` (ACR Version)**

```yaml
version: '3.8'

services:
  redis:
    image: redis:7-alpine
    container_name: redis-cache-acr
    ports:
      - "6379:6379"
    networks:
      - voting-network-acr
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 5s
      timeout: 3s
      retries: 5

  vote:
    image: myacrname.azurecr.io/voting-app:1.0
    container_name: voting-app-acr
    ports:
      - "8080:80"
    depends_on:
      redis:
        condition: service_healthy
    environment:
      - REDIS_HOST=redis
      - REDIS_PORT=6379
      - FLASK_ENV=production
    networks:
      - voting-network-acr
    restart: unless-stopped

  worker:
    image: myacrname.azurecr.io/worker-app:1.0
    container_name: worker-app-acr
    depends_on:
      redis:
        condition: service_healthy
    environment:
      - REDIS_HOST=redis
      - REDIS_PORT=6379
    networks:
      - voting-network-acr
    restart: unless-stopped

networks:
  voting-network-acr:
    driver: bridge
```

**Important:** Replace `myacrname` with your actual ACR name in all image references.

### Step 7: Pull and Run Images from ACR

1. **Stop the local deployment (if running):**

```bash
docker-compose down
```

2. **Update the docker-compose-acr.yml file with your ACR name:**

Replace all instances of `myacrname` with your actual ACR login server.

3. **Login to Docker with ACR credentials:**

```bash
az acr login --name $acrName
```

4. **Run the application using ACR images:**

```bash
docker-compose -f docker-compose-acr.yml up -d
```

**Output:**

```
Pulling redis (redis:7-alpine)...
Pulling myacrname.azurecr.io/voting-app:1.0 ...
Pulling myacrname.azurecr.io/worker-app:1.0 ...
Creating redis-cache-acr ... done
Creating voting-app-acr ... done
Creating worker-app-acr ... done
```

5. **Verify containers are running:**

```bash
docker-compose -f docker-compose-acr.yml ps
```

6. **Test the application:**

```
http://localhost:8080
```

### Step 8: Manage Images in ACR

#### Delete an Image from ACR

```bash
az acr repository delete --name $acrName --image voting-app:1.0 --yes
```

#### Enable Admin Account (Optional)

```bash
az acr update --name $acrName --admin-enabled true
az acr credential show --name $acrName
```

#### Create Additional Tags

```bash
docker tag "$acrLoginServer/voting-app:1.0" "$acrLoginServer/voting-app:latest"
docker push "$acrLoginServer/voting-app:latest"
```

### Step 9: Clean Up Resources

**Stop the ACR deployment:**

```bash
docker-compose -f docker-compose-acr.yml down
```

**Delete the Azure resource group (removes all resources):**

```bash
az group delete --name $resourceGroupName --yes
```

**Delete Docker images:**

```bash
docker rmi voting-app:1.0 worker-app:1.0
docker rmi "$acrLoginServer/voting-app:1.0" "$acrLoginServer/worker-app:1.0"
```

---

## Docker Compose File Reference

### Key Concepts

#### Services
Each service in `docker-compose.yml` represents a container that will be created and managed by Docker Compose:

```yaml
services:
  service-name:
    image: image:tag
    container_name: container-name
    ports:
      - "host-port:container-port"
    environment:
      - KEY=value
    networks:
      - network-name
```

#### Networks
Docker Compose creates a bridge network by default, allowing containers to communicate using their service names as hostnames:

```yaml
networks:
  voting-network:
    driver: bridge
```

#### Dependencies
Use `depends_on` to control the startup order:

```yaml
depends_on:
  redis:
    condition: service_healthy
```

This ensures Redis is healthy before the dependent service starts.

#### Environment Variables
Pass configuration to containers:

```yaml
environment:
  - REDIS_HOST=redis
  - REDIS_PORT=6379
```

#### Health Checks
Monitor container health:

```yaml
healthcheck:
  test: ["CMD", "redis-cli", "ping"]
  interval: 5s
  timeout: 3s
  retries: 5
```

---

## Common Docker Compose Commands

### Container Management

```bash
# Start containers in the background
docker-compose up -d

# Start containers and view logs
docker-compose up

# List running containers
docker-compose ps

# Stop all containers
docker-compose stop

# Start stopped containers
docker-compose start

# Restart containers
docker-compose restart service-name

# Remove containers and networks
docker-compose down

# Remove containers, networks, and volumes
docker-compose down -v
```

### Debugging

```bash
# View logs for all services
docker-compose logs

# Follow logs for a specific service
docker-compose logs -f vote

# View the last 100 lines
docker-compose logs --tail 100

# Execute a command in a running container
docker-compose exec vote /bin/bash

# Show running processes in a container
docker-compose top vote
```

### Image Management

```bash
# Build or rebuild services
docker-compose build

# Build a specific service
docker-compose build vote

# Build without using cache
docker-compose build --no-cache
```

---

## ACR Image Naming Convention

When pushing images to Azure Container Registry, follow this naming convention:

```
<login-server>/<repository-name>:<tag>
```

**Examples:**

```
myacrname.azurecr.io/voting-app:1.0
myacrname.azurecr.io/voting-app:latest
myacrname.azurecr.io/worker-app:v2.0
myacrname.azurecr.io/worker-app:main
```

**Best Practices:**

- Use semantic versioning for tags (e.g., `1.0.0`, `2.1.3`)
- Use `latest` to reference the most recent stable version
- Include branch names for development builds (e.g., `dev`, `main`)
- Use lowercase letters, numbers, periods, underscores, and hyphens only

---

## Troubleshooting

### Issue: Container fails to start

**Solution:**

```bash
# Check container logs
docker-compose logs vote

# Inspect container details
docker inspect <container-id>

# Verify environment variables
docker-compose config
```

### Issue: Cannot connect to Redis

**Ensure:**
- Redis container is running: `docker-compose ps`
- Service name is correct in environment variables
- Containers are on the same network: `docker network inspect voting-network`

### Issue: Port already in use

**Solution:**

```bash
# Find process using the port
netstat -ano | findstr :8080

# Kill the process (replace PID)
taskkill /PID <PID> /F

# Or use a different port in docker-compose.yml
# Change "8080:80" to "8081:80"
```

### Issue: ACR login fails

**Solution:**

```bash
# Ensure you're logged into Azure
az login

# Verify ACR credentials
az acr credential show --name $acrName

# Re-login to ACR
az acr login --name $acrName
```

### Issue: Images not found in ACR

**Verify:**

```bash
# List all repositories
az acr repository list --name $acrName

# Check if image was pushed
az acr repository show-tags --name $acrName --repository voting-app

# Check ACR login server
az acr show --name $acrName --query loginServer
```

---

## Additional Resources

- **Microsoft Voting App Sample:** https://github.com/Azure-Samples/azure-voting-app-redis
- **Docker Compose Documentation:** https://docs.docker.com/compose/
- **Azure Container Registry Documentation:** https://docs.microsoft.com/azure/container-registry/
- **Docker Best Practices:** https://docs.docker.com/develop/dev-best-practices/
- **Azure CLI Reference:** https://docs.microsoft.com/cli/azure/reference-index

---

## Summary

In this guide, you learned how to:

1. **Deploy a multi-container application locally** using Docker Compose with locally built images
2. **Create and manage an Azure Container Registry** for hosting container images
3. **Push Docker images to ACR** and pull them for deployment
4. **Configure Docker Compose files** for both local and ACR-based deployments
5. **Troubleshoot common issues** with Docker Compose and ACR

You can now use these techniques to deploy multi-container applications in both development and production environments on Azure.
