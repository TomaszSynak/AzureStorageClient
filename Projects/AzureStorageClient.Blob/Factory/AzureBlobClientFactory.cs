namespace AzureStorageClient
{
    using System;

    internal class AzureBlobClientFactory
    {
        public static AzureBlobClient<TSettings> Create<TSettings>(IServiceProvider serviceProvider)
            where TSettings : class, IAzureBlobClientSettings, new()
        {
            var container = AzureBlobContainerFactory.Create<TSettings>(serviceProvider);

            var azureBlobContainer = new AzureBlobClient<TSettings>(container);

            return azureBlobContainer;
        }
    }
}
