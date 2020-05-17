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

        public static string GetPartitionKey(Guid? azureTablePartitionId)
            => azureTablePartitionId.HasValue ? $"{azureTablePartitionId:N}" : $"{typeof(TStorable).Name}";

        public static string GetRowKey(Guid azureTableRowId)
        {
            // ToDo: check RowKey limits and disallowed characters
            var rowKey = $"{azureTableRowId:N}";
            if (Encoding.UTF8.GetByteCount(rowKey) > 1024)
            {
                throw new Exception("RowKey to big");
            }

            return rowKey;
        }

        public async Task Initialize(CancellationToken cancellationToken = default)
        {
            if (await _cloudTable.ExistsAsync(cancellationToken))
            {
                return;
            }

            await _cloudTable.CreateIfNotExistsAsync(cancellationToken);
        }

        public async Task<bool> IsAccessible(CancellationToken cancellationToken = default) => await _cloudTable.ExistsAsync(cancellationToken);

        public async Task Upsert(TStorable objectToUpsert, Guid? azureTablePartitionId = null, CancellationToken cancellationToken = default)
        {
            objectToUpsert.PartitionKey = GetPartitionKey(azureTablePartitionId);
            objectToUpsert.RowKey = GetRowKey(objectToUpsert.AzureTableRowId);

            var upsertOperation = TableOperation.InsertOrReplace(objectToUpsert);

            await _cloudTable.ExecuteAsync(upsertOperation, cancellationToken);
        }

        public async Task<TStorable> Get(Guid azureTableRowId, Guid? azureTablePartitionId = null, CancellationToken cancellationToken = default)
        {
            var getOperation = TableOperation.Retrieve<TStorable>(GetPartitionKey(azureTablePartitionId), GetRowKey(azureTableRowId));

            var result = await _cloudTable.ExecuteAsync(getOperation, cancellationToken);

            return result.Result as TStorable;
        }

        public async Task<ImmutableList<TStorable>> GetList(Guid? azureTablePartitionId = null, CancellationToken cancellationToken = default)
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, GetPartitionKey(azureTablePartitionId));

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

        public async Task SetIsDeleted(Guid azureTableRowId, bool isDeleted, Guid? azureTablePartitionId = null, CancellationToken cancellationToken = default)
        {
            var objectToSoftDelete = await Get(azureTableRowId, azureTablePartitionId, cancellationToken);

            objectToSoftDelete.IsDeleted = isDeleted;

            await Upsert(objectToSoftDelete, azureTablePartitionId, cancellationToken);
        }

        public async Task Delete(Guid azureTableRowId, Guid? azureTablePartitionId = null, CancellationToken cancellationToken = default)
        {
            var objectToDelete = await Get(azureTableRowId, azureTablePartitionId, cancellationToken);

            var deleteOperation = TableOperation.Delete(objectToDelete);

            await _cloudTable.ExecuteAsync(deleteOperation, cancellationToken);
        }
    }
}
