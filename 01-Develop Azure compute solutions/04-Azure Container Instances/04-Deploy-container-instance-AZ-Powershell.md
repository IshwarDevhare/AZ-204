# Deploy Azure Container Instance using PowerShell

## Prerequisites
- Azure PowerShell module installed
- Azure subscription
- Resource group created

## Steps

### 1. Connect to Azure
```powershell
Connect-AzAccount
Set-AzContext -SubscriptionId "your-subscription-id"
```

### 2. Create Resource Group (if needed)
```powershell
$resourceGroup = "myResourceGroup"
$location = "East US"

New-AzResourceGroup -Name $resourceGroup -Location $location
```

### 3. Deploy Container Instance
```powershell
$containerGroupName = "mycontainergroup"
$containerName = "mycontainer"
$image = "mcr.microsoft.com/azuredocs/aci-helloworld"

New-AzContainerGroup `
    -ResourceGroupName $resourceGroup `
    -Name $containerGroupName `
    -Image $image `
    -OsType Linux `
    -Cpu 1 `
    -MemoryInGB 1.5 `
    -IpAddressType Public `
    -Port @(80)
```

### 4. Verify Deployment
```powershell
Get-AzContainerGroup -ResourceGroupName $resourceGroup -Name $containerGroupName
```

### 5. Get Container Logs
```powershell
Get-AzContainerInstanceLog `
    -ResourceGroupName $resourceGroup `
    -ContainerGroupName $containerGroupName `
    -ContainerName $containerName
```

### 6. Clean Up Resources
```powershell
Remove-AzContainerGroup `
    -ResourceGroupName $resourceGroup `
    -Name $containerGroupName
```