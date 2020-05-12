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

    internal class AzureBlobContainer
    {
        private readonly BlobContainerClient _blobContainerClient;

        public AzureBlobContainer(IOptions<AzureBlobClientSettings> options)
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

        public async Task<AzureBlob> GetAzureBlob(string blobId, CancellationToken cancellationToken = default)
        {
            await Initialize(cancellationToken);

            return new AzureBlob(_blobContainerClient.GetBlobClient(blobId));
        }

        public async Task<ImmutableList<AzureBlob>> GetAzureBlobList(string prefix = null, CancellationToken cancellationToken = default)
        {
            await Initialize(cancellationToken);

            var blobItemList = new List<BlobItem>();
            foreach (var blobPage in _blobContainerClient.GetBlobs(BlobTraits.Metadata, BlobStates.None, prefix, cancellationToken).AsPages(pageSizeHint: 20))
            {
                var blobsToList = blobPage.Values.Where(IsBlobNotDeleted).ToList();
                blobItemList.AddRange(blobsToList);
            }

            var azureBlobList = blobItemList.Select(bi => new AzureBlob(_blobContainerClient.GetBlobClient(bi.Name))).ToImmutableList();

            return azureBlobList;
        }

        private static bool IsBlobNotDeleted(BlobItem blobItem)
        {
            var azureBlobMetadata = new AzureBlobMetadata(blobItem.Metadata);
            return azureBlobMetadata.IsNotDeleted();
        }
    }
}
