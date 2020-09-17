namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using AzureStorageClient;

    internal class AzureBlobManager
    {
        private readonly AzureBlobClientSettings _blobStorageClientSettings;

        public AzureBlobManager() => _blobStorageClientSettings = ConfigurationProvider.GetBlobStorageClientSettings();

        public async Task SetUp(CancellationToken ct = default) => await SetUpBlobStorage(ct);

        public async Task CleanUp(CancellationToken ct = default) => await CleanUpBlobStorage(ct);

        private async Task SetUpBlobStorage(CancellationToken ct = default)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var blobContainerClient = new BlobContainerClient(_blobStorageClientSettings.ConnectionString, _blobStorageClientSettings.ContainerName);

            var doesNotExist = await blobContainerClient.DoesNotExistAsync(cancellationToken: ct);
            if (doesNotExist)
            {
                await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: ct);
            }
        }

        private async Task CleanUpBlobStorage(CancellationToken ct = default)
        {
            var blobContainerClient = new BlobContainerClient(_blobStorageClientSettings.ConnectionString, _blobStorageClientSettings.ContainerName);
            var containerExists = (await blobContainerClient.ExistsAsync(ct)).Value;
            if (containerExists)
            {
                await blobContainerClient.DeleteAsync(cancellationToken: ct);
            }

            // It takes ~40 sec for container in Azure to be deleted
            // await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}
