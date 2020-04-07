namespace AzureStorageClient.IntegrationTests
{
    using AzureStorageClient;

    internal class TestModel : IStorable
    {
        public string Id { get; set; }

        public string AdditionalId { get; set; }

        public string Value { get; set; }

        public string BlobId => string.IsNullOrWhiteSpace(AdditionalId) ? $"{Id}" : $"{AdditionalId}/{Id}";

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
