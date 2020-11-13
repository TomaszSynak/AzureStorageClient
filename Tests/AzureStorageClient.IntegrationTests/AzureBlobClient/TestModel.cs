namespace AzureStorageClient.IntegrationTests.AzureBlobClient
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.Azure.Cosmos.Table;
    using Newtonsoft.Json;

    internal class TestModel : TableEntity, ITableStorable, IBlobStorable
    {
        [JsonProperty]
        private readonly IDictionary<string, Guid> _somePrivateData;

        public TestModel(IDictionary<string, Guid> somePrivateData)
        {
            _somePrivateData = somePrivateData;
        }

        public string Id { get; set; }

        public string AdditionalId { get; set; }

        public string Value { get; set; }

        public string BlobPath => string.IsNullOrWhiteSpace(AdditionalId) ? $"{Id}" : $"{AdditionalId}/{Id}";

        public Guid AzureTableRowId { get; set; }

        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public IReadOnlyDictionary<string, Guid> PrivateData => _somePrivateData.ToImmutableDictionary();

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
