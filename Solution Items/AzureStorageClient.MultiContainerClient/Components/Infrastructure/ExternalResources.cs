namespace AzureStorageClient.MultiContainerClient.Components.Infrastructure
{
    using System.Threading;
    using System.Threading.Tasks;
    using NinjaContainer;
    using SamuraiContainer;

    public class ExternalResources
    {
        private readonly IAzureBlobClient<NinjaSettings> _ninjaBlobClient;

        private readonly IAzureBlobClient<SamuraiSettings> _samuraiBlobClient;

        public ExternalResources(IAzureBlobClient<NinjaSettings> ninjaBlobClient, IAzureBlobClient<SamuraiSettings> samuraiBlobClient)
        {
            _ninjaBlobClient = ninjaBlobClient;
            _samuraiBlobClient = samuraiBlobClient;
        }

        public async Task<bool> AreAccessible(CancellationToken ct = default)
        {
            return await _ninjaBlobClient.IsAccessible(ct) && await _samuraiBlobClient.IsAccessible(ct);
        }

        public async Task<bool> AreNotAccessible(CancellationToken cancellationToken = default)
            => (await AreAccessible(cancellationToken)).Equals(false);
    }
}
