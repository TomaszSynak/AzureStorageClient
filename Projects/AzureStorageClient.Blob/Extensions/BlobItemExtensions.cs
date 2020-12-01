namespace AzureStorageClient
{
    using Azure.Storage.Blobs.Models;

    internal static class BlobItemExtensions
    {
        public static bool IsBlobNotDeleted(this BlobItem blobItem)
        {
            var azureBlobMetadata = new AzureBlobMetadata(blobItem.Metadata);
            return azureBlobMetadata.IsNotDeleted();
        }
    }
}
