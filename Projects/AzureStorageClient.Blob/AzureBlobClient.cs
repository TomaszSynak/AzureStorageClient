namespace AzureStorageClient
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    internal class AzureBlobClient : IAzureBlobClient
    {
        // ToDo: add performance tests
        private readonly AzureBlobContainer _azureBlobContainer;

        public AzureBlobClient(IOptions<AzureBlobClientSettings> options)
        {
            // ToDo: verify that settings are neither null nor empty
            _azureBlobContainer = AzureBlobContainerFactory.Create(options);
        }

        public async Task<bool> IsAccessible(CancellationToken cancellationToken = default)
            => await _azureBlobContainer.IsAccessible(cancellationToken);

        public async Task UpsertAsync<TStorable>(TStorable objectToUpsert, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new()
        {
            try
            {
                var azureBlob = await GetAzureBlob<TStorable>(objectToUpsert.StorableId, cancellationToken);

                var azureBlobStringContent = objectToUpsert.Serialize();

                await azureBlob.Upload(azureBlobStringContent, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to UPSERT blob {objectToUpsert.StorableId}. ", exception);
            }
        }

        public async Task<TStorable> GetAsync<TStorable>(string storableId, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new()
        {
            try
            {
                var azureBlob = await GetAzureBlob<TStorable>(storableId, cancellationToken);

                var azureBlobStringContent = await azureBlob.Download(cancellationToken);

                return azureBlobStringContent.Deserialize<TStorable>();
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to GET blob {storableId}. ", exception);
            }
        }

        public async Task<ImmutableList<TStorable>> GetListAsync<TStorable>(string prefix = null, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new()
        {
            // ToDo: add performance tests
            // ToDo: let prefix be marked with attribute instead of passing it as parameter
            // ToDo: add filter parameters
            // ToDo: what if list would be empty?
            try
            {
                var azureBlobList = await _azureBlobContainer.GetAzureBlobList(GetOrAddBlobIdPrefix<TStorable>(prefix), cancellationToken);

                var downloadingAzureBlobStringContent = azureBlobList.Select(ab => ab.Download(cancellationToken)).ToList();

                await Task.WhenAll(downloadingAzureBlobStringContent);

                return downloadingAzureBlobStringContent.Select(bsc => bsc.Result.Deserialize<TStorable>()).ToImmutableList();
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to GET blob list. ", exception);
            }
        }

        public async Task SoftDeleteAsync<TStorable>(string storableId, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new()
        {
            try
            {
                var azureBlob = await GetAzureBlob<TStorable>(storableId, cancellationToken);
                await azureBlob.SetIsDeleted(true, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to CheckAsDeleted blob {storableId}. ", exception);
            }
        }

        public async Task RevertSoftDeleteAsync<TStorable>(string storableId, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new()
        {
            try
            {
                var azureBlob = await GetAzureBlob<TStorable>(storableId, cancellationToken);
                await azureBlob.SetIsDeleted(false, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to UncheckAsDeleted blob {storableId}. ", exception);
            }
        }

        public async Task DeleteAsync<TStorable>(string storableId, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new()
        {
            // ToDo: make sure that storableId is the one from TStorable, as user intended
            try
            {
                var azureBlob = await GetAzureBlob<TStorable>(storableId, cancellationToken);

                await azureBlob.Delete(cancellationToken: cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to DELETE blob {storableId}. ", exception);
            }
        }

        private static string GetOrAddBlobIdPrefix<TStorable>(string storableId = null)
            => string.IsNullOrWhiteSpace(storableId)
                ? $"{typeof(TStorable).Name}"
                : $"{typeof(TStorable).Name}/{storableId}";

        private async Task<AzureBlob> GetAzureBlob<TStorable>(string storableId, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new()
        {
            return await _azureBlobContainer.GetAzureBlob(GetOrAddBlobIdPrefix<TStorable>(storableId), cancellationToken);
        }
    }
}
