resource pickupGameAcr 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
  name: 'pickupGameAcr'
  location: resourceGroup().location
  sku: {
    name: 'Standard'
  }
}
