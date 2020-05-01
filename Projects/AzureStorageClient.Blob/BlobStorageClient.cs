namespace AzureStorageClient
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    internal class BlobStorageClient : IStorageClient
    {
        // ToDo: add performance tests
        private readonly BlobStorageContainer _blobStorageContainer;

        public BlobStorageClient(IOptions<BlobStorageClientSettings> options)
            => _blobStorageContainer = BlobStorageContainerFactory.Create(options);

        public async Task<bool> IsAccessible(CancellationToken cancellationToken = default)
            => await _blobStorageContainer.IsAccessible(cancellationToken);

        public async Task UpsertAsync<TStorable>(TStorable objectToUpsert, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
        {
            try
            {
                var blobStorage = await GetBlobStorage<TStorable>(objectToUpsert.BlobId, cancellationToken);

                var blobStringContent = objectToUpsert.Serialize();

                await blobStorage.Upload(blobStringContent, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to UPSERT blob {objectToUpsert.BlobId}. ", exception);
            }
        }

        public async Task<TStorable> GetAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
        {
            try
            {
                var blobStorage = await GetBlobStorage<TStorable>(blobId, cancellationToken);

                var blobStringContent = await blobStorage.Download(cancellationToken);

                return blobStringContent.Deserialize<TStorable>();
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to GET blob {blobId}. ", exception);
            }
        }

        public async Task<ImmutableList<TStorable>> GetListAsync<TStorable>(string prefix = null, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
        {
            // ToDo: add performance tests
            // ToDo: let prefix be marked with attribute instead of passing it as parameter
            // ToDo: add filter parameters
            // ToDo: what if list would be empty?
            try
            {
                var blobStorageList = await _blobStorageContainer.GetBlobStorageList(GetOrAddBlobIdPrefix<TStorable>(prefix), cancellationToken);

                var downloadingBlobStringContent = blobStorageList.Select(ab => ab.Download(cancellationToken)).ToList();

                await Task.WhenAll(downloadingBlobStringContent);

                return downloadingBlobStringContent.Select(bsc => bsc.Result.Deserialize<TStorable>()).ToImmutableList();
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to GET blob list. ", exception);
            }
        }

        public async Task SoftDeleteAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
        {
            try
            {
                var blobStorage = await GetBlobStorage<TStorable>(blobId, cancellationToken);
                await blobStorage.SetIsDeleted(true, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to CheckAsDeleted blob {blobId}. ", exception);
            }
        }

        public async Task RevertSoftDeleteAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
        {
            try
            {
                var blobStorage = await GetBlobStorage<TStorable>(blobId, cancellationToken);
                await blobStorage.SetIsDeleted(false, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to UncheckAsDeleted blob {blobId}. ", exception);
            }
        }

        public async Task DeleteAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
        {
            // ToDo: make sure that blobId is the one from TStorable, as user intended
            try
            {
                var blobStorage = await GetBlobStorage<TStorable>(blobId, cancellationToken);

                await blobStorage.Delete(cancellationToken: cancellationToken);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to DELETE blob {blobId}. ", exception);
            }
        }

        private static string GetOrAddBlobIdPrefix<TSortable>(string blobId = null)
            => string.IsNullOrWhiteSpace(blobId)
                ? $"{typeof(TSortable).Name}"
                : $"{typeof(TSortable).Name}/{blobId}";

        private async Task<BlobStorage> GetBlobStorage<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
        {
            return await _blobStorageContainer.GetBlobStorage(GetOrAddBlobIdPrefix<TStorable>(blobId), cancellationToken);
        }
    }
}
