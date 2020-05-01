[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AzureStorageClient.IntegrationTests")]

namespace AzureStorageClient
{
    using System;
    using System.Net;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class TableStorageClientConfiguration
    {
        private const string SettingsSection = nameof(TableStorageClientSettings);

        public static void AddTableStorageClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Enable TLS 1.2 before connecting to Azure Storage
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var configurationSection = configuration.GetSection(SettingsSection)
                     ?? throw new ArgumentNullException($"{SettingsSection} is missing from configuration.");

            serviceCollection
                .Configure<TableStorageClientSettings>(configurationSection);

            serviceCollection
                .AddTransient<IStorageClient, TableStorageClient>()
                .AddTransient<ITableStorageClient, TableStorageClient>();
        }
    }
}
