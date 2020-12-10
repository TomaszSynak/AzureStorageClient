namespace AzureStorageClient.IntegrationTests.AzureBlobClient
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    [Collection(nameof(IntegrationTests))]
    public class CollectionExtensionsTests
    {
        [Fact]
        public void GetBatches_Enumerable_BatchesGetCorrectly()
        {
            // Arrange
            var expectedHashSet = new HashSet<int>();
            for (int i = 0; i < 100; i++)
            {
                expectedHashSet.Add(i);
            }

            // Act
            var actualHashSet = new HashSet<int>();
            foreach (var batch in expectedHashSet.GetBatchesOf(15))
            {
                foreach (var element in batch)
                {
                    actualHashSet.Add(element);
                }
            }

            // Assert
            Assert.Equal(expectedHashSet.Count, actualHashSet.Count);
        }

        [Fact]
        public void GetBatches_IReadOnlyList_BatchesGetCorrectly()
        {
            // Arrange
            var expectedList = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                expectedList.Add(i);
            }

            // Act
            var actualList = expectedList.GetBatchesOf(15).SelectMany(batch => batch).ToList();

            // Assert
            Assert.Equal(expectedList.Count, actualList.Count);
            Assert.Equal(expectedList.Count, actualList.ToHashSet().Count);
        }
    }
}
