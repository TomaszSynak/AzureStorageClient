﻿[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AzureStorageClient.IntegrationTests")]

namespace AzureStorageClient
{
    using System;
    using System.Net;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class AzureBlobClientConfiguration
    {
        private const string SettingsSection = nameof(AzureBlobClientSettings);

        public static void AddAzureBlobClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Enable TLS 1.2 before connecting to Azure Storage
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var configurationSection = configuration.GetSection(SettingsSection)
                     ?? throw new ArgumentNullException($"{SettingsSection} is missing from configuration.");

            serviceCollection
                .Configure<AzureBlobClientSettings>(configurationSection);

            serviceCollection
                .AddTransient<IAzureBlobClient, AzureBlobClient>();
        }
    }
}