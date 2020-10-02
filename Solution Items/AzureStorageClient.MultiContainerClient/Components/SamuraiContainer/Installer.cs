namespace AzureStorageClient.MultiContainerClient.Components.SamuraiContainer
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    internal static class Installer
    {
        private const string BlobSettingsName = "SamuraiBlobSettings";

        public static IServiceCollection AddSamuraiComponent(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddAzureBlobClient(configuration, BlobSettingsName);

            return serviceCollection;
        }
    }
}
