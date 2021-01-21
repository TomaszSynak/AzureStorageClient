namespace AzureStorageClient.MultiContainerClient.Components.NinjaContainer
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    internal static class Installer
    {
        public static IServiceCollection AddNinjaComponent(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddAzureBlobClient<NinjaSettings>(configuration);

            return serviceCollection;
        }
    }
}
