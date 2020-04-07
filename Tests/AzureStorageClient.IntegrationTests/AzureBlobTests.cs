namespace AzureStorageClient.IntegrationTests
{
    using System;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Infrastructure;
    using Xunit;

    [CleanBlobStorage]
    [Collection(nameof(IntegrationTests))]
    public class AzureBlobTests
    {
        private readonly AzureBlob _azureBlob;

        private readonly BlobClient _blobClient;

        public AzureBlobTests()
        {
            var options = OptionsFactory.Create();
            var blobName = Guid.NewGuid().ToString("D");

            _blobClient = new BlobClient(options.Value.ConnectionString, options.Value.ContainerName, blobName);

            _azureBlob = new AzureBlob(options, blobName);
        }

        [Fact]
        public async Task Upload_UploadHeadersCorrect()
        {
            // Arrange
            var (_, serialized, converted) = TestModelFactory.Create();
            var blobHeaders = BlobHeadersFactory.Create(converted);

            // Act
            await _azureBlob.Upload(serialized);

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
            var (_, serialized, _) = TestModelFactory.Create();

            // Act
            await _azureBlob.Upload(serialized);

            // Assert
            var blobExists = (await _blobClient.ExistsAsync()).Value;
            Assert.True(blobExists);
        }

        [Fact]
        public async Task Upload_IdDeletedIsFalse()
        {
            // Arrange
            var (_, serialized, _) = TestModelFactory.Create();

            // Act
            await _azureBlob.Upload(serialized);

            // Assert
            var blobProperties = (await _blobClient.GetPropertiesAsync()).Value;
            var azureBlobMetadata = new AzureBlobMetadata(blobProperties.Metadata);
            Assert.True(azureBlobMetadata.IsNotDeleted());
            Assert.False(azureBlobMetadata.IsDeleted());
        }

        [Fact]
        public async Task Download_BlobExists_ContentDownloaded()
        {
            // Arrange
            var (testModel, serialized, _) = TestModelFactory.Create();
            await _azureBlob.Upload(serialized);

            // Act
            var downloadedContent = await _azureBlob.Download();
            var deserializedContent = downloadedContent.Deserialize<TestModel>();

            // Assert
            Assert.Equal(testModel.Id, deserializedContent.Id);
            Assert.Equal(testModel.Value, deserializedContent.Value);
        }

        [Fact]
        public async Task Download_BlobDoesNotExist_ExceptionThrown()
        {
            // Arrange
            async Task MethodUnderTest() => await _azureBlob.Download();

            // Act
            var exception = await Record.ExceptionAsync(MethodUnderTest);

            // Assert
            Assert.IsType<Exception>(exception);
        }

        [Fact]
        public async Task Download_BlobExistsAndBlobIsDeleted_ExceptionThrown()
        {
            // Arrange
            var (testModel, serialized, _) = TestModelFactory.Create();
            await _azureBlob.Upload(serialized);
            var azureBlobMetadata = new AzureBlobMetadata();
            azureBlobMetadata.SetIsDeleted(true);
            await _blobClient.SetMetadataAsync(azureBlobMetadata.Metadata);

            async Task MethodUnderTest() => await _azureBlob.Download();

            // Act
            var exception = await Record.ExceptionAsync(MethodUnderTest);

            // Assert
            Assert.IsType<Exception>(exception);

            var blobProperties = (await _blobClient.GetPropertiesAsync()).Value;
            azureBlobMetadata = new AzureBlobMetadata(blobProperties.Metadata);
            Assert.False(azureBlobMetadata.IsNotDeleted());
            Assert.True(azureBlobMetadata.IsDeleted());
        }

        [Fact]
        public async Task Delete_BlobExists_BlobRemoved()
        {
            // Arrange
            var (_, serialized, _) = TestModelFactory.Create();
            await _azureBlob.Upload(serialized);

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
