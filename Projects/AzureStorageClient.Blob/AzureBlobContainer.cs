namespace AzureStorageClient
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;

    internal class AzureBlobContainer
    {
        private readonly BlobContainerClient _blobContainerClient;

        private bool _isInitialized;

        public AzureBlobContainer(IAzureBlobClientSettings options, bool? isInitialized = false)
        {
            _blobContainerClient = new BlobContainerClient(options.ConnectionString, options.ContainerName);
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

        public async Task<ImmutableList<TStorable>> GetAzureBlobFolder<TStorable>(string prefix = null, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            await Initialize(cancellationToken);

            async Task<IReadOnlyList<TStorable>> GetBlobs(IReadOnlyList<BlobItem> blobItemList, CancellationToken ct = default)
            {
                var downloadingAzureBlobs = blobItemList
                    .Where(bi => bi.IsBlobNotDeleted())
                    .Select(bi => CreateAzureBlob(bi.Name))
                    .Select(ab => ab.Download<TStorable>(ct))
                    .ToList();

                await Task.WhenAll(downloadingAzureBlobs);

                return downloadingAzureBlobs.Select(d => d.Result).ToList();
            }

            return await ActionOnBlobs(GetBlobs, prefix, cancellationToken);
        }

        public async Task DeleteAzureBlobFolder(string prefix = null, CancellationToken cancellationToken = default)
        {
            await Initialize(cancellationToken);

            async Task DeleteBlobs(IReadOnlyList<BlobItem> blobItemList, CancellationToken ct = default)
            {
                var deletingAzureBlobs = blobItemList.Select(bi => CreateAzureBlob(bi.Name).Delete(cancellationToken: cancellationToken)).ToList();

                await Task.WhenAll(deletingAzureBlobs);
            }

            await ActionOnBlobs(DeleteBlobs, prefix, cancellationToken);
        }

        private AzureBlob CreateAzureBlob(string blobId)
        {
            return new AzureBlob(_blobContainerClient.GetBlobClient(blobId));
        }

        private async Task ActionOnBlobs(Func<IReadOnlyList<BlobItem>, CancellationToken, Task> actionToPerform, string prefix = null, CancellationToken cancellationToken = default)
        {
            // Azure Blob storage does not have a concept of folders and everything inside the container is considered a blob including the folders.
            // https://stackoverflow.com/a/34737858/5808148
            try
            {
                string continuationToken = null;
                do
                {
                    var blobPageSegment = _blobContainerClient
                        .GetBlobs(BlobTraits.Metadata, BlobStates.None, prefix, cancellationToken)
                        .AsPages(continuationToken, pageSizeHint: 50);

                    foreach (var blobPage in blobPageSegment)
                    {
                        await actionToPerform.Invoke(blobPage.Values, cancellationToken);
                        continuationToken = blobPage.ContinuationToken;
                    }
                }
                while (!string.IsNullOrEmpty(continuationToken));
            }
            catch (RequestFailedException exception)
            {
                throw new BlobContainerException($"Failed to delete folder's {prefix} content.", exception);
            }
        }

        private async Task<ImmutableList<T>> ActionOnBlobs<T>(Func<IReadOnlyList<BlobItem>, CancellationToken, Task<IReadOnlyList<T>>> actionToPerform, string prefix = null, CancellationToken cancellationToken = default)
        {
            // Azure Blob storage does not have a concept of folders and everything inside the container is considered a blob including the folders.
            // https://stackoverflow.com/a/34737858/5808148
            try
            {
                string continuationToken = null;
                var blobItemList = new List<T>();
                do
                {
                    var blobPageSegment = _blobContainerClient
                        .GetBlobs(BlobTraits.Metadata, BlobStates.None, prefix, cancellationToken)
                        .AsPages(continuationToken, pageSizeHint: 50);

                    foreach (var blobPage in blobPageSegment)
                    {
                        var pageResults = await actionToPerform.Invoke(blobPage.Values, cancellationToken);
                        blobItemList.AddRange(pageResults);
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
