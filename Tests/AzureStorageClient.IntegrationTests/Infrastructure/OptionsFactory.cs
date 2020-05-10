namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using AzureStorageClient;
    using Microsoft.Extensions.Options;
    using Moq;

    internal class OptionsFactory
    {
        public static IOptions<AzureBlobClientSettings> CreateBlobSettings(string containerName = null)
        {
            var optionMock = new Mock<IOptions<AzureBlobClientSettings>>();

            var settings = ConfigurationProvider.GetBlobStorageClientSettings();
            settings.ContainerName = containerName ?? settings.ContainerName;

            optionMock
                .Setup(o => o.Value)
                .Returns(settings);

            return optionMock.Object;
        }

        public static IOptions<AzureTableClientSettings> CreateTableSettings()
        {
            var optionMock = new Mock<IOptions<AzureTableClientSettings>>();

            var settings = ConfigurationProvider.GetTableStorageClientSettings();

            optionMock
                .Setup(o => o.Value)
                .Returns(settings);

            return optionMock.Object;
        }
    }
}
