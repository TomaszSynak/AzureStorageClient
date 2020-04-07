namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using System.Text;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using AzureStorageClient;

    internal class BlobStorageManager
    {
        private readonly StorageClientSettings _storageClientSettings;

        public BlobStorageManager() => _storageClientSettings = ConfigurationProvider.GetStorageClientSettings();

        public async Task SetUp() => await SetUpBlobStorage();

        public async Task CleanUp() => await CleanUpBlobStorage();

        private async Task SetUpBlobStorage()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var blobContainerClient = new BlobContainerClient(_storageClientSettings.ConnectionString, _storageClientSettings.ContainerName);

            var doesNotExist = await blobContainerClient.DoesNotExistAsync();
            if (doesNotExist)
            {
                await blobContainerClient.CreateIfNotExistsAsync();
            }
        }

        private async Task CleanUpBlobStorage()
        {
            var blobContainerClient = new BlobContainerClient(_storageClientSettings.ConnectionString, _storageClientSettings.ContainerName);
            var containerExists = (await blobContainerClient.ExistsAsync()).Value;
            if (containerExists)
            {
                await blobContainerClient.DeleteAsync();
            }

            // It takes ~40 sec for container in Azure to be deleted
            // await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}
