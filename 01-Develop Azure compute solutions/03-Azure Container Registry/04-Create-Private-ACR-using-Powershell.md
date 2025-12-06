# Create Azure Container Registry using PowerShell

## Login to Azure

```powershell
Connect-AzAccount
```

## Create Resource Group

```powershell
New-AzResourceGroup -Name myResourceGroup -Location EastUS
```


## Create Azure Container Registry

```powershell
New-AzContainerRegistry -ResourceGroupName myResourceGroup -Name myacr -Sku Basic -Location EastUS


$registry = New-AzContainerRegistry -ResourceGroupName "myResourceGroup" -Name "mycontainerregistry" -EnableAdminUser -Sku Standard -Location EastUS
```

**SKU Options:** `Basic`, `Standard`, `Premium`

## Enable Admin User (Optional)

```powershell
Update-AzContainerRegistry -Name myacr -ResourceGroupName myResourceGroup -EnableAdminUser
```

## Get Registry Credentials

```powershell
Get-AzContainerRegistryCredential -ResourceGroupName myResourceGroup -Name myacr
```

## Login to the registry
```
Connect-AzContainerRegistry -Name "acr_name"
Connect-AzContainerRegistry -Name $registry.Name
```

## Login to ACR with Docker

```powershell
$creds = Get-AzContainerRegistryCredential -ResourceGroupName myResourceGroup -Name myacr
$creds.Password | docker login myacr.azurecr.io -u $creds.Username --password-stdin
```

## Push an Image to ACR

```powershell
# Tag the image
docker tag myimage:latest myacr.azurecr.io/myimage:v1

# Push the image
docker push myacr.azurecr.io/myimage:v1
```

## List Repositories

```powershell
Get-AzContainerRegistryRepository -RegistryName myacr
```

## Delete ACR (Optional)

```powershell
Remove-AzContainerRegistry -Name myacr -ResourceGroupName myResourceGroup
```
