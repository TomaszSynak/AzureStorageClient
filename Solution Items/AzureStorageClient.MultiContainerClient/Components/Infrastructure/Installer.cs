namespace AzureStorageClient.MultiContainerClient.Components.Infrastructure
{
    using Microsoft.Extensions.DependencyInjection;

    internal static class Installer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddTransient<ExternalResources>();
        }
    }
}
