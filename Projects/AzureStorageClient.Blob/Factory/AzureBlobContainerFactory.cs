namespace AzureStorageClient
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    internal static class AzureBlobContainerFactory
    {
        private static bool _isInitialized;

        public static AzureBlobContainer Register(IServiceProvider serviceProvider)
        {
            var azureBlobContainer = new AzureBlobContainer(serviceProvider.GetService<IOptions<AzureBlobClientSettings>>(), _isInitialized);

            azureBlobContainer.Initialize();

            _isInitialized = true;

            return azureBlobContainer;
        }
    }
}
