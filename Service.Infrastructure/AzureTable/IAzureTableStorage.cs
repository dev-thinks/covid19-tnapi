using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Infrastructure.AzureTable
{
    /// <summary>
    /// Azure table operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAzureTableStorage<T> where T : AzureTableEntity, new()
    {
        /// <summary>
        /// Get item by partition key and row key
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        Task<T> GetItem(string partitionKey, string rowKey);

        /// <summary>
        /// Get all list items
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetList();

        /// <summary>
        /// Get list by partition key
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task<List<T>> GetList(string partitionKey);

        /// <summary>
        /// Inserts a new item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task Insert(T item);

        /// <summary>
        /// Updates a row
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task Update(T item);

        /// <summary>
        /// Deletes a row by partition key and row key
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        Task Delete(string partitionKey, string rowKey);
    }
}