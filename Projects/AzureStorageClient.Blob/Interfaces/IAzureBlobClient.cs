namespace AzureStorageClient
{
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAzureBlobClient
    {
        Task<bool> IsAccessible(CancellationToken cancellationToken = default);

        Task UpsertAsync<TStorable>(TStorable objectToUpsert, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable;

        Task<TStorable> GetAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable;

        Task<ImmutableList<TStorable>> GetFolderContentAsync<TStorable>(string prefix = null, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable;

        Task SoftDeleteAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable;

        Task RevertSoftDeleteAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable;

        Task DeleteAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable;

        Task DeleteFolderAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable;
    }
}
