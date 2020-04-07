namespace AzureStorageClient.IntegrationTests
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
        public async Task GetAzureBlob_ContainerDoesNotExists_ContainerCreated()
        {
            // ToDo: use create resource attribute
            // Arrange
            var options = OptionsFactory.Create(Guid.NewGuid().ToString("D"));
            var azureBlobContainer = new AzureBlobContainer(options);
            var blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);

            var containerExists = (await blobContainerClient.ExistsAsync()).Value;
            if (containerExists)
            {
                await blobContainerClient.DeleteAsync();
            }

            // Act
            await azureBlobContainer.GetAzureBlob(Guid.NewGuid().ToString("D"));

            // Assert
            containerExists = (await blobContainerClient.ExistsAsync()).Value;
            Assert.True(containerExists);

            // Clean up
            await blobContainerClient.DeleteAsync();
        }

        [Fact]
        [CleanBlobStorage]
        public async Task GetAzureBlob_ContainerExists_ContainerCreated()
        {
            // Arrange
            var options = OptionsFactory.Create();
            var azureBlobContainer = new AzureBlobContainer(options);
            var blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);

            // Act
            await azureBlobContainer.GetAzureBlob(Guid.NewGuid().ToString("D"));

            // Assert
            var containerExists = (await blobContainerClient.ExistsAsync()).Value;
            Assert.True(containerExists);
        }

        [Fact]
        public async Task GetAzureBlobList_ContainerExists_GetContainerContent()
        {
            // Arrange
            var options = OptionsFactory.Create(containerName: "some-mock-container");
            var azureBlobContainer = new AzureBlobContainer(options);
            var azureBlob = await azureBlobContainer.GetAzureBlob(Guid.NewGuid().ToString("D"));
            await azureBlob.Upload("Some mock blob content");

            // Act
            var azureBlobList = await azureBlobContainer.GetAzureBlobList();

            // Assert
            Assert.NotEmpty(azureBlobList);
            Assert.Single(azureBlobList);

            // Clean up
            var blobContainerClient = new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);
            var containerExists = (await blobContainerClient.ExistsAsync()).Value;
            if (containerExists)
            {
                await blobContainerClient.DeleteAsync();
            }
        }

        [Fact]
        public async Task GetAzureBlobList_ContainerContainsDeletedBlob_GetEmptyContent()
        {
            // Arrange
            var options = OptionsFactory.Create(containerName: "some-mock-container");
            var azureBlobContainer = new AzureBlobContainer(options);

            var blobName = Guid.NewGuid().ToString("D");

            var azureBlob = await azureBlobContainer.GetAzureBlob(blobName);
            await azureBlob.Upload("Some mock blob content");

            var blobClient = new BlobClient(options.Value.ConnectionString, options.Value.ContainerName, blobName);
            var azureBlobMetadata = new AzureBlobMetadata();
            azureBlobMetadata.SetIsDeleted(true);
            await blobClient.SetMetadataAsync(azureBlobMetadata.Metadata);

            // Act
            var azureBlobList = await azureBlobContainer.GetAzureBlobList();

            // Assert
            Assert.Empty(azureBlobList);

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
