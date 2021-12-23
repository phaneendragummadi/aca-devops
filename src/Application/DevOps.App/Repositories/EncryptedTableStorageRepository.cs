using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using ConfigurationProvider = DevOps.App.Configuration.ConfigurationProvider;

namespace DevOps.App.Repositories
{
    public class EncryptedTableStorageRepository
    {
        private const string EncryptionEntityPartitionKey = "DevOps";
        private const string EncryptionEntityRowKey = "Introduction";
    
      
        private readonly TableClient _tableClient;


        public EncryptedTableStorageRepository()
        {
            var connectionString = ConfigurationProvider.Get<string>("StorageAccount.ConnectionString");
            var tableName = ConfigurationProvider.Get<string>("StorageAccount.TableName");
            _tableClient = new TableClient(connectionString, tableName);

            //"https://devdevopsdemostorage.table.core.windows.net/EncryptionTable"
            //var storageAccount = CloudStorageAccount.Parse(connectionString);
            //var tableClient = storageAccount.CreateCloudTableClient();

            //table = tableClient.GetTableReference(tableName);

        }
        private TableEntity ToTableEntity(string key, string value)
        {
            TableEntity tableEntity;
            tableEntity = new TableEntity
            {
                PartitionKey = EncryptionEntityPartitionKey,
                RowKey = EncryptionEntityRowKey
            };
            tableEntity.Add(key, value);
            return tableEntity;
        }
        public async Task UpdateAsync(string entityKey, string entityValue)
        {

            await _tableClient.UpsertEntityAsync(ToTableEntity(entityKey, entityValue));

            //await table.CreateIfNotExistsAsync();
            //await table.ExecuteAsync(operation);
        }

        public async Task<TableEntity> GetAsync()
        {
            var result = await _tableClient.GetEntityAsync<TableEntity>(EncryptionEntityPartitionKey, EncryptionEntityRowKey);
            // var operation = TableOperation.Retrieve<EncryptionEntity>(EncryptionEntity.EncryptionEntityPartitionKey, EncryptionEntity.EncryptionEntityRowKey);
            await _tableClient.CreateIfNotExistsAsync();
            // await table.CreateIfNotExistsAsync();
            //var result = await table.ExecuteAsync(operation);

            if (result == null)
            {
                throw new InvalidOperationException("No entity was found. Please insert an entity first.");
            }

            
            return result;
        }
    }
}