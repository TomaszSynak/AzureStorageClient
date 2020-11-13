namespace AzureStorageClient
{
    using System;

    public class BlobContentCorruptedException : Exception
    {
        public BlobContentCorruptedException()
        {
        }

        public BlobContentCorruptedException(string message)
            : base(message)
        {
        }

        public BlobContentCorruptedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal static BlobContentCorruptedException Create(string blobName)
        {
            return new BlobContentCorruptedException($"Blob {blobName}'s content integrity is corrupted - MD5 mismatch.");
        }
    }
}
