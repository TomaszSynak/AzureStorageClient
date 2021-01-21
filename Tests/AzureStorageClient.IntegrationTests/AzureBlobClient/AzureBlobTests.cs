namespace AzureStorageClient.IntegrationTests.AzureBlobClient
{
    using System;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Infrastructure;
    using Xunit;

    [CleanAzureBlob]
    [Collection(nameof(IntegrationTests))]
    public class AzureBlobTests
    {
        private readonly AzureBlob _azureBlob;

        private readonly BlobClient _blobClient;

        public AzureBlobTests()
        {
            var options = OptionsFactory.CreateBlobSettings();
            var blobName = Guid.NewGuid().ToString("D");

            _blobClient = new BlobClient(options.Value.ConnectionString, options.Value.ContainerName, blobName);

            _azureBlob = new AzureBlob(options.Value, blobName);
        }

        [Fact]
        public async Task Upload_UploadHeadersCorrect()
        {
            // Arrange
            var (testModel, _, converted) = TestModelFactory.Create();
            var blobHeaders = AzureBlobHeadersFactory.Create(converted);

            // Act
            await _azureBlob.Upload(testModel);

            // Assert
            var blobProperties = (await _blobClient.GetPropertiesAsync()).Value;

            Assert.Equal("application/json", blobProperties.ContentType);
            Assert.Equal("UTF-8", blobProperties.ContentEncoding);
            Assert.Equal(blobHeaders.ContentHash, blobProperties.ContentHash);
            Assert.Null(blobProperties.ContentDisposition);
            Assert.Null(blobProperties.CacheControl);
            Assert.Null(blobProperties.ContentLanguage);
        }

        [Fact]
        public async Task Upload_BlobUploaded()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();

            // Act
            await _azureBlob.Upload(testModel);

            // Assert
            var blobExists = (await _blobClient.ExistsAsync()).Value;
            Assert.True(blobExists);
        }

        [Fact]
        public async Task Upload_IdDeletedIsFalse()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();

            // Act
            await _azureBlob.Upload(testModel);

            // Assert
            var blobProperties = (await _blobClient.GetPropertiesAsync()).Value;
            var blobStorageMetadata = new AzureBlobMetadata(blobProperties.Metadata);
            Assert.True(blobStorageMetadata.IsNotDeleted());
            Assert.False(blobStorageMetadata.IsDeleted());
        }

        [Fact]
        public async Task Download_BlobExists_ContentDownloaded()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _azureBlob.Upload(testModel);

            // Act
            var deserializedContent = await _azureBlob.Download<TestModel>();

            // Assert
            Assert.Equal(testModel.Id, deserializedContent.Id);
            Assert.Equal(testModel.Value, deserializedContent.Value);
        }

        [Fact]
        public async Task Download_BlobDoesNotExist_NullReturned()
        {
            // Act
            var result = await _azureBlob.Download<TestModel>();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Download_BlobExistsAndBlobIsDeleted_NullReturned()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _azureBlob.Upload(testModel);
            var blobStorageMetadata = new AzureBlobMetadata();
            blobStorageMetadata.SetIsDeleted(true);
            await _blobClient.SetMetadataAsync(blobStorageMetadata.Metadata);

            // Act
            var result = await _azureBlob.Download<TestModel>();

            // Assert
            Assert.Null(result);

            var blobProperties = (await _blobClient.GetPropertiesAsync()).Value;
            blobStorageMetadata = new AzureBlobMetadata(blobProperties.Metadata);
            Assert.False(blobStorageMetadata.IsNotDeleted());
            Assert.True(blobStorageMetadata.IsDeleted());
        }

        [Fact]
        public async Task Delete_BlobExists_BlobRemoved()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _azureBlob.Upload(testModel);

            // Act
            await _azureBlob.Delete();

            // Assert
            var blobExists = (await _blobClient.ExistsAsync()).Value;

            Assert.False(blobExists);
        }

        [Fact]
        public async Task Delete_BlobDoesNotExist_DoesNotThrowException()
        {
            // Arrange
            async Task MethodUnderTest() => await _azureBlob.Delete();

            // Act
            var exception = await Record.ExceptionAsync(MethodUnderTest);

            // Assert
            Assert.Null(exception);
        }
    }
}
