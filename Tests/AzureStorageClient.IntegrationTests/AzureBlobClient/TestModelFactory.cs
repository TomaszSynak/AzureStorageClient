namespace AzureStorageClient.IntegrationTests.AzureBlobClient
{
    using System;
    using System.Collections.Generic;

    internal static class TestModelFactory
    {
        public static (TestModel testModel, string serialized, byte[] converted) Create()
        {
            var privateData = new Dictionary<string, Guid>
            {
                { "SomePrivateData", Guid.NewGuid() }
            };

            var model = new TestModel(privateData)
            {
                Id = Guid.NewGuid().ToString("D"),
                AdditionalId = Guid.NewGuid().ToString("D"),
                Value = $"SomeValueWithPolishLetters ł ą ę ó ź ż ć ś - {Guid.NewGuid():D}",
                AzureTableRowId = Guid.NewGuid()
            };

            var serialized = model.Serialize();
            var converted = model.Convert();

            return (model, serialized, converted);
        }

        public static (TestModel testModel, string serialized, byte[] converted) CreateWithoutAdditionalId()
        {
            var privateData = new Dictionary<string, Guid>
            {
                { "SomePrivateData", Guid.NewGuid() }
            };

            var model = new TestModel(privateData)
            {
                Id = Guid.NewGuid().ToString("D"),
                Value = $"SomeMockValueToTestUpsert ł ą ę ó ź ż ć ś -{Guid.NewGuid():D}",
                AzureTableRowId = Guid.NewGuid()
            };

            var serialized = model.Serialize();
            var converted = model.Convert();

            return (model, serialized, converted);
        }
    }
}
