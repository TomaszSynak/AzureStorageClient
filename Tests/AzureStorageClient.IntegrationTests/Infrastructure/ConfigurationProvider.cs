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

        public static TableStorageClientSettings GetTableStorageClientSettings()
        {
            var storageClientSettings = new TableStorageClientSettings();
            GetConfigurationRoot().GetSection(nameof(TableStorageClientSettings)).Bind(storageClientSettings);
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
