namespace AzureStorageClient.IntegrationTests
{
    using System;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Infrastructure;
    using Xunit;

    [Collection(nameof(IntegrationTests))]
    public class BlobStorageContainerTests
    {
        [Fact]
        public async Task GetBlobStorage_ContainerDoesNotExists_ContainerCreated()
        {
            // ToDo: use create resource attribute
            // Arrange
            var options = OptionsFactory.Create(Guid.NewGuid().ToString("D"));
            var blobStorageContainer = new BlobStorageContainer(options);
            var blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);

            var containerExists = (await blobContainerClient.ExistsAsync()).Value;
            if (containerExists)
            {
                await blobContainerClient.DeleteAsync();
            }

            // Act
            await blobStorageContainer.GetBlobStorage(Guid.NewGuid().ToString("D"));

            // Assert
            containerExists = (await blobContainerClient.ExistsAsync()).Value;
            Assert.True(containerExists);

            // Clean up
            await blobContainerClient.DeleteAsync();
        }

        [Fact]
        [CleanBlobStorage]
        public async Task GetBlobStorage_ContainerExists_ContainerCreated()
        {
            // Arrange
            var options = OptionsFactory.Create();
            var blobStorageContainer = new BlobStorageContainer(options);
            var blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);

            // Act
            await blobStorageContainer.GetBlobStorage(Guid.NewGuid().ToString("D"));

            // Assert
            var containerExists = (await blobContainerClient.ExistsAsync()).Value;
            Assert.True(containerExists);
        }

        [Fact]
        public async Task GetBlobStorageList_ContainerExists_GetContainerContent()
        {
            // Arrange
            var options = OptionsFactory.Create(containerName: "some-mock-container");
            var blobStorageContainer = new BlobStorageContainer(options);
            var blobStorage = await blobStorageContainer.GetBlobStorage(Guid.NewGuid().ToString("D"));
            await blobStorage.Upload("Some mock blob content");

            // Act
            var blobStorageList = await blobStorageContainer.GetBlobStorageList();

            // Assert
            Assert.NotEmpty(blobStorageList);
            Assert.Single(blobStorageList);

            // Clean up
            var blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);
            var containerExists = (await blobContainerClient.ExistsAsync()).Value;
            if (containerExists)
            {
                await blobContainerClient.DeleteAsync();
            }
        }

        [Fact]
        public async Task GetBlobStorageList_ContainerContainsDeletedBlob_GetEmptyContent()
        {
            // Arrange
            var options = OptionsFactory.Create(containerName: "some-mock-container");
            var blobStorageContainer = new BlobStorageContainer(options);

            var blobName = Guid.NewGuid().ToString("D");

            var blobStorage = await blobStorageContainer.GetBlobStorage(blobName);
            await blobStorage.Upload("Some mock blob content");

            var blobClient = new BlobClient(options.Value.ConnectionString, options.Value.ContainerName, blobName);
            var blobStorageMetadata = new BlobStorageMetadata();
            blobStorageMetadata.SetIsDeleted(true);
            await blobClient.SetMetadataAsync(blobStorageMetadata.Metadata);

            // Act
            var blobStorageList = await blobStorageContainer.GetBlobStorageList();

            // Assert
            Assert.Empty(blobStorageList);

            // Clean up
            var blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);
            var containerExists = (await blobContainerClient.ExistsAsync()).Value;
            if (containerExists)
            {
                await blobContainerClient.DeleteAsync();
            }
        }
    }
}
