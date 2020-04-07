namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using AzureStorageClient;
    using Microsoft.Extensions.Options;
    using Moq;

    internal class OptionsFactory
    {
        public static IOptions<StorageClientSettings> Create(string containerName = null)
        {
            var optionMock = new Mock<IOptions<StorageClientSettings>>();

            var settings = ConfigurationProvider.GetStorageClientSettings();
            settings.ContainerName = containerName ?? settings.ContainerName;

            optionMock
                .Setup(o => o.Value)
                .Returns(settings);

            return optionMock.Object;
        }
    }
}
