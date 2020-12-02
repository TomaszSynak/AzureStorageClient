namespace AzureStorageClient.IntegrationTests.AzureBlobClient
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Infrastructure;
    using Microsoft.Extensions.Options;
    using Xunit;

    [Collection(nameof(IntegrationTests))]
    public class AzureBlobContainerTests
    {
        [Fact]
        public async Task GetBlobStorage_ContainerDoesNotExists_ContainerCreated()
        {
            // ToDo: use create resource attribute
            // Arrange
            var options = OptionsFactory.CreateBlobSettings(containerName: $"some-mock-container-{Guid.NewGuid()}");
            var blobStorageContainer = new AzureBlobContainer(options);
            var blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);

            var containerExists = (await blobContainerClient.ExistsAsync()).Value;
            if (containerExists)
            {
                await blobContainerClient.DeleteAsync();
            }

            // Act
            await blobStorageContainer.GetAzureBlob(Guid.NewGuid().ToString("D"));

            // Assert
            containerExists = (await blobContainerClient.ExistsAsync()).Value;
            Assert.True(containerExists);

            // Clean up
            await CleanUp(options);
        }

        [Fact]
        [CleanAzureBlob]
        public async Task GetBlobStorage_ContainerExists_ContainerCreated()
        {
            // Arrange
            var options = OptionsFactory.CreateBlobSettings(containerName: $"some-mock-container-{Guid.NewGuid()}");
            var blobStorageContainer = new AzureBlobContainer(options);
            var blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);

            // Act
            await blobStorageContainer.GetAzureBlob(Guid.NewGuid().ToString("D"));

            // Assert
            var containerExists = (await blobContainerClient.ExistsAsync()).Value;
            Assert.True(containerExists);

            // Clean up
            await CleanUp(options);
        }

        [Fact]
        public async Task GetBlobStorageList_LargeFolder_BlobsFetchedInBatches()
        {
            // Arrange
            var options = OptionsFactory.CreateBlobSettings(containerName: $"some-mock-container-{Guid.NewGuid()}");
            var blobStorageContainer = new AzureBlobContainer(options);
            for (int i = 0; i < 100; i++)
            {
                var blobStorage = await blobStorageContainer.GetAzureBlob($"LargeFolder/Blob-{i}");
                await blobStorage.Upload(TestModelFactory.Create().testModel);
            }

            // Act
            var blobStorageList = await blobStorageContainer.GetAzureBlobFolder<TestModel>("LargeFolder");

            // Assert
            Assert.NotEmpty(blobStorageList);
            Assert.Equal(100, blobStorageList.Count);

            // Clean up
            await CleanUp(options);
        }

        [Fact]
        public async Task GetBlobStorageList_ContainerContainsDeletedBlob_GetEmptyContent()
        {
            // Arrange
            var options = OptionsFactory.CreateBlobSettings(containerName: $"some-mock-container-{Guid.NewGuid()}");
            var blobStorageContainer = new AzureBlobContainer(options);

            var blobName = Guid.NewGuid().ToString("D");

            var blobStorage = await blobStorageContainer.GetAzureBlob(blobName);
            await blobStorage.Upload(TestModelFactory.Create().testModel);

            var blobClient = new BlobClient(options.Value.ConnectionString, options.Value.ContainerName, blobName);
            var blobStorageMetadata = new AzureBlobMetadata();
            blobStorageMetadata.SetIsDeleted(true);
            await blobClient.SetMetadataAsync(blobStorageMetadata.Metadata);

            // Act
            var blobStorageList = await blobStorageContainer.GetAzureBlobFolder<TestModel>();

            // Assert
            Assert.Empty(blobStorageList);

            // Clean up
            await CleanUp(options);
        }

        [Fact]
        public async Task DeleteAzureBlobFolder_LargeFolder_BlobsDeletedInBatches()
        {
            // Arrange
            var options = OptionsFactory.CreateBlobSettings(containerName: $"some-mock-container-{Guid.NewGuid()}");
            var blobStorageContainer = new AzureBlobContainer(options);
            for (int i = 0; i < 100; i++)
            {
                var blobStorage = await blobStorageContainer.GetAzureBlob($"LargeFolder/Blob-{i}");
                await blobStorage.Upload(TestModelFactory.Create().testModel);
            }

            // Act
            await blobStorageContainer.DeleteAzureBlobFolder("LargeFolder");

            // Assert
            var blobStorageList = await blobStorageContainer.GetAzureBlobFolder<TestModel>("LargeFolder");
            Assert.Empty(blobStorageList);

            // Clean up
            await CleanUp(options);
        }

        private static async Task CleanUp(IOptions<AzureBlobClientSettings> options, CancellationToken cancellationToken = default)
        {
            var blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);
            var containerExists = (await blobContainerClient.ExistsAsync(cancellationToken)).Value;
            if (containerExists)
            {
                await blobContainerClient.DeleteAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
