namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using System.IO;
    using AzureStorageClient;
    using Microsoft.Extensions.Configuration;

    internal static class ConfigurationProvider
    {
        private static IConfigurationRoot _configurationRoot;

        public static StorageClientSettings GetStorageClientSettings()
        {
            var storageClientSettings = new StorageClientSettings();
            GetConfigurationRoot().GetSection(nameof(StorageClientSettings)).Bind(storageClientSettings);
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
