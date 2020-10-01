namespace AzureStorageClient
{
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAzureBlobClient
    {
        Task<bool> IsAccessible(CancellationToken cancellationToken = default);

        Task UpsertAsync<TStorable>(TStorable objectToUpsert, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new();

        Task<TStorable> GetAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new();

        Task<ImmutableList<TStorable>> GetListAsync<TStorable>(string prefix = null, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new();

        Task SoftDeleteAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new();

        Task RevertSoftDeleteAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new();

        Task DeleteAsync<TStorable>(string blobPath, CancellationToken cancellationToken = default)
            where TStorable : class, IBlobStorable, new();
    }
}
