namespace AzureStorageClient
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Azure.Cosmos.Table.Queryable;

    internal class AzureTable<TStorable> : IAzureTable
        where TStorable : class, ITableStorable, new()
    {
        private readonly CloudTable _cloudTable;

        public AzureTable(CloudTable cloudTable) => _cloudTable = cloudTable;

        public static string GetPartitionKey() => $"{typeof(TStorable).Name}";

        public async Task Initialize(CancellationToken cancellationToken = default)
        {
            if (await _cloudTable.ExistsAsync(cancellationToken))
            {
                return;
            }

            await _cloudTable.CreateIfNotExistsAsync(cancellationToken);
        }

        public async Task<bool> IsAccessible(CancellationToken cancellationToken = default) => await _cloudTable.ExistsAsync(cancellationToken);

        public async Task Upsert(TStorable objectToUpsert, CancellationToken cancellationToken = default)
        {
            objectToUpsert.PartitionKey = GetPartitionKey();
            objectToUpsert.RowKey = objectToUpsert.AzureTableRowId.ToString("N");

            if (Encoding.UTF8.GetByteCount(objectToUpsert.RowKey) > 1024)
            {
                throw new Exception("RowKey to big");
            }

            var upsertOperation = TableOperation.InsertOrReplace(objectToUpsert);

            await _cloudTable.ExecuteAsync(upsertOperation, cancellationToken);
        }

        public async Task<TStorable> Get(Guid azureTableRowId, CancellationToken cancellationToken = default)
        {
            // ToDo: check RowKey limits and disallowed characters
            var rowKey = azureTableRowId.ToString("N");
            if (Encoding.UTF8.GetByteCount(rowKey) > 1024)
            {
                throw new Exception("RowKey to big");
            }

            var getOperation = TableOperation.Retrieve<TStorable>(GetPartitionKey(), rowKey);

            var result = await _cloudTable.ExecuteAsync(getOperation, cancellationToken);

            return result.Result as TStorable;
        }

        public async Task<ImmutableList<TStorable>> GetList(string prefix = null, CancellationToken cancellationToken = default)
        {
            prefix = prefix ?? GetPartitionKey();

            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, prefix);

            var query = new TableQuery<TStorable>().Where(filter).AsTableQuery();

            TableContinuationToken continuationToken = null;

            var result = new List<TStorable>();

            do
            {
                var partialResult = await _cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken, cancellationToken);
                result.AddRange(partialResult);
                continuationToken = partialResult.ContinuationToken;
            }
            while (continuationToken != null);

            return result.ToImmutableList();
        }

        public async Task SetIsDeleted(Guid azureTableRowId, bool isDeleted, CancellationToken cancellationToken = default)
        {
            var objectToSoftDelete = await Get(azureTableRowId, cancellationToken);

            objectToSoftDelete.IsDeleted = isDeleted;

            await Upsert(objectToSoftDelete, cancellationToken);
        }

        public async Task Delete(Guid azureTableId, CancellationToken cancellationToken = default)
        {
            var objectToDelete = await Get(azureTableId, cancellationToken);

            var deleteOperation = TableOperation.Delete(objectToDelete);

            await _cloudTable.ExecuteAsync(deleteOperation, cancellationToken);
        }
    }
}
