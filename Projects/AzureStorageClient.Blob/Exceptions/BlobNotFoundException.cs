namespace AzureStorageClient
{
    using System;

    public class BlobNotFoundException : Exception
    {
        public BlobNotFoundException()
        {
        }

        public BlobNotFoundException(string message)
            : base(message)
        {
        }

        public BlobNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal static BlobNotFoundException Create(string blobName)
        {
            return new BlobNotFoundException($"Blob {blobName} does not exists.");
        }
    }
}
