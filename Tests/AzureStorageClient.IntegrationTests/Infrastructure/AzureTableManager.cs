namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;

    internal class AzureTableManager
    {
        private readonly CloudTableClient _cloudTableClient;

        public AzureTableManager()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationProvider.GetTableStorageClientSettings().ConnectionString);
            _cloudTableClient = cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
        }

        public async Task SetUp(string tableName, CancellationToken cancellationToken = default) => await SetUpBlobStorage(tableName, cancellationToken);

        public async Task CleanUp(string tableName, CancellationToken cancellationToken = default) => await CleanUpBlobStorage(tableName, cancellationToken);

        private async Task SetUpBlobStorage(string tableName, CancellationToken cancellationToken = default)
        {
            var cloudTable = _cloudTableClient.GetTableReference(tableName);

            if (await cloudTable.ExistsAsync(cancellationToken))
            {
                return;
            }

            await cloudTable.CreateIfNotExistsAsync(cancellationToken);
        }

        private async Task CleanUpBlobStorage(string tableName, CancellationToken cancellationToken = default)
        {
            var cloudTable = _cloudTableClient.GetTableReference(tableName);

            if (await cloudTable.ExistsAsync(cancellationToken))
            {
                await cloudTable.DeleteIfExistsAsync(cancellationToken);
            }
        }
    }
}
