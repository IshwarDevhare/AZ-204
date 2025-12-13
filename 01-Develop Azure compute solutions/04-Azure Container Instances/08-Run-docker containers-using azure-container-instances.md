# Run Docker Containers Using Azure Container Instances

## Overview

Azure Container Instances (ACI) provides a fast and simple way to run containers in Azure without managing virtual machines or adopting a higher-level service.

## Prerequisites

- Azure CLI installed
- Docker installed locally (for building images)
- Active Azure subscription

## Creating Container Instances

### Using Azure CLI

```bash
# Create a resource group
az group create --name myResourceGroup --location eastus

# Run a container instance
az container create \
    --resource-group myResourceGroup \
    --name mycontainer \
    --image mcr.microsoft.com/azuredocs/aci-helloworld \
    --dns-name-label aci-demo \
    --ports 80
```

### Using ARM Templates

```json
{
    "type": "Microsoft.ContainerInstance/containerGroups",
    "apiVersion": "2021-03-01",
    "name": "myContainerGroup",
    "location": "[resourceGroup().location]",
    "properties": {
        "containers": [
            {
                "name": "mycontainer",
                "properties": {
                    "image": "nginx:latest",
                    "ports": [
                        {
                            "port": 80,
                            "protocol": "TCP"
                        }
                    ]
                }
            }
        ],
        "osType": "Linux"
    }
}
```

## Container Configuration

### Environment Variables

```bash
az container create \
    --resource-group myResourceGroup \
    --name mycontainer \
    --image myimage:latest \
    --environment-variables 'KEY1=value1' 'KEY2=value2'
```

### Volume Mounting

```bash
az container create \
    --resource-group myResourceGroup \
    --name mycontainer \
    --image myimage:latest \
    --azure-file-volume-share-name myshare \
    --azure-file-volume-account-name mystorageaccount \
    --azure-file-volume-account-key mystoragekey \
    --azure-file-volume-mount-path /mnt/volume
```

## Monitoring and Logs

```bash
# View container logs
az container logs --resource-group myResourceGroup --name mycontainer

# Monitor container events
az container show --resource-group myResourceGroup --name mycontainer
```

## Container Groups

```bash
# Create multi-container group
az container create \
    --resource-group myResourceGroup \
    --name mycontainergroup \
    --image nginx:latest \
    --cpu 1 \
    --memory 1.5
```

## Best Practices

- Use specific image tags instead of `latest`
- Set appropriate CPU and memory limits
- Implement health checks for production workloads
- Use Azure Container Registry for private images
- Configure restart policies based on workload requirements

## Step-by-Step Guide Actual Demo

Follow these steps in sequence to deploy containers using Azure Container Instances:

1. **Login to Azure**
    ```bash
    # Login to your Azure account
    az login
    
    # Set the subscription (if you have multiple)
    az account set --subscription "your-subscription-id"
    ```

2. **Create Resource Group**
    ```bash
    az group create --name mmdemo0101 --location centralindia
    ```

3. **Deploy Container Instance**
    ```bash
    az container create \
         --resource-group mmdemo0101 \
         --name mmdemo0101 \
         --image mcr.microsoft.com/azuredocs/aci-helloworld \
         --dns-name-label mmdemo0101 \
         --ports 80 \
         --os-type Linux \
         --cpu 1 \
         --memory 1 \
         --location centralindia
    ```
    
    > **Important**: The `--dns-name-label` must be globally unique within the Azure region. The DNS namespace is shared across all Azure subscriptions in that region, not just your resource group. Use unique identifiers like timestamps, usernames, or random strings to avoid conflicts.

4. **Verify Deployment**
    ```bash
    # Check container status
    az container show --resource-group myResourceGroup --name mycontainer --query "{FQDN:ipAddress.fqdn,ProvisioningState:provisioningState}" --out table
    ```

5. **Access Your Application**
    - Navigate to the FQDN shown in the previous step
    - Or use: `http://aci-demo.eastus.azurecontainer.io`

6. **Monitor and Troubleshoot** (if needed)
    ```bash
    # View logs
    az container logs --resource-group mmdemo0101 --name mmdemo0101
    
    # Check container events
    az container show --resource-group mmdemo0101 --name mmdemo0101
    ```

7. **Update image, say we need to have database and image needs to updated**
    ```
    $cosomsEndPoint = az cosmosdb create -g mmdemo0101 -n sbdemo0101 --query documentEndPoint --output tsv
    ```

    NOTE: If we got error related to cosmosdb registration then run below command:
    az provider register --namespace Microsoft.DocumentDB

    chck registeration status:

    az provider show --namespace Microsoft.DocumentDB --query "registrationState"

    ```
    $cosmosMasterKey = az cosmosdb keys list -g mmdemo0101 -n mmdemo0101 --query primaryMasterKey --output tsv
    ```

7. **Cleanup Resources**
    ```bash
    # Delete container
    az container delete --resource-group mmdemo0101 --name mmdemo0101
    
    # Delete resource group
    az group delete --name mmdemo0101
    ```

