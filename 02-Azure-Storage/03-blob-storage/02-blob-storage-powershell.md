## 1. Prerequisites

### Install the Az module (once)

Open PowerShell **as Administrator**:

```powershell
Install-Module -Name Az -Repository PSGallery -Force
```

### Sign in to Azure

```powershell
Connect-AzAccount
```

If you have multiple subscriptions:

```powershell
Get-AzSubscription
Set-AzContext -SubscriptionId "<your-subscription-id>"
```

---

## 2. Create or Get a Storage Account

### Variables

```powershell
$resourceGroup = "MyResourceGroup"
$location = "eastus"
$storageAccountName = "mystorageacct123"   # must be globally unique
```

### Create a resource group (if needed)

```powershell
New-AzResourceGroup -Name $resourceGroup -Location $location
```

### Create a storage account

```powershell
New-AzStorageAccount `
  -ResourceGroupName $resourceGroup `
  -Name $storageAccountName `
  -Location $location `
  -SkuName Standard_LRS `
  -Kind StorageV2
```

---

## 3. Get the Storage Context

Most blob commands use a **storage context**.

```powershell
$storageAccount = Get-AzStorageAccount `
  -ResourceGroupName $resourceGroup `
  -Name $storageAccountName

$ctx = $storageAccount.Context
```

---

## 4. Create a Blob Container

```powershell
$containerName = "mycontainer"

New-AzStorageContainer `
  -Name $containerName `
  -Context $ctx `
  -Permission Off
```

`Permission Off` keeps it private (recommended).

---

## 5. Upload Files to Blob Storage

### Upload a single file

```powershell
Set-AzStorageBlobContent `
  -File "C:\files\example.txt" `
  -Container $containerName `
  -Blob "example.txt" `
  -Context $ctx
```

### Upload an entire folder

```powershell
Get-ChildItem "C:\files" -File | ForEach-Object {
    Set-AzStorageBlobContent `
      -File $_.FullName `
      -Container $containerName `
      -Blob $_.Name `
      -Context $ctx
}
```

---

## 6. List Blobs in a Container

```powershell
Get-AzStorageBlob `
  -Container $containerName `
  -Context $ctx
```

---

## 7. Download a Blob

```powershell
Get-AzStorageBlobContent `
  -Container $containerName `
  -Blob "example.txt" `
  -Destination "C:\downloads\example.txt" `
  -Context $ctx
```

---

## 8. Delete a Blob

```powershell
Remove-AzStorageBlob `
  -Container $containerName `
  -Blob "example.txt" `
  -Context $ctx
```

---

## 9. Using a SAS Token (No Login Required)

If you **can’t log in** (e.g., scripts, CI/CD):

### Generate a SAS token

```powershell
New-AzStorageContainerSASToken `
  -Name $containerName `
  -Context $ctx `
  -Permission rwdl `
  -ExpiryTime (Get-Date).AddHours(2)
```

### Use SAS with context

```powershell
$ctx = New-AzStorageContext `
  -StorageAccountName $storageAccountName `
  -SasToken "<sas-token-here>"
```

---

## 10. Common Commands Cheat Sheet

| Task            | Command                    |
| --------------- | -------------------------- |
| Login           | `Connect-AzAccount`        |
| List containers | `Get-AzStorageContainer`   |
| Upload blob     | `Set-AzStorageBlobContent` |
| Download blob   | `Get-AzStorageBlobContent` |
| Delete blob     | `Remove-AzStorageBlob`     |