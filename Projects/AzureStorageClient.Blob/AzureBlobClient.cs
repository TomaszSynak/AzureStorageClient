namespace AzureStorageClient
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class AzureBlobClient : IAzureBlobClient
    {
        // ToDo: add performance tests
        private readonly AzureBlobContainer _azureBlobContainer;

        public AzureBlobClient(AzureBlobContainer azureBlobContainer) => _azureBlobContainer = azureBlobContainer;

        public async Task<bool> IsAccessible(CancellationToken cancellationToken = default)
            => await _azureBlobContainer.IsAccessible(cancellationToken);

        public async Task UpsertAsync<TStorable>(TStorable objectToUpsert, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            try
            {
                var azureBlob = await GetAzureBlob<TStorable>(objectToUpsert.BlobPath, cancellationToken);

                await azureBlob.Upload(objectToUpsert, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new BlobClientException($"Failed to UPSERT blob {objectToUpsert.BlobPath}. ", exception);
            }
        }

        public async Task UpsertAsync<TStorable>(IReadOnlyList<TStorable> objectToUpsertList, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            try
            {
                await _azureBlobContainer.Initialize(cancellationToken);

                const int batchSize = 50;
                var numberOfBatches = (int)Math.Ceiling(objectToUpsertList.Count / (decimal)batchSize);

                for (var i = 0; i < numberOfBatches; ++i)
                {
                    var uploadingAzureBlobs = objectToUpsertList
                        .Skip(i * batchSize).Take(batchSize)
                        .Select(o => UpsertAsync(o, cancellationToken))
                        .ToList();

                    // ToDo: if any task throw exception, rethrow it
                    await Task.WhenAll(uploadingAzureBlobs);
                }
            }
            catch (Exception exception)
            {
                throw new BlobClientException($"Failed to BULK UPSERT blobs {string.Join(",", objectToUpsertList.Select(b => b.BlobPath))}. ", exception);
            }
        }

        public async Task<TStorable> GetAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            try
            {
                var azureBlob = await GetAzureBlob<TStorable>(blobPath, cancellationToken);

                return await azureBlob.Download<TStorable>(cancellationToken);
            }
            catch (Exception exception)
            {
                throw new BlobClientException($"Failed to GET blob {blobPath}. ", exception);
            }
        }

        public async Task<ImmutableList<TStorable>> GetFolderContentAsync<TStorable>(string prefix = null, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            // ToDo: add performance tests
            // ToDo: let prefix be marked with attribute instead of passing it as parameter
            // ToDo: add filter parameters
            // ToDo: what if list would be empty?
            try
            {
                return await _azureBlobContainer.GetAzureBlobFolder<TStorable>(GetOrAddBlobPathPrefix<TStorable>(prefix), cancellationToken);
            }
            catch (Exception exception)
            {
                throw new BlobClientException("Failed to GET blob list. ", exception);
            }
        }

        public async Task SoftDeleteAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            try
            {
                var azureBlob = await GetAzureBlob<TStorable>(blobPath, cancellationToken);
                await azureBlob.SetIsDeleted(true, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new BlobClientException($"Failed to CheckAsDeleted blob {blobPath}. ", exception);
            }
        }

        public async Task RevertSoftDeleteAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            try
            {
                var azureBlob = await GetAzureBlob<TStorable>(blobPath, cancellationToken);
                await azureBlob.SetIsDeleted(false, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new BlobClientException($"Failed to UncheckAsDeleted blob {blobPath}. ", exception);
            }
        }

        public async Task DeleteAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            // ToDo: make sure that blobPath is the one from TStorable, as user intended
            try
            {
                var azureBlob = await GetAzureBlob<TStorable>(blobPath, cancellationToken);

                await azureBlob.Delete(cancellationToken: cancellationToken);
            }
            catch (Exception exception)
            {
                throw new BlobClientException($"Failed to DELETE blob {blobPath}. ", exception);
            }
        }

        public async Task DeleteFolderAsync<TStorable>(string blobPath = null, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            try
            {
                await _azureBlobContainer.DeleteAzureBlobFolder(GetOrAddBlobPathPrefix<TStorable>(blobPath), cancellationToken);
            }
            catch (Exception exception)
            {
                throw new BlobClientException($"Failed to DELETE folder {blobPath}. ", exception);
            }
        }

        private static string GetOrAddBlobPathPrefix<TStorable>(string blobPath = null)
            => string.IsNullOrWhiteSpace(blobPath)
                ? $"{typeof(TStorable).Name}"
                : $"{typeof(TStorable).Name}/{blobPath}";

        private async Task<AzureBlob> GetAzureBlob<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            return await _azureBlobContainer.GetAzureBlob(GetOrAddBlobPathPrefix<TStorable>(blobPath), cancellationToken);
        }
    }
}
