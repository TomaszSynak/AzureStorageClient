namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    public class AzureBlobClientSettings : IAzureBlobClientSettings
    {
        public string ConnectionString { get; set; }

        public string ContainerName { get; set; }
    }
}
