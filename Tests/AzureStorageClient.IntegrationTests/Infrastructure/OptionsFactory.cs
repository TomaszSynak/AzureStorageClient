namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using AzureStorageClient;
    using Microsoft.Extensions.Options;
    using Moq;

    internal class OptionsFactory
    {
        public static IOptions<BlobStorageClientSettings> CreateBlobSettings(string containerName = null)
        {
            var optionMock = new Mock<IOptions<BlobStorageClientSettings>>();

            var settings = ConfigurationProvider.GetBlobStorageClientSettings();
            settings.ContainerName = containerName ?? settings.ContainerName;

            optionMock
                .Setup(o => o.Value)
                .Returns(settings);

            return optionMock.Object;
        }

        public static IOptions<TableStorageClientSettings> CreateTableSettings(string containerName = null)
        {
            var optionMock = new Mock<IOptions<TableStorageClientSettings>>();

            var settings = ConfigurationProvider.GetTableStorageClientSettings();
            settings.ContainerName = containerName ?? settings.ContainerName;

            optionMock
                .Setup(o => o.Value)
                .Returns(settings);

            return optionMock.Object;
        }
    }
}
