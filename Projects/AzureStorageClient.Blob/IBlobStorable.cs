namespace AzureStorageClient
{
    public interface IBlobStorable
    {
        // ToDo: name of storable entity/userId/resourceId
        string StorableId { get; }
    }
}
