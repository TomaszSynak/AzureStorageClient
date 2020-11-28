namespace AzureStorageClient
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.Options;

    internal class AzureBlobContainer
    {
        private readonly BlobContainerClient _blobContainerClient;

        private bool _isInitialized;

        public AzureBlobContainer(IOptions<AzureBlobClientSettings> options, bool? isInitialized = false)
        {
            _blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);
            _isInitialized = isInitialized ?? false;
        }

        public async Task<bool> IsAccessible(CancellationToken cancellationToken = default)
            => await _blobContainerClient.IsAccessibleAsync(cancellationToken);

        public void Initialize()
        {
            if (_isInitialized || _blobContainerClient.Exists())
            {
                _isInitialized = true;
                return;
            }

            _blobContainerClient.CreateIfNotExists();
            _isInitialized = true;
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            if (_isInitialized || await _blobContainerClient.ExistsAsync(cancellationToken))
            {
                _isInitialized = true;
                return;
            }

            await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            _isInitialized = true;
        }

        public async Task<AzureBlob> GetAzureBlob(string blobId, CancellationToken cancellationToken = default)
        {
            await Initialize(cancellationToken);

            return CreateAzureBlob(blobId);
        }

        public async Task<ImmutableList<AzureBlob>> GetAzureBlobList(string prefix = null, CancellationToken cancellationToken = default)
        {
            await Initialize(cancellationToken);

            var blobItemList = FetchBlobs(prefix, cancellationToken);

            var azureBlobList = blobItemList.Where(IsBlobNotDeleted).Select(bi => CreateAzureBlob(bi.Name)).ToImmutableList();

            return azureBlobList;
        }

        public async Task DeleteAzureBlobFolder(string prefix = null, CancellationToken cancellationToken = default)
        {
            await Initialize(cancellationToken);

            var blobItemList = FetchBlobs(prefix, cancellationToken);

            var deletingAzureBlobs = blobItemList.Select(bi => CreateAzureBlob(bi.Name).Delete(cancellationToken: cancellationToken)).ToList();

            await Task.WhenAll(deletingAzureBlobs);
        }

        private static bool IsBlobNotDeleted(BlobItem blobItem)
        {
            var azureBlobMetadata = new AzureBlobMetadata(blobItem.Metadata);
            return azureBlobMetadata.IsNotDeleted();
        }

        private AzureBlob CreateAzureBlob(string blobId)
        {
            return new AzureBlob(_blobContainerClient.GetBlobClient(blobId));
        }

        private ImmutableList<BlobItem> FetchBlobs(string prefix = null, CancellationToken cancellationToken = default)
        {
            // Azure Blob storage does not have a concept of folders and everything inside the container is considered a blob including the folders.
            // https://stackoverflow.com/a/34737858/5808148
            try
            {
                string continuationToken = null;
                var blobItemList = new List<BlobItem>();
                do
                {
                    var blobPageSegment = _blobContainerClient
                        .GetBlobs(BlobTraits.Metadata, BlobStates.None, prefix, cancellationToken)
                        .AsPages(continuationToken, pageSizeHint: 20);

                    foreach (var blobPage in blobPageSegment)
                    {
                        blobItemList.AddRange(blobPage.Values);
                        continuationToken = blobPage.ContinuationToken;
                    }
                }
                while (!string.IsNullOrEmpty(continuationToken));

                return blobItemList.ToImmutableList();
            }
            catch (RequestFailedException exception)
            {
                throw new BlobContainerException($"Failed to delete folder's {prefix} content.", exception);
            }
        }
    }
}
