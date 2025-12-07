# Utilize ACR Tasks to Build Container Images

## What are ACR Tasks?

**Azure Container Registry Tasks (ACR Tasks)** is a suite of features within Azure Container Registry that provides cloud-based container image building for platforms including Linux, Windows, and ARM.

### Key Benefits:
- No need for Docker Desktop locally
- Build images directly in Azure
- Automated builds on code commit
- Multi-step task workflows
- Scheduled builds

## Prerequisites

- Azure CLI installed
- Azure Container Registry created
- Source code or Dockerfile ready

## Quick Build with ACR Tasks

### Step 1: Create Project Directory

```powershell
# Create project directory
mkdir acr-build-demo
cd acr-build-demo
```

### Step 2: Create Dockerfile

Create a simple Dockerfile based on Microsoft's hello-world image:

```powershell
@"
FROM mcr.microsoft.com/hello-world
"@ | Out-File -FilePath Dockerfile -Encoding utf8
```

### Step 3: Build Image Using ACR Tasks

```powershell
az acr build --registry deployment1 --image hello-world:v1 .
```

**What happens:**
- The build context (current directory) is uploaded to ACR
- ACR creates a container to build the image
- The image is built in Azure
- The image is automatically pushed to the registry

### Step 4: Verify the Build

```powershell
# List repositories
az acr repository list --name deployment1 --output table

# Show tags
az acr repository show-tags --name deployment1 --repository hello-world --output table
```

### Step 5: Run the Image

```powershell
docker pull deployment1.azurecr.io/hello-world:v1
docker run --rm deployment1.azurecr.io/hello-world:v1
```

## Build Custom Hello-World Image

### Step 1: Create Custom Application Files

```powershell
# Create a simple HTML file
@"
<!DOCTYPE html>
<html>
<head><title>ACR Tasks Demo</title></head>
<body>
<h1>Hello from ACR Tasks!</h1>
<p>This image was built using Azure Container Registry Tasks.</p>
</body>
</html>
"@ | Out-File -FilePath index.html -Encoding utf8
```

### Step 2: Create Custom Dockerfile

```powershell
@"
FROM mcr.microsoft.com/hello-world
COPY index.html /usr/share/nginx/html/
"@ | Out-File -FilePath Dockerfile -Encoding utf8
```

### Step 3: Build with Custom Context

```powershell
az acr build --registry deployment1 --image custom-hello:v1 .
```

### Step 4: Build with Build Arguments

```powershell
# Create Dockerfile with build args
@"
FROM mcr.microsoft.com/hello-world
ARG BUILD_DATE
ARG VERSION
LABEL build_date=$BUILD_DATE
LABEL version=$VERSION
"@ | Out-File -FilePath Dockerfile -Encoding utf8

# Build with arguments
az acr build --registry deployment1 `
  --image custom-hello:v2 `
  --build-arg BUILD_DATE=$(Get-Date -Format "yyyy-MM-dd") `
  --build-arg VERSION=2.0 `
  .
```

## Multi-Step Tasks

### Step 1: Create acr-task.yaml

```powershell
@"
version: v1.1.0
steps:
  - build: -t {{.Run.Registry}}/hello-world:{{.Run.ID}} -f Dockerfile .
  - push: 
    - {{.Run.Registry}}/hello-world:{{.Run.ID}}
  - build: -t {{.Run.Registry}}/hello-world:latest -f Dockerfile .
  - push:
    - {{.Run.Registry}}/hello-world:latest
"@ | Out-File -FilePath acr-task.yaml -Encoding utf8
```

### Step 2: Run Multi-Step Task

```powershell
az acr run --registry deployment1 --file acr-task.yaml .
```

## Create Scheduled ACR Task

### Step 1: Create Task for Automated Builds

```powershell
az acr task create `
  --registry deployment1 `
  --name hello-build-task `
  --image hello-world:{{.Run.ID}} `
  --context https://github.com/Azure-Samples/acr-build-helloworld-node.git `
  --file Dockerfile `
  --commit-trigger-enabled false
```

### Step 2: Manually Trigger the Task

```powershell
az acr task run --registry deployment1 --name hello-build-task
```

### Step 3: List All Tasks

```powershell
az acr task list --registry deployment1 --output table
```

### Step 4: View Task Run History

```powershell
az acr task list-runs --registry deployment1 --output table
```

### Step 5: Get Specific Task Run Logs

```powershell
# Get the run ID from previous command
az acr task logs --registry deployment1 --run-id <run-id>
```

## Build from GitHub Repository

### Build Directly from GitHub

```powershell
az acr build --registry deployment1 `
  --image hello-world:{{.Run.ID}} `
  https://github.com/Azure-Samples/acr-build-helloworld-node.git
```

## ACR Task with Timer Trigger

Create a task that runs on a schedule:

```powershell
az acr task create `
  --registry deployment1 `
  --name scheduled-hello-build `
  --image hello-world:{{.Run.ID}} `
  --context https://github.com/Azure-Samples/acr-build-helloworld-node.git `
  --file Dockerfile `
  --schedule "0 2 * * *" `
  --commit-trigger-enabled false
```

Schedule syntax uses cron format:
- `0 2 * * *` - Runs daily at 2:00 AM UTC
- `0 */4 * * *` - Runs every 4 hours
- `0 0 * * 0` - Runs weekly on Sunday at midnight

## Manage ACR Tasks

### Update a Task

```powershell
az acr task update `
  --registry deployment1 `
  --name hello-build-task `
  --image hello-world:latest
```

### Show Task Details

```powershell
az acr task show --registry deployment1 --name hello-build-task
```

### Delete a Task

```powershell
az acr task delete --registry deployment1 --name hello-build-task
```

### Cancel a Running Task

```powershell
az acr task cancel-run --registry deployment1 --run-id <run-id>
```

## ACR Build with Platform Specification

### Build for Specific Platform

```powershell
# Build for Linux
az acr build --registry deployment1 --platform linux --image hello-world:linux .

# Build for Windows
az acr build --registry deployment1 --platform windows --image hello-world:windows .

# Build for ARM
az acr build --registry deployment1 --platform linux/arm64 --image hello-world:arm .
```

## Monitor Build Progress

### Stream Build Logs

```powershell
az acr build --registry deployment1 --image hello-world:v1 . --no-wait
```

### Check Build Status

```powershell
az acr task list-runs --registry deployment1 --output table
```

## Best Practices

1. **Use ACR Tasks instead of local builds** for CI/CD pipelines
2. **Tag images with unique identifiers** like `{{.Run.ID}}`
3. **Use multi-step tasks** for complex build workflows
4. **Enable geo-replication** before using ACR Tasks for global deployments
5. **Set up automated triggers** for Git commits
6. **Use build arguments** for dynamic configuration
7. **Monitor task runs** regularly for failures
8. **Clean up old task runs** to save storage
9. **Use specific base image versions** (avoid `latest`)
10. **Test tasks locally first** before scheduling

## Common ACR Task Variables

- `{{.Run.ID}}` - Unique run identifier
- `{{.Run.Registry}}` - Registry name
- `{{.Run.Date}}` - Run date
- `{{.Values.myValue}}` - Custom value from task definition

## Troubleshooting

### Build Failed

```powershell
# View detailed logs
az acr task logs --registry deployment1 --run-id <run-id>

# Check task definition
az acr task show --registry deployment1 --name hello-build-task
```

### Access Denied

```powershell
# Ensure you have AcrPush role
az role assignment create `
  --assignee <user-email> `
  --role AcrPush `
  --scope /subscriptions/<sub-id>/resourceGroups/<rg>/providers/Microsoft.ContainerRegistry/registries/deployment1
```

### Context Upload Failed

```powershell
# Reduce context size with .dockerignore
@"
.git
.vscode
*.md
node_modules
"@ | Out-File -FilePath .dockerignore -Encoding utf8
```

## Compare: Local Build vs ACR Tasks

| Feature | Local Build | ACR Tasks |
|---------|-------------|-----------|
| **Requires Docker** | Yes | No |
| **Build Location** | Local machine | Azure cloud |
| **CI/CD Integration** | Manual | Automated |
| **Multi-platform** | Requires buildx | Native support |
| **Build logs** | Local only | Stored in Azure |
| **Scheduled builds** | Requires scheduler | Built-in |

## Additional Resources

- [ACR Tasks Overview](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-tasks-overview)
- [ACR Task YAML Reference](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-tasks-reference-yaml)
- [Automate builds with ACR Tasks](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-tutorial-quick-task)



## Actual DEMO
(powershell commmand)
echo "FROM mcr.microsoft.com/hello-world" > Dockerfile


az acr build --image sample/hello-world:v1 --registry deployment1 --file Dockerfile .

((az acr update --name deployment1 --sku Standard))