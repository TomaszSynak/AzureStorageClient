namespace AzureStorageClient
{
    using System;

    public class BlobClientException : Exception
    {
        public BlobClientException()
        {
        }

        public BlobClientException(string message)
            : base(message)
        {
        }

        public BlobClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
