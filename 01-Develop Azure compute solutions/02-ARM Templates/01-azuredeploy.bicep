
var storageAccountName = 'mkdemo0101'
param location string = resourceGroup().location

resource storageAccount 'Microsoft.Storage/storageAccounts@2025-06-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
  tags: {
    environment: 'dev'
    project: 'mkdemo'
    business_unit: 'connectivity'
  }
}

output accountName string = storageAccountName
