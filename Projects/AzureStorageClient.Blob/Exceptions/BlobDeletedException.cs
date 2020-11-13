namespace AzureStorageClient
{
    using System;

    public class BlobDeletedException : Exception
    {
        public BlobDeletedException()
        {
        }

        public BlobDeletedException(string message)
            : base(message)
        {
        }

        public BlobDeletedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal static BlobDeletedException Create(string blobName)
        {
            return new BlobDeletedException($"Blob {blobName} is deleted.");
        }
    }
}
