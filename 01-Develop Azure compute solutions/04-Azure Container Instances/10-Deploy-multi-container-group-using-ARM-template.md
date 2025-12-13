# Deploy Multi-Container Group Using ARM Template

## Overview
Azure Container Instances supports deploying multiple containers onto a single container host using a container group. Container groups are useful when building application sidecars for logging, monitoring, or any other configuration where a service needs a second attached process.

## Prerequisites
- Azure CLI installed
- Azure subscription
- Resource group created

## ARM Template Structure

### Template Parameters
```json
{
    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
        "containerGroupName": {
            "type": "string",
            "defaultValue": "myContainerGroup",
            "metadata": {
                "description": "Name for the container group"
            }
        },
        "location": {
            "type": "string",
            "defaultValue": "[resourceGroup().location]",
            "metadata": {
                "description": "Location for all resources"
            }
        }
    }
}
```

### Multi-Container Template
```json
{
    "resources": [
        {
            "type": "Microsoft.ContainerInstance/containerGroups",
            "apiVersion": "2021-03-01",
            "name": "[parameters('containerGroupName')]",
            "location": "[parameters('location')]",
            "properties": {
                "containers": [
                    {
                        "name": "web-container",
                        "properties": {
                            "image": "nginx:latest",
                            "ports": [
                                {
                                    "port": 80,
                                    "protocol": "TCP"
                                }
                            ],
                            "resources": {
                                "requests": {
                                    "cpu": 1,
                                    "memoryInGB": 1
                                }
                            }
                        }
                    },
                    {
                        "name": "sidecar-container",
                        "properties": {
                            "image": "busybox:latest",
                            "command": ["tail", "-f", "/dev/null"],
                            "resources": {
                                "requests": {
                                    "cpu": 0.5,
                                    "memoryInGB": 0.5
                                }
                            }
                        }
                    }
                ],
                "osType": "Linux",
                "ipAddress": {
                    "type": "Public",
                    "ports": [
                        {
                            "port": 80,
                            "protocol": "TCP"
                        }
                    ]
                },
                "restartPolicy": "Always"
            }
        }
    ],
    "outputs": {
        "fqdn": {
            "type": "string",
            "value": "[reference(resourceId('Microsoft.ContainerInstance/containerGroups', parameters('containerGroupName'))).ipAddress.fqdn]"
        }
    }
}
```

## Deployment Commands

### Deploy using Azure CLI
```bash
# Create resource group
az group create --name myResourceGroup --location eastus

# Deploy template
az deployment group create \
    --resource-group myResourceGroup \
    --template-file multi-container-template.json \
    --parameters containerGroupName=myMultiContainerGroup
```

### Deploy using PowerShell
```powershell
# Create resource group
New-AzResourceGroup -Name "myResourceGroup" -Location "East US"

# Deploy template
New-AzResourceGroupDeployment `
    -ResourceGroupName "myResourceGroup" `
    -TemplateFile "multi-container-template.json" `
    -containerGroupName "myMultiContainerGroup"
```

## Key Configuration Options

### Shared Resources
- **Network**: Containers share the same IP address and port namespace
- **Storage**: Containers can share volumes for data exchange
- **Lifecycle**: All containers start and stop together

### Volume Mounting
```json
"volumes": [
    {
        "name": "shared-volume",
        "emptyDir": {}
    }
],
"volumeMounts": [
    {
        "name": "shared-volume",
        "mountPath": "/shared"
    }
]
```

## Best Practices
- Use container groups for tightly coupled containers
- Ensure containers have compatible resource requirements
- Consider using init containers for setup tasks
- Monitor resource consumption across all containers
- Use shared volumes for inter-container communication

## Verification
```bash
# Check deployment status
az container show --resource-group myResourceGroup --name myMultiContainerGroup

# View logs from specific container
az container logs --resource-group myResourceGroup --name myMultiContainerGroup --container-name web-container
```


## Alternative: Using Bicep Templates

### Bicep Template for Multi-Container Group
```bicep
@description('Name for the container group')
param containerGroupName string = 'myContainerGroup'

@description('Location for all resources')
param location string = resourceGroup().location

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
    name: containerGroupName
    location: location
    properties: {
        containers: [
            {
                name: 'web-container'
                properties: {
                    image: 'nginx:latest'
                    ports: [
                        {
                            port: 80
                            protocol: 'TCP'
                        }
                    ]
                    resources: {
                        requests: {
                            cpu: 1
                            memoryInGB: 1
                        }
                    }
                }
            }
            {
                name: 'sidecar-container'
                properties: {
                    image: 'busybox:latest'
                    command: [
                        'tail'
                        '-f'
                        '/dev/null'
                    ]
                    resources: {
                        requests: {
                            cpu: json('0.5')
                            memoryInGB: json('0.5')
                        }
                    }
                }
            }
        ]
        osType: 'Linux'
        ipAddress: {
            type: 'Public'
            ports: [
                {
                    port: 80
                    protocol: 'TCP'
                }
            ]
        }
        restartPolicy: 'Always'
    }
}

output fqdn string = containerGroup.properties.ipAddress.fqdn
```

### Deploy Bicep Template
```bash
# Deploy directly with Bicep
az deployment group create \
        --resource-group myResourceGroup \
        --template-file multi-container.bicep \
        --parameters containerGroupName=myMultiContainerGroup

# Generate ARM template from Bicep
az bicep build --file multi-container.bicep
```

### Bicep with Shared Volumes
```bicep
resource containerGroupWithVolume 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
    name: '${containerGroupName}-with-volume'
    location: location
    properties: {
        containers: [
            {
                name: 'web-container'
                properties: {
                    image: 'nginx:latest'
                    ports: [
                        {
                            port: 80
                            protocol: 'TCP'
                        }
                    ]
                    volumeMounts: [
                        {
                            name: 'shared-volume'
                            mountPath: '/shared'
                        }
                    ]
                    resources: {
                        requests: {
                            cpu: 1
                            memoryInGB: 1
                        }
                    }
                }
            }
            {
                name: 'sidecar-container'
                properties: {
                    image: 'busybox:latest'
                    command: [
                        'tail'
                        '-f'
                        '/dev/null'
                    ]
                    volumeMounts: [
                        {
                            name: 'shared-volume'
                            mountPath: '/shared'
                        }
                    ]
                    resources: {
                        requests: {
                            cpu: json('0.5')
                            memoryInGB: json('0.5')
                        }
                    }
                }
            }
        ]
        volumes: [
            {
                name: 'shared-volume'
                emptyDir: {}
            }
        ]
        osType: 'Linux'
        ipAddress: {
            type: 'Public'
            ports: [
                {
                    port: 80
                    protocol: 'TCP'
                }
            ]
        }
        restartPolicy: 'Always'
    }
}
```
