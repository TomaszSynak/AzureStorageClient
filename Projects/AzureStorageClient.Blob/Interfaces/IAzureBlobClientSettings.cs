namespace AzureStorageClient
{
    public interface IAzureBlobClientSettings
    {
        string ConnectionString { get; set; }

        string ContainerName { get; set; }
    }
}
