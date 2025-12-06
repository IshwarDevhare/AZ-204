# Create Azure Container Registry using Azure Portal

## Steps to Create ACR

### 1. Navigate to Azure Portal
- Go to https://portal.azure.com
- Sign in with your Azure account

### 2. Create Container Registry
- Click **"Create a resource"**
- Search for **"Container Registry"**
- Click **"Create"**

### 3. Configure Basic Settings
**Subscription**: Select your Azure subscription

**Resource Group**: 
- Select existing or click "Create new"
- Enter resource group name

**Registry Name**:
- Must be globally unique
- 5-50 alphanumeric characters only
- Example: `mycontainerregistry001`

**Location**: Select the Azure region (e.g., East US)

**SKU**: Choose pricing tier
- Basic (Development)
- Standard (Production)
- Premium (High-volume, geo-replication)

### 4. Configure Networking (Optional)
- **Public access**: Enable/disable public endpoint
- **Private endpoint**: Configure for VNet integration (Premium only)

### 5. Configure Encryption (Optional - Premium only)
- Use customer-managed keys for encryption

### 6. Review and Create
- Click **"Review + create"**
- Verify all settings
- Click **"Create"**

### 7. Deployment
- Wait for deployment to complete (1-2 minutes)
- Click **"Go to resource"**

## Post-Creation Steps

### Enable Admin User (for testing)
1. In ACR resource, go to **"Access keys"**
2. Enable **"Admin user"**
3. Copy username and password for Docker login

### View Login Server
- In Overview page, note the **Login server** (e.g., `myregistry.azurecr.io`)

### Connect to Registry
```bash
# Login using Azure CLI
az acr login --name mycontainerregistry001

# Or login with Docker using admin credentials
docker login mycontainerregistry001.azurecr.io
```

## Important Notes
- Admin user is for testing only; use service principals or managed identities for production
- Registry name cannot be changed after creation
- Premium tier required for geo-replication and private endpoints
