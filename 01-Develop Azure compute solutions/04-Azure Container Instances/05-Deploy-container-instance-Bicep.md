# Deploy Azure Container Instances using Bicep

## Prerequisites
- Azure CLI installed
- Bicep CLI installed
- Azure subscription

## Steps

### 1. Create a Bicep template file
Create a new file named `container-instance.bicep`:

```bicep
param containerName string = 'mycontainer'
param location string = resourceGroup().location
param image string = 'mcr.microsoft.com/azuredocs/aci-helloworld'

resource containerInstance 'Microsoft.ContainerInstance/containerGroups@2023-05-01' = {
    name: containerName
    location: location
    properties: {
        containers: [
            {
                name: containerName
                properties: {
                    image: image
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

output fqdn string = containerInstance.properties.ipAddress.fqdn
```

### 2. Deploy the Bicep template
```bash
# Create resource group
az group create --name myResourceGroup --location eastus

# Deploy the template
az deployment group create \
    --resource-group myResourceGroup \
    --template-file container-instance.bicep \
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