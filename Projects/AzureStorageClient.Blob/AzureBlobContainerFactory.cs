namespace AzureStorageClient
{
    using Microsoft.Extensions.Options;

    internal static class AzureBlobContainerFactory
    {
        public static AzureBlobContainer Create(IOptions<AzureBlobClientSettings> options)
        {
            var azureBlobContainer = new AzureBlobContainer(options);
            azureBlobContainer.Initialize();
            return azureBlobContainer;
        }
    }
}
