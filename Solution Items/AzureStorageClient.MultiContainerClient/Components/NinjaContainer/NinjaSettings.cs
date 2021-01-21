namespace AzureStorageClient.MultiContainerClient.Components.NinjaContainer
{
    public class NinjaSettings : IAzureBlobClientSettings
    {
        public string ConnectionString { get; set; }

        public string ContainerName { get; set; }
    }
}
