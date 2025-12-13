# Deploy Container Instance using Azure CLI

## Prerequisites
- Azure CLI installed and configured
- Valid Azure subscription
- Resource group created

## Steps

### 1. Login to Azure
```bash
az login
```

### 2. Create a Resource Group (if needed)
```bash
az group create --name myResourceGroup --location eastus
```

### 3. Deploy Container Instance
```bash
az container create \
    --resource-group myResourceGroup \
    --name mycontainer \
    --image mcr.microsoft.com/azuredocs/aci-helloworld \
    --dns-name-label aci-demo \
    --ports 80
```

### 4. Verify Deployment
```bash
az container show --resource-group myResourceGroup --name mycontainer --query "{FQDN:ipAddress.fqdn,ProvisioningState:provisioningState}" --out table
```

### 5. View Container Logs
```bash
az container logs --resource-group myResourceGroup --name mycontainer
```

### 6. Clean Up Resources
```bash
az container delete --resource-group myResourceGroup --name mycontainer --yes
```

## Additional Options
- Use `--environment-variables` for environment variables
- Use `--secure-environment-variables` for sensitive data
- Use `--restart-policy` to control restart behavior
- Use `--cpu` and `--memory` to specify resource requirements


# Actual Demo #
## Demo Steps

### Step 1: Create Resource Group
```bash
az group create --name mmdemo0101 --location centralindia
```

### Step 2: Deploy Container Instance
```bash
az container create \
    --resource-group mmdemo0101 \
    --name mmdemo0101 \
    --image mcr.microsoft.com/azuredocs/aci-helloworld \
    --dns-name-label mmdemo0101 \
    --ports 80 \
    --os-type Linux \
    --cpu 1 \
    --memory 1
```

### Step 3: Verify Deployment Status
```bash
az container show \
    --resource-group mmdemo0101 \
    --name mmdemo0101 \
    --query "{FQDN:ipAddress.fqdn,ProvisioningState:provisioningState}" \
    --out table
```

### Step 4: Access Application
Once deployed, access the application using the FQDN from the previous command:
`http://mmdemo0101.centralindia.azurecontainer.io`
az group create --name mmdemo0101 --location centralindia

az container create -g mmdemo0101 -n mmdemo0101 --image mcr.microsoft.com/azuredocs/aci-helloworld --dns-name-label mmdemo0101 --ports 80 --os-type Linux --cpu 1 --memory 1

az container show --resource-group mmdemo0101 --name mmdemo0101 --query 
"{FQDN:ipAddress.fqdn,ProvisioningState:provisioningState}" --out table