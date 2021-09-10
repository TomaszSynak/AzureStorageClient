namespace AzureStorageClient
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Microsoft.Azure.Cosmos.Table;

    internal class AzureTableContainer
    {
        private readonly CloudTableClient _cloudTableClient;

        private readonly ConcurrentDictionary<string, IAzureTable> _azureTableDictionary;

        private readonly int _maxCreationRetries = 60;

        private readonly TimeSpan _pauseBetweenCreationRetries = TimeSpan.FromSeconds(1);

        public AzureTableContainer(CloudStorageAccount cloudStorageAccount)
        {
            _cloudTableClient = cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
            _azureTableDictionary = new ConcurrentDictionary<string, IAzureTable>();

            _cloudTableClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(_pauseBetweenCreationRetries, _maxCreationRetries);
            _cloudTableClient.DefaultRequestOptions.ServerTimeout = TimeSpan.FromMilliseconds(3 * _maxCreationRetries * _pauseBetweenCreationRetries.TotalMilliseconds);
            _cloudTableClient.DefaultRequestOptions.PayloadFormat = TablePayloadFormat.JsonNoMetadata;
        }

        public static string GetTableName<TStorable>()
            => $"{typeof(TStorable).Name}";

        public AzureTable<TStorable> GetAzureTable<TStorable>()
            where TStorable : class, ITableStorable, new()
        {
            var tableName = GetTableName<TStorable>();

            if (_azureTableDictionary.TryGetValue(tableName, out var azureTable))
            {
                return azureTable as AzureTable<TStorable>;
            }

            azureTable = _azureTableDictionary.GetOrAdd(tableName, CreateAzureTable<TStorable>);

            return (AzureTable<TStorable>)azureTable;
        }

        private IAzureTable CreateAzureTable<TStorable>(string tableName)
            where TStorable : class, ITableStorable, new()
        {
            var cloudTable = _cloudTableClient.GetTableReference(tableName);

            var azureTable = new AzureTable<TStorable>(cloudTable);

            using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
            {
                azureTable.Initialize(cancellationTokenSource.Token).GetAwaiter().GetResult();
            }

            return azureTable;
        }
    }
}
