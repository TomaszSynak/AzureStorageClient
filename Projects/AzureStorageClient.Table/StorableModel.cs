namespace AzureStorageClient
{
    using Microsoft.Azure.Cosmos.Table;

    internal class StorableModel : TableEntity
    {
        public StorableModel()
        {
        }

        public string Name { get; set; }
    }
}
