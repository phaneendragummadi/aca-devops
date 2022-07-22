using System;
using System.Threading.Tasks;
using Arcus.Security.Core;
using DevOps.App.Interfaces;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using GuardNet;
using Microsoft.Extensions.Configuration;

namespace DevOps.App.Repositories
{
    public class EncryptedTableStorageRepository
    {
        private const string EncryptionEntityPartitionKey = "DevOps";
        private const string EncryptionEntityRowKey = "Introduction";
        private readonly IConfiguration _configuration;
        private readonly TableClient _tableClient;
        private readonly IKeyVaultManager _secretManager;

        public EncryptedTableStorageRepository(IKeyVaultManager secretManager,IConfiguration configuration)
        {
            _secretManager = secretManager;
            _configuration = configuration;
            var storageConnectionStringName = _configuration["StorageAccount.ConnectionString"];
            var storageTableName = _configuration["StorageAccount.TableName"];
            string connectionString = _secretManager.GetSecret(storageConnectionStringName).Result;
            string tableName = _secretManager.GetSecret(storageTableName).Result;

            var serviceClient = new TableServiceClient(connectionString);
            var tableItem  = serviceClient.CreateTableIfNotExists(tableName);
            _tableClient = new TableClient(connectionString, tableName);
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
        }

        public async Task<TableEntity> GetAsync()
        {
            var result = await _tableClient.GetEntityAsync<TableEntity>(EncryptionEntityPartitionKey, EncryptionEntityRowKey);
            await _tableClient.CreateIfNotExistsAsync();

            if (result == null)
            {
                throw new InvalidOperationException("No entity was found. Please insert an entity first.");
            }
           
            return result;
        }
    }
}