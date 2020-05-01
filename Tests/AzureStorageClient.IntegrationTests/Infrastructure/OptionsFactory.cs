namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using AzureStorageClient;
    using Microsoft.Extensions.Options;
    using Moq;

    internal class OptionsFactory
    {
        public static IOptions<BlobStorageClientSettings> Create(string containerName = null)
        {
            var optionMock = new Mock<IOptions<BlobStorageClientSettings>>();

            var settings = ConfigurationProvider.GetStorageClientSettings();
            settings.ContainerName = containerName ?? settings.ContainerName;

            optionMock
                .Setup(o => o.Value)
                .Returns(settings);

            return optionMock.Object;
        }
    }
}
