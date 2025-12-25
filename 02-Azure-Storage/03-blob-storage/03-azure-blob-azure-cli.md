# Using Azure Blob Storage with Azure CLI

This guide explains how to work with **Azure Blob Storage using the Azure CLI**.

---

## 1. Sign In to Azure

```bash
az login
````

If you have multiple subscriptions:

```bash
az account list --output table
az account set --subscription "<your-subscription-id>"
```

---

## 2. Define Variables

```bash
RESOURCE_GROUP="MyResourceGroup"
LOCATION="eastus"
STORAGE_ACCOUNT="mystorageacct123"   # must be globally unique
CONTAINER_NAME="mycontainer"
```

---

## 3. Create a Resource Group (If Needed)

```bash
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION
```

---

## 4. Create a Storage Account

```bash
az storage account create \
  --name $STORAGE_ACCOUNT \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku Standard_LRS \
  --kind StorageV2
```

---

## 5. Get the Storage Account Key

```bash
ACCOUNT_KEY=$(az storage account keys list \
  --account-name $STORAGE_ACCOUNT \
  --resource-group $RESOURCE_GROUP \
  --query "[0].value" \
  --output tsv)
```

---

## 6. Create a Blob Container

```bash
az storage container create \
  --name $CONTAINER_NAME \
  --account-name $STORAGE_ACCOUNT \
  --account-key $ACCOUNT_KEY
```

---

## 7. Upload Files to Blob Storage

### Upload a Single File

```bash
az storage blob upload \
  --account-name $STORAGE_ACCOUNT \
  --account-key $ACCOUNT_KEY \
  --container-name $CONTAINER_NAME \
  --file ./example.txt \
  --name example.txt
```

### Upload All Files in a Directory

```bash
az storage blob upload-batch \
  --account-name $STORAGE_ACCOUNT \
  --account-key $ACCOUNT_KEY \
  --destination $CONTAINER_NAME \
  --source ./files
```

---

## 8. List Blobs in a Container

```bash
az storage blob list \
  --account-name $STORAGE_ACCOUNT \
  --account-key $ACCOUNT_KEY \
  --container-name $CONTAINER_NAME \
  --output table
```

---

## 9. Download a Blob

```bash
az storage blob download \
  --account-name $STORAGE_ACCOUNT \
  --account-key $ACCOUNT_KEY \
  --container-name $CONTAINER_NAME \
  --name example.txt \
  --file ./example.txt
```

---

## 10. Delete a Blob

```bash
az storage blob delete \
  --account-name $STORAGE_ACCOUNT \
  --account-key $ACCOUNT_KEY \
  --container-name $CONTAINER_NAME \
  --name example.txt
```

---

## 11. Generate a SAS Token (Optional)

```bash
az storage container generate-sas \
  --account-name $STORAGE_ACCOUNT \
  --account-key $ACCOUNT_KEY \
  --name $CONTAINER_NAME \
  --permissions rwdl \
  --expiry $(date -u -d "2 hours" '+%Y-%m-%dT%H:%MZ') \
  --output tsv
```

---

## 12. Common Commands Reference

| Task                   | Command                        |
| ---------------------- | ------------------------------ |
| Login                  | `az login`                     |
| Create storage account | `az storage account create`    |
| Create container       | `az storage container create`  |
| Upload blob            | `az storage blob upload`       |
| Upload directory       | `az storage blob upload-batch` |
| List blobs             | `az storage blob list`         |
| Download blob          | `az storage blob download`     |
| Delete blob            | `az storage blob delete`       |

---