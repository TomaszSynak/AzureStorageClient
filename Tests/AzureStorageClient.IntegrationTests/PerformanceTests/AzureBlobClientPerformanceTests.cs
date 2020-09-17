namespace AzureStorageClient.IntegrationTests.PerformanceTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Xunit;
    using Xunit.Abstractions;
    using AzureBlobClient = AzureStorageClient.AzureBlobClient;

    [SetUpAzureBlob]
    [Collection(nameof(IntegrationTests))]
    public class AzureBlobClientPerformanceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly AzureBlobClient _azureBlobClient;

        public AzureBlobClientPerformanceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _azureBlobClient = new AzureBlobClient(OptionsFactory.CreateBlobSettings());
        }

        //[Fact]
        //public async Task CreateReadModel()
        //{
        //    var stopwatch = Stopwatch.StartNew();

        //    stopwatch.Restart();

        //    var azureBlobClient = new AzureBlobClient(OptionsFactory.CreateBlobSettings());
        //    var readModel = ReadModelFactory.Create();
        //    await azureBlobClient.UpsertAsync(readModel, CancellationToken.None);

        //    stopwatch.Stop();

        //    _testOutputHelper.WriteLine($"Upload read blob time {stopwatch.Elapsed.TotalMilliseconds} ms");
        //}
        [Fact]
        public async Task GetAsync_SingleUser()
        {
            // Arrange
            var expectedReadModel = ReadModelFactory.Create();

            // Act
            var stopwatch = Stopwatch.StartNew();

            stopwatch.Restart();
            var blob = await _azureBlobClient.GetAsync<ReadModel>(expectedReadModel.StorableId);
            stopwatch.Stop();

            // Assert
            Assert.NotNull(blob);
            Assert.Equal(expectedReadModel.Id, blob.Id);
            Assert.Equal(expectedReadModel.AdditionalId, blob.AdditionalId);
            Assert.Equal(expectedReadModel.SectionName1, blob.SectionName1);
            Assert.Equal(expectedReadModel.SectionContent1, blob.SectionContent1);
            Assert.Equal(expectedReadModel.SectionName2, blob.SectionName2);
            Assert.Equal(expectedReadModel.SectionContent2, blob.SectionContent2);
            Assert.Equal(expectedReadModel.SectionName3, blob.SectionName3);
            Assert.Equal(expectedReadModel.SectionContent3, blob.SectionContent3);
            Assert.Equal(expectedReadModel.SectionName4, blob.SectionName4);
            Assert.Equal(expectedReadModel.SectionContent4, blob.SectionContent4);
            Assert.Equal(expectedReadModel.SectionName5, blob.SectionName5);
            Assert.Equal(expectedReadModel.SectionContent5, blob.SectionContent5);
            Assert.Equal(expectedReadModel.SectionName6, blob.SectionName6);
            Assert.Equal(expectedReadModel.SectionContent6, blob.SectionContent6);
            Assert.Equal(expectedReadModel.SectionName7, blob.SectionName7);
            Assert.Equal(expectedReadModel.SectionContent7, blob.SectionContent7);
            Assert.Equal(expectedReadModel.SectionName8, blob.SectionName8);
            Assert.Equal(expectedReadModel.SectionContent8, blob.SectionContent8);
            Assert.Equal(expectedReadModel.SectionName9, blob.SectionName9);
            Assert.Equal(expectedReadModel.SectionContent9, blob.SectionContent9);
            Assert.Equal(expectedReadModel.SectionName10, blob.SectionName10);
            Assert.Equal(expectedReadModel.SectionContent10, blob.SectionContent10);

            _testOutputHelper.WriteLine($"SingleUser read time {stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        [Fact]
        public async Task GetAsync_MultipleUsers()
        {
            // Act
            const int numberOfConcurrentUser = 100;
            var stopwatch = Stopwatch.StartNew();

            // Arrange
            var expectedReadModel = ReadModelFactory.Create();
            var taskList = new List<Task<ReadModel>>();

            stopwatch.Restart();

            for (int i = 0; i < numberOfConcurrentUser; i++)
            {
                taskList.Add(_azureBlobClient.GetAsync<ReadModel>(expectedReadModel.StorableId));
            }

            var results = await Task.WhenAll(taskList);

            stopwatch.Stop();

            // Assert
            foreach (var result in results)
            {
                Assert.NotNull(result);
            }

            _testOutputHelper.WriteLine($"MultipleUsers read time {stopwatch.Elapsed.TotalMilliseconds} ms for {numberOfConcurrentUser} users");
        }
    }
}
