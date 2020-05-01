namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using System.IO;
    using AzureStorageClient;
    using Microsoft.Extensions.Configuration;

    internal static class ConfigurationProvider
    {
        private static IConfigurationRoot _configurationRoot;

        public static BlobStorageClientSettings GetStorageClientSettings()
        {
            var storageClientSettings = new BlobStorageClientSettings();
            GetConfigurationRoot().GetSection(nameof(BlobStorageClientSettings)).Bind(storageClientSettings);
            return storageClientSettings;
        }

        private static IConfigurationRoot GetConfigurationRoot()
        {
            return _configurationRoot ?? (_configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", true, true)
                .Build());
        }
    }
}
