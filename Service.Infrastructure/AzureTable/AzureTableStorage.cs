using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace Service.Infrastructure.AzureTable
{
    /// <summary>
    /// Azure table storage repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AzureTableStorage<T> : IAzureTableStorage<T>
        where T : AzureTableEntity, new()
    {
        private readonly AzureTableSettings _settings;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="settings"></param>
        public AzureTableStorage(AzureTableSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Get all list of items
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetList()
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Query
            TableQuery<T> query = new TableQuery<T>();

            List<T> results = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<T> queryResults =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            return results;
        }
        
        /// <summary>
        /// Get list by partition key
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<List<T>> GetList(string partitionKey)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Query
            TableQuery<T> query = new TableQuery<T>()
                                        .Where(TableQuery.GenerateFilterCondition("PartitionKey", 
                                                QueryComparisons.Equal, partitionKey));

            List<T> results = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<T> queryResults =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;

                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            return results;
        }
        
        /// <summary>
        /// Get item by partition key and row key
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public async Task<T> GetItem(string partitionKey, string rowKey)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            //Execute
            TableResult result = await table.ExecuteAsync(operation);

            return (T)result.Result;
        }
        
        /// <summary>
        /// Inserts a new item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task Insert(T item)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.Insert(item);

            //Execute
            await table.ExecuteAsync(operation);
        }
        
        /// <summary>
        /// Updates a row item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task Update(T item)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.InsertOrReplace(item);

            //Execute
            await table.ExecuteAsync(operation);
        }
        
        /// <summary>
        /// Deletes a row by partition key and row key
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public async Task Delete(string partitionKey, string rowKey)
        {
            //Item
            T item = await GetItem(partitionKey, rowKey);
            
            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.Delete(item);

            //Execute
            await table.ExecuteAsync(operation);
        }

        /// <summary>
        /// Return table, create table if not exists already
        /// </summary>
        /// <returns></returns>
        private async Task<CloudTable> GetTableAsync()
        {
            //Account
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new StorageCredentials(_settings.StorageAccount, _settings.StorageKey), true);

            //Client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            //Table
            CloudTable table = tableClient.GetTableReference(_settings.TableName);
            await table.CreateIfNotExistsAsync();

            return table;
        }
    }
}