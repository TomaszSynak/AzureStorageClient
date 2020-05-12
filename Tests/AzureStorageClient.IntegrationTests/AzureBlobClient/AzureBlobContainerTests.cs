namespace AzureStorageClient.IntegrationTests.AzureBlobClient
{
    using System;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Infrastructure;
    using Xunit;

    [Collection(nameof(IntegrationTests))]
    public class AzureBlobContainerTests
    {
        [Fact]
        public async Task GetBlobStorage_ContainerDoesNotExists_ContainerCreated()
        {
            // ToDo: use create resource attribute
            // Arrange
            var options = OptionsFactory.CreateBlobSettings(Guid.NewGuid().ToString("D"));
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
            await blobContainerClient.DeleteAsync();
        }

        [Fact]
        [CleanAzureBlob]
        public async Task GetBlobStorage_ContainerExists_ContainerCreated()
        {
            // Arrange
            var options = OptionsFactory.CreateBlobSettings();
            var blobStorageContainer = new AzureBlobContainer(options);
            var blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);

            // Act
            await blobStorageContainer.GetAzureBlob(Guid.NewGuid().ToString("D"));

            // Assert
            var containerExists = (await blobContainerClient.ExistsAsync()).Value;
            Assert.True(containerExists);
        }

        [Fact]
        public async Task GetBlobStorageList_ContainerExists_GetContainerContent()
        {
            // Arrange
            var options = OptionsFactory.CreateBlobSettings(containerName: "some-mock-container");
            var blobStorageContainer = new AzureBlobContainer(options);
            var blobStorage = await blobStorageContainer.GetAzureBlob(Guid.NewGuid().ToString("D"));
            await blobStorage.Upload("Some mock blob content");

            // Act
            var blobStorageList = await blobStorageContainer.GetAzureBlobList();

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
            var options = OptionsFactory.CreateBlobSettings(containerName: "some-mock-container");
            var blobStorageContainer = new AzureBlobContainer(options);

            var blobName = Guid.NewGuid().ToString("D");

            var blobStorage = await blobStorageContainer.GetAzureBlob(blobName);
            await blobStorage.Upload("Some mock blob content");

            var blobClient = new BlobClient(options.Value.ConnectionString, options.Value.ContainerName, blobName);
            var blobStorageMetadata = new AzureBlobMetadata();
            blobStorageMetadata.SetIsDeleted(true);
            await blobClient.SetMetadataAsync(blobStorageMetadata.Metadata);

            // Act
            var blobStorageList = await blobStorageContainer.GetAzureBlobList();

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
