namespace AzureStorageClient.IntegrationTests.AzureBlobClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Xunit;
    using AzureBlobClient = AzureStorageClient.AzureBlobClient;

    [CleanAzureBlob]
    [Collection(nameof(IntegrationTests))]
    public class AzureBlobClientTests
    {
        private readonly AzureBlobClient _azureBlobClient;

        public AzureBlobClientTests() => _azureBlobClient = new AzureBlobClient(new AzureBlobContainer(OptionsFactory.CreateBlobSettings()));

        [Fact]
        public async Task UpsertAsync_ObjectUpsertCorrectly()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();

            // Act
            await _azureBlobClient.UpsertAsync(testModel);

            // Assert
            var blob = await _azureBlobClient.GetAsync<TestModel>(testModel.BlobPath);
            Assert.NotNull(blob);
        }

        [Fact]
        public async Task UpsertAsync_BulkUpsert_ListUpsertCorrectly()
        {
            // Arrange
            var listToUpsert = new List<TestModel>();
            for (int i = 0; i < 100; i++)
            {
                listToUpsert.Add(TestModelFactory.Create().testModel);
            }

            // Act
            await _azureBlobClient.UpsertAsync(listToUpsert);

            // Assert
            var uploadedBlobList = await _azureBlobClient.GetFolderContentAsync<TestModel>();
            Assert.NotEmpty(uploadedBlobList);
            Assert.Equal(listToUpsert.Count, uploadedBlobList.Count);
        }

        [Fact]
        public async Task GetAsync_ObjectRetrievedCorrectly()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _azureBlobClient.UpsertAsync(testModel);

            // Act
            var blobContent = await _azureBlobClient.GetAsync<TestModel>(testModel.BlobPath);

            // Assert
            Assert.NotNull(blobContent);
            Assert.Equal(testModel.Id, blobContent.Id);
            Assert.Equal(testModel.Value, blobContent.Value);
        }

        [Fact]
        public async Task GetListAsync_ListRetrievedCorrectlyWithNewlyCreatedBlob()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _azureBlobClient.UpsertAsync(testModel);

            // Act
            var blobContentList = await _azureBlobClient.GetFolderContentAsync<TestModel>();
            var newlyCreatedBlob = blobContentList.SingleOrDefault(bc => bc.Id.Equals(testModel.Id, StringComparison.InvariantCultureIgnoreCase));

            // Assert
            Assert.NotEmpty(blobContentList);
            Assert.NotNull(newlyCreatedBlob);
            Assert.Equal(testModel.Id, newlyCreatedBlob.Id);
            Assert.Equal(testModel.Value, newlyCreatedBlob.Value);
        }

        [Fact]
        public async Task GetListAsync_PassPrefix_ShouldListCorrectBlobs()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _azureBlobClient.UpsertAsync(testModel);
            var (notListedModel, _, _) = TestModelFactory.CreateWithoutAdditionalId();
            await _azureBlobClient.UpsertAsync(notListedModel);

            // Act
            var blobContentList = await _azureBlobClient.GetFolderContentAsync<TestModel>(prefix: testModel.AdditionalId);
            var newlyCreatedBlob = blobContentList.SingleOrDefault(bc => bc.Id.Equals(testModel.Id, StringComparison.InvariantCultureIgnoreCase));

            // Assert
            Assert.NotEmpty(blobContentList);
            Assert.Single(blobContentList);
            Assert.NotNull(newlyCreatedBlob);
            Assert.Equal(testModel.Id, newlyCreatedBlob.Id);
            Assert.Equal(testModel.Value, newlyCreatedBlob.Value);
        }

        [Fact]
        public async Task DeleteAsync_ObjectDeletedCorrectly()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _azureBlobClient.UpsertAsync(testModel);

            // Act
            await _azureBlobClient.DeleteAsync<TestModel>(testModel.BlobPath);

            // Assert
            var checkIfExists = await _azureBlobClient.GetAsync<TestModel>(testModel.BlobPath);
            Assert.Null(checkIfExists);
        }

        [Fact]
        public async Task DeleteFolderAsync_FolderDeletedCorrectly()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _azureBlobClient.UpsertAsync(testModel);

            // Act
            await _azureBlobClient.DeleteFolderAsync<TestModel>(testModel.AdditionalId);

            // Assert
            var checkIfExists = await _azureBlobClient.GetAsync<TestModel>(testModel.BlobPath);
            Assert.Null(checkIfExists);
        }
    }
}
