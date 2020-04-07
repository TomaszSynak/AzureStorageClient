namespace AzureStorageClient
{
    public interface IStorable
    {
        // ToDo: name of storable entity/userId/resourceId
        string BlobId { get; }
    }
}
