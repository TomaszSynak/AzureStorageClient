namespace AzureStorageClient
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Extensions.Options;

    internal class AzureTableClient : IAzureTableClient
    {
        private readonly AzureTableContainer _azureTableContainer;

        public AzureTableClient(IOptions<AzureTableClientSettings> options)
        {
            // ToDo: verify that settings are neither null nor empty
            var cloudStorageAccount = CloudStorageAccount.Parse(options.Value.ConnectionString);
            _azureTableContainer = new AzureTableContainer(cloudStorageAccount);
        }

        public async Task<bool> IsAccessible<TStorable>(CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new()
        {
            return await _azureTableContainer
                .GetAzureTable<TStorable>()
                .IsAccessible(cancellationToken);
        }

        public async Task UpsertAsync<TStorable>(TStorable objectToUpsert, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new()
        {
            try
            {
                await _azureTableContainer
                    .GetAzureTable<TStorable>()
                    .Upsert(objectToUpsert, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to UPSERT entity {objectToUpsert.AzureTableRowId}. ", exception);
            }
        }

        public async Task<TStorable> GetAsync<TStorable>(Guid azureTableRowId, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new()
        {
            try
            {
                return await _azureTableContainer
                    .GetAzureTable<TStorable>()
                    .Get(azureTableRowId, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to GET entity {azureTableRowId}. ", exception);
            }
        }

        public async Task<ImmutableList<TStorable>> GetListAsync<TStorable>(string prefix = null, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new()
        {
            try
            {
                return await _azureTableContainer
                    .GetAzureTable<TStorable>()
                    .GetList(prefix, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to GET entities with prefix {prefix}. ", exception);
            }
        }

        public async Task SoftDeleteAsync<TStorable>(Guid azureTableRowId, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new()
        {
            try
            {
                await _azureTableContainer
                    .GetAzureTable<TStorable>()
                    .SetIsDeleted(azureTableRowId, true, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to SOFT DELETE entity {azureTableRowId}. ", exception);
            }
        }

        public async Task RevertSoftDeleteAsync<TStorable>(Guid azureTableRowId, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new()
        {
            try
            {
                await _azureTableContainer
                    .GetAzureTable<TStorable>()
                    .SetIsDeleted(azureTableRowId, false, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to REVERT SOFT DELETE entity {azureTableRowId}. ", exception);
            }
        }

        public async Task DeleteAsync<TStorable>(Guid azureTableRowId, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new()
        {
            try
            {
                await _azureTableContainer
                    .GetAzureTable<TStorable>()
                    .Delete(azureTableRowId, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to DELETE entity {azureTableRowId}. ", exception);
            }
        }
    }
}
