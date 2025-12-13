# Deploy Multi-Container Group Using YAML

## Overview
Azure Container Instances supports deploying multiple containers in a single container group using YAML configuration files.

## Prerequisites
- Azure CLI installed
- Azure subscription
- Resource group created

## YAML Configuration Structure

### Basic Multi-Container YAML Template
```yaml
apiVersion: 2019-12-01
location: eastus
name: myContainerGroup
properties:
    containers:
    - name: container1
        properties:
            image: nginx:latest
            resources:
                requests:
                    cpu: 1.0
                    memoryInGB: 1.5
            ports:
            - protocol: TCP
                port: 80
    - name: container2
        properties:
            image: alpine:latest
            command: ["/bin/sh", "-c", "sleep 3600"]
            resources:
                requests:
                    cpu: 0.5
                    memoryInGB: 1.0
    osType: Linux
    ipAddress:
        type: Public
        ports:
        - protocol: TCP
            port: 80
tags: {}
type: Microsoft.ContainerInstance/containerGroups
```

## Deployment Methods

## Create resource group

```
az group create --name myResourceGroup --location centralindia
```

### Method 1: Using Azure CLI
```bash
# Deploy container group from YAML file
az container create --resource-group myResourceGroup --file container-group.yaml

# Monitor deployment
az container show --resource-group myResourceGroup --name myContainerGroup --output table
```

### Method 2: Using Azure REST API
```bash
# Deploy using REST API
curl -X PUT \
    'https://management.azure.com/subscriptions/{subscription-id}/resourceGroups/{resource-group}/providers/Microsoft.ContainerInstance/containerGroups/{container-group-name}?api-version=2019-12-01' \
    -H 'Authorization: Bearer {access-token}' \
    -H 'Content-Type: application/json' \
    -d @container-group.yaml
```

## Advanced Configuration Examples

### With Volume Mounts
```yaml
apiVersion: 2019-12-01
location: eastus
name: myContainerGroup
properties:
    containers:
    - name: web-container
        properties:
            image: nginx:latest
            volumeMounts:
            - name: shared-volume
                mountPath: /usr/share/nginx/html
            resources:
                requests:
                    cpu: 1.0
                    memoryInGB: 1.0
    - name: sidecar-container
        properties:
            image: alpine:latest
            volumeMounts:
            - name: shared-volume
                mountPath: /data
    volumes:
    - name: shared-volume
        emptyDir: {}
    osType: Linux
```

### With Environment Variables and Secrets
```yaml
apiVersion: 2019-12-01
location: eastus
name: myContainerGroup
properties:
    containers:
    - name: app-container
        properties:
            image: myapp:latest
            environmentVariables:
            - name: ENV_VAR
                value: "production"
            - name: SECRET_KEY
                secureValue: "your-secret-key"
            resources:
                requests:
                    cpu: 1.0
                    memoryInGB: 1.0
    osType: Linux
```

## Management Commands

### View Container Logs
```bash
# View logs for specific container
az container logs --resource-group myResourceGroup --name myContainerGroup --container-name container1

# Follow logs
az container logs --resource-group myResourceGroup --name myContainerGroup --container-name container1 --follow
```

### Execute Commands
```bash
# Execute command in container
az container exec --resource-group myResourceGroup --name myContainerGroup --container-name container1 --exec-command "/bin/bash"
```

### Clean Up
```bash
# Delete container group
az container delete --resource-group myResourceGroup --name myContainerGroup --yes
```

## Best Practices
- Use resource limits to prevent resource contention
- Implement health checks for production workloads
- Use secrets for sensitive configuration data
- Monitor container logs and metrics
- Plan for data persistence with volume mounts