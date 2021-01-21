namespace AzureStorageClient
{
    using System;

    internal static class AzureBlobClientSettingsExtensions
    {
        public static void IsValid<TSettings>(this IAzureBlobClientSettings azureBlobClientSettings)
            where TSettings : class, IAzureBlobClientSettings, new()
        {
            if (string.IsNullOrWhiteSpace(azureBlobClientSettings.ConnectionString))
            {
                throw new ArgumentException($"{typeof(TSettings).Name}'s ConnectionString is either null or empty.");
            }

            if (string.IsNullOrWhiteSpace(azureBlobClientSettings.ContainerName))
            {
                throw new ArgumentException($"{typeof(TSettings).Name}'s ContainerName is either null or empty.");
            }
        }
    }
}
