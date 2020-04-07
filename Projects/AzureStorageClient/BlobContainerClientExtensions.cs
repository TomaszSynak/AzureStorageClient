namespace AzureStorageClient
{
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;

    internal static class BlobContainerClientExtensions
    {
        public static async Task<bool> IsAccessibleAsync(this BlobContainerClient blobContainerClient, CancellationToken cancellationToken = default)
            => (await blobContainerClient.ExistsAsync(cancellationToken)).GetRawResponse().Status.Equals((int)HttpStatusCode.OK);

        public static bool DoesNotExist(this BlobContainerClient blobContainerClient)
            => !blobContainerClient.Exists().Value;

        public static async Task<bool> DoesNotExistAsync(this BlobContainerClient blobContainerClient, CancellationToken cancellationToken = default)
            => !(await blobContainerClient.ExistsAsync(cancellationToken)).Value;
    }
}
