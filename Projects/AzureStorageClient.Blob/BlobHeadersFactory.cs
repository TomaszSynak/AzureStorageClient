namespace AzureStorageClient
{
    using System.Security.Cryptography;
    using Azure.Storage.Blobs.Models;

    internal class BlobHeadersFactory
    {
        public static BlobHttpHeaders Create(byte[] blobStringContent)
        {
            var blobMd5Hash = MD5.Create().ComputeHash(blobStringContent);

            return new BlobHttpHeaders
            {
                ContentType = "application/json",
                ContentEncoding = "UTF-8",
                ContentHash = blobMd5Hash,
                ContentLanguage = null,
                CacheControl = null,
                ContentDisposition = null
            };
        }
    }
}
