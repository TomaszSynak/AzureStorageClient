namespace AzureStorageClient.IntegrationTests.PerformanceTests
{
    internal class ReadModel : IBlobStorable
    {
        public string Id { get; set; }

        public string AdditionalId { get; set; }

        public string SectionName1 { get; set; }

        public string SectionContent1 { get; set; }

        public string SectionName2 { get; set; }

        public string SectionContent2 { get; set; }

        public string SectionName3 { get; set; }

        public string SectionContent3 { get; set; }

        public string SectionName4 { get; set; }

        public string SectionContent4 { get; set; }

        public string SectionName5 { get; set; }

        public string SectionContent5 { get; set; }

        public string SectionName6 { get; set; }

        public string SectionContent6 { get; set; }

        public string SectionName7 { get; set; }

        public string SectionContent7 { get; set; }

        public string SectionName8 { get; set; }

        public string SectionContent8 { get; set; }

        public string SectionName9 { get; set; }

        public string SectionContent9 { get; set; }

        public string SectionName10 { get; set; }

        public string SectionContent10 { get; set; }

        public string StorableId => GenerateStorableId(Id, AdditionalId);

        public static string GenerateStorableId(string id, string additionalId)
            => string.IsNullOrWhiteSpace(additionalId) ? $"{id}" : $"{additionalId}/{id}";

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
