[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AzureStorageClient.IntegrationTests")]

namespace AzureStorageClient
{
    using System;
    using System.Net;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class BlobStorageClientConfiguration
    {
        private const string SettingsSection = nameof(BlobStorageClientSettings);

        public static void AddBlobStorageClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Enable TLS 1.2 before connecting to Azure Storage
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var configurationSection = configuration.GetSection(SettingsSection)
                     ?? throw new ArgumentNullException($"{SettingsSection} is missing from configuration.");

            serviceCollection
                .Configure<BlobStorageClientSettings>(configurationSection);

            serviceCollection
                .AddTransient<IBlobStorageClient, BlobStorageClient>();
        }
    }
}
