namespace AzureStorageClient
{
    using System.Security.Cryptography;
    using Azure.Storage.Blobs.Models;

    internal class AzureBlobHeadersFactory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5351:Do Not Use Broken Cryptographic Algorithms", Justification = "Verify blob integrity - not used for security purposes")]
        public static BlobHttpHeaders Create(byte[] blobStringContent)
        {
            byte[] blobMd5Hash = null;
            using (var md5 = MD5.Create())
            {
                blobMd5Hash = md5.ComputeHash(blobStringContent);
            }

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
