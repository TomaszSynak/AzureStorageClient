namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using System;

    internal static class TestModelFactory
    {
        public static (TestModel testModel, string serialized, byte[] converted) Create()
        {
            var model = new TestModel
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
            var model = new TestModel
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
