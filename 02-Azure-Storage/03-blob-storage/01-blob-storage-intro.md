# **Azure Blob Storage – Quick Reference Guide**

## **1. Overview**

Azure Blob Storage is a cloud-based object storage service for storing **unstructured data** such as:

* Text files (Markdown `.md`), images, videos
* Backups, logs, large datasets

It is **scalable**, **secure**, and accessible via HTTP/HTTPS.

---

## **2. Key Concepts**

| Concept             | Description                                                                     |
| ------------------- | ------------------------------------------------------------------------------- |
| **Storage Account** | Top-level container for all Azure storage resources                             |
| **Container**       | Acts like a folder; all blobs must reside in a container                        |
| **Blob**            | The actual file stored in storage (Block, Append, Page)                         |
| **Access Tiers**    | Hot: frequent access <br> Cool: infrequent access <br> Archive: rarely accessed |

**Blob Types:**

* **Block blob:** For text & binary files (most common)
* **Append blob:** Optimized for logs or append operations
* **Page blob:** Optimized for random read/write (e.g., virtual disks)

---

## **3. Uploading Files**

### **Azure Portal**

1. Go to **Storage Account → Containers → Select container**
2. Click **Upload**
3. Select file (e.g., `README.md`) → **Upload**

### **Azure CLI**

```bash
az storage blob upload \
  --account-name <storage-account-name> \
  --container-name <container-name> \
  --name <blob-name> \
  --file <local-file-path>
```

Example:

```bash
az storage blob upload \
  --account-name mystorageaccount \
  --container-name mycontainer \
  --name README.md \
  --file ./README.md
```

### **Python SDK**

```python
from azure.storage.blob import BlobServiceClient

conn_str = "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;EndpointSuffix=core.windows.net"
blob_service_client = BlobServiceClient.from_connection_string(conn_str)
container_client = blob_service_client.get_container_client("mycontainer")

with open("README.md", "rb") as data:
    container_client.upload_blob(name="README.md", data=data)
```

---

## **4. Accessing Files**

* **Public URL:**

```
https://<storage-account-name>.blob.core.windows.net/<container-name>/<blob-name>
```

* **Secure Access:** Use a **SAS token** if the container is private.

---
## **5. Containers

* Organizes a set of blobs
* Accounts can store unlimited containers
* Container name must be a valid DNS name
* A container must exist before data can be uploaded 

## **6. Useful Commands**

* List containers:

```bash
az storage container list --account-name <storage-account-name>
```

* List blobs in a container:

```bash
az storage blob list --account-name <storage-account-name> --container-name <container-name> --output table
```

* Delete a blob:

```bash
az storage blob delete --account-name <storage-account-name> --container-name <container-name> --name <blob-name>
```

