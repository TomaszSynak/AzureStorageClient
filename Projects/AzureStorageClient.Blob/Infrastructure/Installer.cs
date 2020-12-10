[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AzureStorageClient.IntegrationTests")]

namespace AzureStorageClient
{
    using System;
    using System.Net;
    using System.Text;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class Installer
    {
        private const string SettingsSection = nameof(AzureBlobClientSettings);

        public static void AddAzureBlobClient(this IServiceCollection serviceCollection, IConfiguration configuration, string settingsSection = null)
        {
            // Enable TLS 1.2 before connecting to Azure Storage
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var configurationSection = configuration?.GetSection(settingsSection ?? SettingsSection)
                ?? throw new ArgumentNullException($"{SettingsSection} is missing from configuration.");

            serviceCollection
                .Configure<AzureBlobClientSettings>(configurationSection);

            serviceCollection
                .AddTransient(AzureBlobContainerFactory.Register);

            serviceCollection
                .AddTransient<IAzureBlobClient, AzureBlobClient>();
        }

        public static void InitializeAzureBlobClient(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder = applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));

            using (var serviceScope = applicationBuilder.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var blobContainer = serviceScope.ServiceProvider.GetService<AzureBlobContainer>();
                blobContainer.Initialize();
            }
        }
    }
}