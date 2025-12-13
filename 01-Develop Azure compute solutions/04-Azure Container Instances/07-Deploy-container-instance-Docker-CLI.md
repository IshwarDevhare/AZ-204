# Deploy Azure Container Instance using Docker CLI

## ⚠️ DEPRECATED
**Note:** Docker CLI integration with Azure Container Instances has been removed from Docker Desktop. 
Please use Azure CLI instead. See `02-Deploy-container-instance-AZ-CLI.md` for the recommended approach.

## Alternative: Use Azure CLI (Recommended)
```bash
az container create \
    --resource-group mmdemo0101 \
    --name mycontainer \
    --image nginx \
    --dns-name-label mycontainer-demo \
    --ports 80 \
    --os-type Linux \
    --cpu 1 \
    --memory 1
```

For complete instructions, refer to `02-Deploy-container-instance-AZ-CLI.md`.