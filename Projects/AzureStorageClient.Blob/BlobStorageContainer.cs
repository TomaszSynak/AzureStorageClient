namespace AzureStorageClient
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.Options;

    internal class BlobStorageContainer
    {
        private readonly BlobContainerClient _blobContainerClient;

        public BlobStorageContainer(IOptions<BlobStorageClientSettings> options)
            => _blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);

        public async Task<bool> IsAccessible(CancellationToken cancellationToken = default)
            => await _blobContainerClient.IsAccessibleAsync(cancellationToken);

        public void Initialize()
        {
            if (_blobContainerClient.DoesNotExist())
            {
                _blobContainerClient.CreateIfNotExists();
            }
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            if (await _blobContainerClient.DoesNotExistAsync(cancellationToken))
            {
                await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            }
        }

        public async Task<BlobStorage> GetBlobStorage(string blobId, CancellationToken cancellationToken = default)
        {
            await Initialize(cancellationToken);

            return new BlobStorage(_blobContainerClient.GetBlobClient(blobId));
        }

        public async Task<ImmutableList<BlobStorage>> GetBlobStorageList(string prefix = null, CancellationToken cancellationToken = default)
        {
            await Initialize(cancellationToken);

            var blobItemList = new List<BlobItem>();
            foreach (var blobPage in _blobContainerClient.GetBlobs(BlobTraits.Metadata, BlobStates.None, prefix, cancellationToken).AsPages(pageSizeHint: 20))
            {
                var blobsToList = blobPage.Values.Where(IsBlobNotDeleted).ToList();
                blobItemList.AddRange(blobsToList);
            }

            var blobStorageList = blobItemList.Select(bi => new BlobStorage(_blobContainerClient.GetBlobClient(bi.Name))).ToImmutableList();

            return blobStorageList;
        }

        private static bool IsBlobNotDeleted(BlobItem blobItem)
        {
            var blobStorageMetadata = new BlobStorageMetadata(blobItem.Metadata);
            return blobStorageMetadata.IsNotDeleted();
        }
    }
}
