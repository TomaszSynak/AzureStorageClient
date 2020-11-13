namespace AzureStorageClient
{
    using System;

    public class BlobContainerException : Exception
    {
        public BlobContainerException()
        {
        }

        public BlobContainerException(string message)
            : base(message)
        {
        }

        public BlobContainerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
