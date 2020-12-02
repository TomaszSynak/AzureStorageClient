namespace AzureStorageClient
{
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.Options;

    internal class AzureBlob
    {
        private readonly BlobClient _blobClient;

        private AzureBlobMetadata _azureBlobMetadata;

        private BlobProperties _blobProperties;

        public AzureBlob(IOptions<AzureBlobClientSettings> options, string blobName)
        {
            _blobClient = new BlobClient(options.Value.ConnectionString, options.Value.ContainerName, blobName);
            _azureBlobMetadata = new AzureBlobMetadata();
        }

        public AzureBlob(BlobClient blobClient)
        {
            _blobClient = blobClient;
            _azureBlobMetadata = new AzureBlobMetadata();
        }

        public string BlobPath => _blobClient.Name;

        public async Task Upload<TStorable>(TStorable objectToUpsert, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            var blobStringContent = objectToUpsert.Serialize();

            var blobByteContent = blobStringContent.Encode();

            _azureBlobMetadata.SetIsDeleted(false);

            using (var memoryStream = new MemoryStream(blobByteContent.Length))
            {
                await memoryStream.WriteAsync(blobByteContent, 0, blobByteContent.Length, cancellationToken);
                memoryStream.Position = 0;

                await _blobClient.UploadAsync(memoryStream, AzureBlobHeadersFactory.Create(blobByteContent), _azureBlobMetadata.Metadata, cancellationToken: cancellationToken);
                await RefreshPropertiesAsync(cancellationToken);
            }
        }

        public async Task SetIsDeleted(bool isDeleted, CancellationToken cancellationToken = default)
        {
            var blobExists = await _blobClient.ExistsAsync(cancellationToken);
            if (blobExists.Value)
            {
                await RefreshPropertiesAsync(cancellationToken);

                _azureBlobMetadata.SetIsDeleted(isDeleted);

                await _blobClient.SetMetadataAsync(_azureBlobMetadata.Metadata, cancellationToken: cancellationToken);

                return;
            }

            throw BlobNotFoundException.Create(_blobClient.Name);
        }

        public async Task<TStorable> Download<TStorable>(CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable
        {
            var blobExists = await _blobClient.ExistsAsync(cancellationToken);
            if (blobExists.Value)
            {
                await RefreshPropertiesAsync(cancellationToken);

                if (_azureBlobMetadata.IsNotDeleted())
                {
                    var blobByteContent = new byte[_blobProperties.ContentLength];

                    using (var memoryStream = new MemoryStream(blobByteContent))
                    {
                        await _blobClient.DownloadToAsync(memoryStream, cancellationToken);

                        CheckBlobIntegrity(_blobProperties, blobByteContent);

                        var blobStringContent = blobByteContent.Decode();

                        return blobStringContent.Deserialize<TStorable>();
                    }
                }

                throw BlobDeletedException.Create(_blobClient.Name);
            }

            throw BlobNotFoundException.Create(_blobClient.Name);
        }

        public async Task Delete(DeleteSnapshotsOption? deleteSnapshotsOption = null, CancellationToken cancellationToken = default)
        {
            if (await _blobClient.ExistsAsync(cancellationToken))
            {
                await _blobClient.DeleteIfExistsAsync(deleteSnapshotsOption ?? DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5351:Do Not Use Broken Cryptographic Algorithms", Justification = "Verify blob integrity")]
        public void CheckBlobIntegrity(BlobProperties blobProperties, byte[] blobByteContent)
        {
            byte[] blobMd5Hash = null;
            using (var md5 = MD5.Create())
            {
                blobMd5Hash = md5.ComputeHash(blobByteContent);
            }

            for (int i = 0; i < blobMd5Hash.Length; i++)
            {
                if (blobProperties.ContentHash[i].Equals(blobMd5Hash[i]))
                {
                    continue;
                }

                throw BlobContentCorruptedException.Create(_blobClient.Name);
            }
        }

        private async Task RefreshPropertiesAsync(CancellationToken cancellationToken = default)
        {
            _blobProperties = (await _blobClient.GetPropertiesAsync(cancellationToken: cancellationToken)).Value;

            _azureBlobMetadata = new AzureBlobMetadata(_blobProperties.Metadata);
        }
    }
}
