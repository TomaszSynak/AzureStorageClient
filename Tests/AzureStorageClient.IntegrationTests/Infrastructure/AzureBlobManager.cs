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

        public async Task SetUp(CancellationToken cancellationToken = default) => await SetUpBlobStorage(cancellationToken);

        public async Task CleanUp(CancellationToken cancellationToken = default) => await CleanUpBlobStorage(cancellationToken);

        private async Task SetUpBlobStorage(CancellationToken cancellationToken = default)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var blobContainerClient = new BlobContainerClient(_blobStorageClientSettings.ConnectionString, _blobStorageClientSettings.ContainerName);

            var doesNotExist = await blobContainerClient.DoesNotExistAsync(cancellationToken: cancellationToken);
            if (doesNotExist)
            {
                await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            }
        }

        private async Task CleanUpBlobStorage(CancellationToken cancellationToken = default)
        {
            var blobContainerClient = new BlobContainerClient(_blobStorageClientSettings.ConnectionString, _blobStorageClientSettings.ContainerName);
            var containerExists = (await blobContainerClient.ExistsAsync(cancellationToken)).Value;
            if (containerExists)
            {
                await blobContainerClient.DeleteAsync(cancellationToken: cancellationToken);
            }

            // It takes ~40 sec for container in Azure to be deleted
            // await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}
