namespace AzureStorageClient.IntegrationTests.AzureTableClient
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    internal class TestModel : TableEntity, ITableStorable, IBlobStorable
    {
        public string Id { get; set; }

        public string AdditionalId { get; set; }

        public string Value { get; set; }

        public string BlobPath => string.IsNullOrWhiteSpace(AdditionalId) ? $"{Id}" : $"{AdditionalId}/{Id}";

        public Guid AzureTableRowId { get; set; }

        public bool IsDeleted { get; set; }

        public string Serialize()
        {
            return Serializer.Serialize(this);
        }

        public byte[] Convert()
        {
            return Serialize().Encode();
        }
    }
}
