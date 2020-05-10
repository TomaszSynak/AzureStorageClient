namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    internal class TestModel : TableEntity, ITableStorable, IBlobStorable
    {
        public TestModel()
        {
        }

        public string Id { get; set; }

        public string AdditionalId { get; set; }

        public string Value { get; set; }

        public string StorableId => string.IsNullOrWhiteSpace(AdditionalId) ? $"{Id}" : $"{AdditionalId}/{Id}";

        public Guid AzureTableId { get; set; }

        public bool IsDeleted { get; set; }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        public byte[] Convert()
        {
            return Serialize().Encode();
        }
    }
}
