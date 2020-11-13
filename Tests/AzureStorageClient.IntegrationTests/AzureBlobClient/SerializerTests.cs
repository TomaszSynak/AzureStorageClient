namespace AzureStorageClient.IntegrationTests.AzureBlobClient
{
    using System;
    using System.Linq;
    using Xunit;

    [Collection(nameof(IntegrationTests))]
    public class SerializerTests
    {
        /// <summary>
        /// You should not override the Newtonsoft.Json ContractResolver with CamelCasePropertyNamesContractResolver for Json Serialization.
        /// Newtonsoft.Json by default respects the casing used in property/field names which is typically PascalCase.
        /// This can be overriden to serialize the names to camelCase and blob client will store the JSON in the storage as specified by the Newtonsoft.Json settings.
        /// https://martendb.io/documentation/documents/json/newtonsoft/
        /// </summary>
        [Fact]
        public void SerializerSettings_PropertyNameShouldNotBeLowercased()
        {
            // Arrange
            var (testModel, _, _) = TestModelFactory.Create();

            // Act
            var testModelSerialized = testModel.Serialize();
            var deserializedTestModel = testModelSerialized.Deserialize<TestModel>();

            // Assert
            Assert.Equal(testModel.PrivateData.Keys.First(), deserializedTestModel.PrivateData.Keys.First(), StringComparer.InvariantCulture);
        }
    }
}
