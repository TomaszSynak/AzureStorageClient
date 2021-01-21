namespace AzureStorageClient.MultiContainerClient.Components.SamuraiContainer
{
    public class SamuraiSettings : IAzureBlobClientSettings
    {
        public string ConnectionString { get; set; }

        public string ContainerName { get; set; }
    }
}
