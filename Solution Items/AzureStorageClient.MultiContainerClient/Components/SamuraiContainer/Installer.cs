namespace AzureStorageClient.MultiContainerClient.Components.SamuraiContainer
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    internal static class Installer
    {
        public static IServiceCollection AddSamuraiComponent(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddAzureBlobClient<SamuraiSettings>(configuration);

            return serviceCollection;
        }
    }
}
