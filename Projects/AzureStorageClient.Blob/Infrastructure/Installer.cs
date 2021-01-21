[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AzureStorageClient.IntegrationTests")]

namespace AzureStorageClient
{
    using System;
    using System.Net;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class Installer
    {
        public static void AddAzureBlobClient<TSettings>(this IServiceCollection serviceCollection, IConfiguration configuration)
            where TSettings : class, IAzureBlobClientSettings, new()
        {
            // Enable TLS 1.2 before connecting to Azure Storage
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var configurationSection = configuration?.GetSection(typeof(TSettings).Name)
                ?? throw new ArgumentNullException($"{typeof(TSettings).Name} is missing from configuration.");

            serviceCollection
                .Configure<TSettings>(configurationSection);

            serviceCollection
                .AddTransient(typeof(IAzureBlobClient<TSettings>), AzureBlobClientFactory.Create<TSettings>);
        }

        // public static void InitializeAzureBlobClient<TClient>(this IApplicationBuilder applicationBuilder)
        //    where TClient : IAzureBlobClient
        // {
        //    applicationBuilder = applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));
        //    using (var serviceScope = applicationBuilder.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        //    {
        //        var blobContainer = serviceScope.ServiceProvider.GetService<AzureBlobContainer<TClient>>();
        //        blobContainer.Initialize();
        //    }
        // }
    }
}