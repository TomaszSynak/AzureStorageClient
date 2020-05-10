namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using System.IO;
    using AzureStorageClient;
    using Microsoft.Extensions.Configuration;

    internal static class ConfigurationProvider
    {
        private static IConfigurationRoot _configurationRoot;

        public static AzureBlobClientSettings GetBlobStorageClientSettings()
        {
            var storageClientSettings = new AzureBlobClientSettings();
            GetConfigurationRoot().GetSection(nameof(AzureBlobClientSettings)).Bind(storageClientSettings);
            return storageClientSettings;
        }

        public static AzureTableClientSettings GetTableStorageClientSettings()
        {
            var storageClientSettings = new AzureTableClientSettings();
            GetConfigurationRoot().GetSection(nameof(AzureTableClientSettings)).Bind(storageClientSettings);
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
