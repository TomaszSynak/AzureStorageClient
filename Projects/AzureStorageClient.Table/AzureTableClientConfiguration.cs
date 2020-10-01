[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AzureStorageClient.IntegrationTests")]

namespace AzureStorageClient
{
    using System;
    using System.Net;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class AzureTableClientConfiguration
    {
        private const string SettingsSection = nameof(AzureTableClientSettings);

        public static void AddAzureTableClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Enable TLS 1.2 before connecting to Azure Storage
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var configurationSection = configuration?.GetSection(SettingsSection)
                     ?? throw new ArgumentNullException($"{SettingsSection} is missing from configuration.");

            serviceCollection
                .Configure<AzureTableClientSettings>(configurationSection);

            serviceCollection
                .AddTransient<IAzureTableClient, AzureTableClient>();
        }
    }
}
