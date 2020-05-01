[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AzureStorageClient.IntegrationTests")]

namespace AzureStorageClient
{
    using System;
    using System.Net;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class StorageClientConfiguration
    {
        private const string StorageClientSettingsSection = nameof(StorageClientSettings);

        public static void AddStorageClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Enable TLS 1.2 before connecting to Azure Storage
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var configurationSection = configuration.GetSection(StorageClientSettingsSection)
                     ?? throw new ArgumentNullException($"{StorageClientSettingsSection} is missing from configuration.");

            serviceCollection
                .Configure<StorageClientSettings>(configurationSection);

            serviceCollection
                .AddTransient<IStorageClient, StorageClient>();
        }
    }
}
