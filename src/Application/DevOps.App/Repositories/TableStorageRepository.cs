using System;
using System.Threading.Tasks;
using DevOps.App.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ConfigurationProvider = DevOps.App.Configuration.ConfigurationProvider;

namespace DevOps.App.Repositories
{
    public class TableStorageRepository
    {
        private readonly CloudTable table;

        public TableStorageRepository()
        {
            var connectionString = ConfigurationProvider.Get<string>("StorageAccount.ConnectionString");
            var tableName = ConfigurationProvider.Get<string>("StorageAccount.TableName");
            
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference(tableName);
        }

        public async Task UpdateAsync(EncryptionEntity entity)
        {
            var operation = TableOperation.InsertOrReplace(entity);

            await table.CreateIfNotExistsAsync();
            await table.ExecuteAsync(operation);
        }

        public async Task<EncryptionEntity> GetAsync()
        {
            var operation = TableOperation.Retrieve<EncryptionEntity>(EncryptionEntity.EncryptionEntityPartitionKey, EncryptionEntity.EncryptionEntityRowKey);

            await table.CreateIfNotExistsAsync();
            var result = await table.ExecuteAsync(operation);

            if (result.Result == null)
            {
                throw new InvalidOperationException("No entity was found. Please insert an entity first.");
            }

            var entity = (EncryptionEntity) result.Result;
            return entity;
        }
    }
}