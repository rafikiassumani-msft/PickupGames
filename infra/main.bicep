targetScope = 'subscription'

resource dotnetRg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: 'pickupGameRG'
  location: 'westus'
  tags:{
    'Note': 'deployment for microservices'   
  }
}

module pickupGameAcr './acr.bicep' = {
  name: 'pickupGameAcr'
  scope: dotnetRg    // Deployed in the scope of resource group we created above
}




