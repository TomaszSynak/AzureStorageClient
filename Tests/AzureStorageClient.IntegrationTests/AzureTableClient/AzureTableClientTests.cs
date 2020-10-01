namespace AzureStorageClient.IntegrationTests.AzureTableClient
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Xunit;
    using AzureTableClient = AzureStorageClient.AzureTableClient;

    [CleanAzureTable(nameof(TestModel))]
    [Collection(nameof(IntegrationTests))]
    public class AzureTableClientTests
    {
        private readonly AzureTableClient _azureTableClient;

        public AzureTableClientTests()
        {
            _azureTableClient = new AzureTableClient(OptionsFactory.CreateTableSettings());
        }

        [Fact]
        public async Task IsAccessible_ReturnsTrue()
        {
            // Act
            var result = await _azureTableClient.IsAccessible<TestModel>();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpsertAsync_ObjectUpsertCorrectly()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();

            // Act
            await _azureTableClient.UpsertAsync(testModel);

            // Assert
            var storableEntity = await _azureTableClient.GetAsync<TestModel>(testModel.AzureTableRowId);
            Assert.NotNull(storableEntity);
        }

        [Fact]
        public async Task GetAsync_ObjectRetrievedCorrectly()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _azureTableClient.UpsertAsync(testModel);

            // Act
            var storableEntity = await _azureTableClient.GetAsync<TestModel>(testModel.AzureTableRowId);

            // Assert
            Assert.NotNull(storableEntity);
            Assert.Equal(testModel.Id, storableEntity.Id);
            Assert.Equal(testModel.Value, storableEntity.Value);
            Assert.Equal(testModel.IsDeleted, storableEntity.IsDeleted);
            Assert.Equal(testModel.AdditionalId, storableEntity.AdditionalId);
            Assert.Equal(testModel.AzureTableRowId, storableEntity.AzureTableRowId);
        }

        [Fact]
        public async Task GetListAsync_ListRetrievedCorrectlyWithNewlyCreatedBlob()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _azureTableClient.UpsertAsync(testModel);

            // Act
            var storableEntityList = await _azureTableClient.GetListAsync<TestModel>();
            var newlyCreatedStorableEntity = storableEntityList.SingleOrDefault(se => se.Id.Equals(testModel.Id, StringComparison.InvariantCultureIgnoreCase));

            // Assert
            Assert.NotEmpty(storableEntityList);
            Assert.NotNull(newlyCreatedStorableEntity);
            Assert.Equal(testModel.Id, newlyCreatedStorableEntity.Id);
            Assert.Equal(testModel.Value, newlyCreatedStorableEntity.Value);
            Assert.Equal(testModel.IsDeleted, newlyCreatedStorableEntity.IsDeleted);
            Assert.Equal(testModel.AdditionalId, newlyCreatedStorableEntity.AdditionalId);
            Assert.Equal(testModel.AzureTableRowId, newlyCreatedStorableEntity.AzureTableRowId);
        }

        [Fact]
        public async Task SoftDeleteAsync_ObjectSoftDeletedCorrectly()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            testModel.IsDeleted = false;
            await _azureTableClient.UpsertAsync(testModel);

            // Act
            await _azureTableClient.SoftDeleteAsync<TestModel>(testModel.AzureTableRowId);

            // Assert
            var storableEntity = await _azureTableClient.GetAsync<TestModel>(testModel.AzureTableRowId);
            Assert.NotNull(storableEntity);
            Assert.True(storableEntity.IsDeleted);
        }

        [Fact]
        public async Task RevertSoftDeleteAsync_ObjectRevertSoftDeletedCorrectly()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            testModel.IsDeleted = true;
            await _azureTableClient.UpsertAsync(testModel);

            // Act
            await _azureTableClient.RevertSoftDeleteAsync<TestModel>(testModel.AzureTableRowId);

            // Assert
            var storableEntity = await _azureTableClient.GetAsync<TestModel>(testModel.AzureTableRowId);
            Assert.NotNull(storableEntity);
            Assert.False(storableEntity.IsDeleted);
        }

        [Fact]
        public async Task DeleteAsync_ObjectDeletedCorrectly()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();
            await _azureTableClient.UpsertAsync(testModel);

            // Act
            await _azureTableClient.DeleteAsync<TestModel>(testModel.AzureTableRowId);

            // Assert
            var storableEntity = await _azureTableClient.GetAsync<TestModel>(testModel.AzureTableRowId);
            Assert.Null(storableEntity);
        }
    }
}
