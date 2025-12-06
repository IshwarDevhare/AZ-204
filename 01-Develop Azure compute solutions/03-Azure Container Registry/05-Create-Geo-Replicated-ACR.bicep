param resourceName string
param location string = resourceGroup().location
param sku string = 'Basic'


resource acr 'Microsoft.ContainerRegistry/registries@2025-11-01' = {
  name: resourceName
  location: location
  sku: {
    name: sku
  }
  properties: {
    adminUserEnabled: false
  }
}

output registryName string = acr.name
output acrloginServer string = acr.properties.loginServer

// resource acrReplication 'Microsoft.ContainerRegistry/registries/replications@2025-11-01' = {
//   parent: acr
//   name: 'centralindia'
//   location: 'centralindia'
// }
