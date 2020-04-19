using System;

namespace Service.Infrastructure.AzureTable
{
    /// <summary>
    /// Azure table storage settings
    /// </summary>
    public class AzureTableSettings
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="storageAccount"></param>
        /// <param name="storageKey"></param>
        /// <param name="tableName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AzureTableSettings(string storageAccount,
            string storageKey,
            string tableName)
        {
            if (string.IsNullOrEmpty(storageAccount))
                throw new ArgumentNullException(nameof(storageAccount));

            if (string.IsNullOrEmpty(storageKey))
                throw new ArgumentNullException(nameof(storageKey));

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            StorageAccount = storageAccount;
            StorageKey = storageKey;
            TableName = tableName;
        }

        /// <summary>
        /// Azure storage account
        /// </summary>
        public string StorageAccount { get; }
        
        /// <summary>
        /// Azure storage key
        /// </summary>
        public string StorageKey { get; }
        
        /// <summary>
        /// Azure table name
        /// </summary>
        public string TableName { get; }
    }
}