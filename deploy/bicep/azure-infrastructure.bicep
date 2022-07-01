@description('Provide the pricing tier of the key vault.')
param Storage_SkuName string = 'Standard_LRS'
param StorageAccountTableName string
param Application_Name string
param Application_Environment string
param Application_Version string

@description('The Runtime stack of current web app')
param Appication_LinuxFxVersion string = 'DOTNETCORE|6.0'
param Release_Name string
param Release_RequestedFor string
param Release_SourceCodeBranch string
param Release_TriggerType string
param Release_Url string

@description('Provide the pricing tier of the App Service Plan.')
param AppSvcPlan_SkuName string = 'B1'

@description('Provide the pricing tier of the key vault.')
param Keyvault_SkuName string = 'Standard'
param Keyvault_TenantId string
param Keyvault_ObjectId string

@description('Specifies the name of the secret that you want to create.')
param Keyvault_SecretName string

@description('Value of the secret from Key Vault.')
@secure()
param Keyvault_SecretValue string

var KeyVaultName_var = '${Application_Name}-vault'
var AppServicePlanName_var = '${Application_Name}Plan'
var AppServiceName_var = Application_Name
var StorageAccountName_var = '${toLower(replace(Application_Name, '-', ''))}storage'
var StorageAccountResourceId = StorageAccountName.id
var Tags = {
  environment: Application_Environment
  version: Application_Version
  releaseName: Release_Name
  createdBy: Release_Url
  branch: Release_SourceCodeBranch
  triggeredBy: Release_RequestedFor
  triggerType: Release_TriggerType
}

resource default 'Microsoft.Resources/tags@2019-10-01' = {
  name: 'default'
  properties: {
    tags: Tags
  }
  dependsOn: []
}

resource StorageAccountName 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: StorageAccountName_var
  location: resourceGroup().location
  tags: Tags
  sku: {
    tier: 'Standard'
    name: Storage_SkuName
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
  }
}

resource StorageAccountName_default_StorageAccountTableName 'Microsoft.Storage/storageAccounts/tableServices/tables@2019-06-01' = {
  name: '${StorageAccountName_var}/default/${StorageAccountTableName}'
  dependsOn: [
    StorageAccountName
  ]
}

resource AppServicePlanName 'Microsoft.Web/serverFarms@2020-06-01' = {
  name: AppServicePlanName_var
  location: resourceGroup().location
  tags: Tags
  kind: 'linux'
  sku: {
    name: AppSvcPlan_SkuName
    capacity: 1
  }
  properties: {
    reserved: true
  }
}

resource AppServiceName 'Microsoft.Web/sites@2020-06-01' = {
  name: AppServiceName_var
  location: resourceGroup().location
  tags: Tags
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'app'
  properties: {
    serverFarmId: AppServicePlanName.id
    siteConfig: {
      linuxFxVersion: Appication_LinuxFxVersion
      connectionStrings: [
        {
          name: 'StorageAccount.ConnectionString'
          connectionString: 'DefaultEndpointsProtocol=https;AccountName=${StorageAccountName_var};AccountKey=${listKeys(StorageAccountResourceId, '2019-06-01').keys[0].value}'
        }
      ]
      appSettings: [
        {
          name: 'StorageAccount.TableName'
          value: StorageAccountTableName
        }
        {
          name: 'Encryption.Key'
          value: 'EC545C08-43AF-4B49-965F-75B27300CFA0'
        }
        {
          name: 'VaultUri'
          value: 'https://${KeyVaultName_var}.vault.azure.net/'
        }
      ]
    }
  }
}

resource KeyVaultName 'Microsoft.KeyVault/vaults@2018-02-14' = {
  name: KeyVaultName_var
  location: resourceGroup().location
  tags: Tags
  properties: {
    tenantId: Keyvault_TenantId
    sku: {
      name: Keyvault_SkuName
      family: 'A'
    }
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
    enabledForTemplateDeployment: true
    accessPolicies: [
      {
        tenantId: Keyvault_TenantId
        objectId: Keyvault_ObjectId
        permissions: {
          keys: [
            'list'
            'get'
          ]
          secrets: [
            'list'
            'get'
          ]
        }
      }
      {
        tenantId: Keyvault_TenantId
        objectId: reference(AppServiceName.id, '2019-08-01', 'full').identity.principalId
        permissions: {
          secrets: [
            'get'
          ]
        }
      }
    ]
  }
}

resource keyVaultName_Keyvault_SecretName 'Microsoft.KeyVault/vaults/secrets@2018-02-14' = {
  parent: KeyVaultName
  name: '${Keyvault_SecretName}'
  tags: Tags
  properties: {
    value: Keyvault_SecretValue
  }
  dependsOn: [
    AppServiceName
  ]
}