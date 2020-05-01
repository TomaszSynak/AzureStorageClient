namespace AzureStorageClient
{
    using Microsoft.Extensions.Options;

    internal static class BlobStorageContainerFactory
    {
        public static BlobStorageContainer Create(IOptions<BlobStorageClientSettings> options)
        {
            var blobStorageContainer = new BlobStorageContainer(options);
            blobStorageContainer.Initialize();
            return blobStorageContainer;
        }
    }
}
