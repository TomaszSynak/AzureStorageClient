namespace AzureStorageClient.MultiContainerClient.Components.Infrastructure
{
    using System.Threading;
    using System.Threading.Tasks;

    public class ExternalResources
    {
        private readonly IAzureBlobClient _azureBlobClient;

        public ExternalResources(IAzureBlobClient azureBlobClient)
        {
            _azureBlobClient = azureBlobClient;
        }

        public async Task<bool> AreAccessible(CancellationToken ct = default)
        {
            return await _azureBlobClient.IsAccessible(ct);
        }

        public async Task<bool> AreNotAccessible(CancellationToken cancellationToken = default)
            => (await AreAccessible(cancellationToken)).Equals(false);
    }
}
