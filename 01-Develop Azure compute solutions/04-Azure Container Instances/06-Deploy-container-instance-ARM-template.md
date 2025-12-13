# Deploy Azure Container Instances using ARM Template

## Prerequisites
- Azure CLI installed
- Azure subscription

## Steps

### 1. Create an ARM template file
Create a new file named `container-instance.json`:

# Deploy Azure Container Instances using ARM Template

## Prerequisites
- Azure CLI installed
- Azure subscription

## Note
While ARM templates work well for deploying Azure Container Instances, consider using **Bicep** as the preferred approach. Bicep provides a more readable syntax, better IntelliSense support, and compiles to ARM templates automatically, making it less error-prone and easier to maintain.

## Steps

### 1. Create an ARM template file
Create a new file named `container-instance.json`:

```json
{
    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
        "containerName": {
            "type": "string",
            "defaultValue": "mycontainer",
            "metadata": {
                "description": "Name of the container instance"
            }
        },
        "location": {
            "type": "string",
            "defaultValue": "[resourceGroup().location]",
            "metadata": {
                "description": "Location for all resources"
            }
        },
        "image": {
            "type": "string",
            "defaultValue": "mcr.microsoft.com/azuredocs/aci-helloworld",
            "metadata": {
                "description": "Container image to deploy"
            }
        }
    },
    "resources": [
        {
            "type": "Microsoft.ContainerInstance/containerGroups",
            "apiVersion": "2023-05-01",
            "name": "[parameters('containerName')]",
            "location": "[parameters('location')]",
            "properties": {
                "containers": [
                    {
                        "name": "[parameters('containerName')]",
                        "properties": {
                            "image": "[parameters('image')]",
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
            "value": "[reference(parameters('containerName')).ipAddress.fqdn]"
        }
    }
}
```

### 2. Deploy the ARM template
```bash
# Create resource group
az group create --name myResourceGroup --location eastus

# Deploy the template
az deployment group create \
    --resource-group myResourceGroup \
    --template-file container-instance.json \
    --parameters containerName=myapp
```

### 3. Verify deployment
```bash
# Check container status
az container show --resource-group myResourceGroup --name myapp

# Get container logs
az container logs --resource-group myResourceGroup --name myapp
```

### 4. Clean up resources
```bash
az group delete --name myResourceGroup --yes --no-wait
```
{
    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
        "containerName": {
            "type": "string",
            "defaultValue": "mycontainer",
            "metadata": {
                "description": "Name of the container instance"
            }
        },
        "location": {
            "type": "string",
            "defaultValue": "[resourceGroup().location]",
            "metadata": {
                "description": "Location for all resources"
            }
        },
        "image": {
            "type": "string",
            "defaultValue": "mcr.microsoft.com/azuredocs/aci-helloworld",
            "metadata": {
                "description": "Container image to deploy"
            }
        }
    },
    "resources": [
        {
            "type": "Microsoft.ContainerInstance/containerGroups",
            "apiVersion": "2023-05-01",
            "name": "[parameters('containerName')]",
            "location": "[parameters('location')]",
            "properties": {
                "containers": [
                    {
                        "name": "[parameters('containerName')]",
                        "properties": {
                            "image": "[parameters('image')]",
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
            "value": "[reference(parameters('containerName')).ipAddress.fqdn]"
        }
    }
}
```

### 2. Deploy the ARM template
```bash
# Create resource group
az group create --name myResourceGroup --location eastus

# Deploy the template
az deployment group create \
    --resource-group myResourceGroup \
    --template-file container-instance.json \
    --parameters containerName=myapp
```

### 3. Verify deployment
```bash
# Check container status
az container show --resource-group myResourceGroup --name myapp

# Get container logs
az container logs --resource-group myResourceGroup --name myapp
```

### 4. Clean up resources
```bash
az group delete --name myResourceGroup --yes --no-wait
```