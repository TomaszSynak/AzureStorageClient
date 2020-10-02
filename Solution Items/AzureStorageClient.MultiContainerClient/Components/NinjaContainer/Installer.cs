namespace AzureStorageClient.MultiContainerClient.Components.NinjaContainer
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    internal static class Installer
    {
        private const string BlobSettingsName = "NinjaBlobSettings";

        public static IServiceCollection AddNinjaComponent(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddAzureBlobClient(configuration, BlobSettingsName);

            return serviceCollection;
        }
    }
}
