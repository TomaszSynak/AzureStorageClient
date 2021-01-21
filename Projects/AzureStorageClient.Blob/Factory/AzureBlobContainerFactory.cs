namespace AzureStorageClient
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    internal static class AzureBlobContainerFactory
    {
        private static readonly IDictionary<string, bool> ContainerInitStatus = new ConcurrentDictionary<string, bool>();

        public static AzureBlobContainer Create<TSettings>(IServiceProvider serviceProvider)
            where TSettings : class, IAzureBlobClientSettings, new()
        {
            var settings = serviceProvider.GetService<IOptions<TSettings>>();

            var azureBlobContainer = new AzureBlobContainer(settings.Value, IsInitialized<TSettings>());

            if (IsInitialized<TSettings>())
            {
                return azureBlobContainer;
            }

            azureBlobContainer.Initialize();

            MarkAsInitialized<TSettings>();

            return azureBlobContainer;
        }

        private static bool IsInitialized<TSettings>()
            where TSettings : class, IAzureBlobClientSettings, new()
        {
            if (ContainerInitStatus.TryGetValue(typeof(TSettings).Name, out var isInitialized))
            {
                return isInitialized;
            }

            ContainerInitStatus.Add(typeof(TSettings).Name, isInitialized);

            return isInitialized;
        }

        private static void MarkAsInitialized<TSettings>()
            where TSettings : class, IAzureBlobClientSettings, new()
        {
            if (ContainerInitStatus.ContainsKey(typeof(TSettings).Name))
            {
                ContainerInitStatus[typeof(TSettings).Name] = true;
                return;
            }

            ContainerInitStatus.Add(typeof(TSettings).Name, true);
        }
    }
}
