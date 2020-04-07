namespace AzureStorageClient.IntegrationTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Xunit;

    [CleanBlobStorage]
    [Collection(nameof(IntegrationTests))]
    public class StorageClientTests
    {
        private readonly StorageClient _storageClient;

        public StorageClientTests() => _storageClient = new StorageClient(OptionsFactory.Create());

        [Fact]
        public async Task UpsertAsync_ObjectUpsertCorrectly()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();

            // Act
            await _storageClient.UpsertAsync(testModel);

            // Assert
            var blob = await _storageClient.GetAsync<TestModel>(testModel.BlobId);
            Assert.NotNull(blob);
        }

        [Fact]
        public async Task GetAsync_ObjectRetrievedCorrectly()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _storageClient.UpsertAsync(testModel);

            // Act
            var blobContent = await _storageClient.GetAsync<TestModel>(testModel.BlobId);

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
            await _storageClient.UpsertAsync(testModel);

            // Act
            var blobContentList = await _storageClient.GetListAsync<TestModel>();
            var newlyCreatedBlob = blobContentList.SingleOrDefault(bc => bc.Id.Equals(testModel.Id));

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
            await _storageClient.UpsertAsync(testModel);
            var (notListedModel, _, _) = TestModelFactory.CreateWithoutAdditionalId();
            await _storageClient.UpsertAsync(notListedModel);

            // Act
            var blobContentList = await _storageClient.GetListAsync<TestModel>(prefix: testModel.AdditionalId);
            var newlyCreatedBlob = blobContentList.SingleOrDefault(bc => bc.Id.Equals(testModel.Id));

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
            await _storageClient.UpsertAsync(testModel);

            // Act
            await _storageClient.DeleteAsync<TestModel>(testModel.BlobId);

            // Assert
            async Task CheckIfExists() => await _storageClient.GetAsync<TestModel>(testModel.BlobId);
            var exception = await Record.ExceptionAsync(CheckIfExists);
            Assert.IsType<Exception>(exception);
            Assert.StartsWith($"Failed to GET blob {testModel.BlobId}.", exception.Message);
        }
    }
}
