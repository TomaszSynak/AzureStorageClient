namespace AzureStorageClient.IntegrationTests.AzureTableClient
{
    using System;

    internal static class TestModelFactory
    {
        public static (TestModel testModel, string serialized, byte[] converted) Create()
        {
            var model = new TestModel
            {
                Id = "61cd1715-e818-480e-9681-57356ef024b8",
                AdditionalId = Guid.NewGuid().ToString("D"),
                Value = $"SomeValueWithPolishLetters ł ą ę ó ź ż ć ś - {Guid.NewGuid():D}",
                AzureTableRowId = Guid.Parse("46b83f72-708f-4eea-b071-24d9db97f140")
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
