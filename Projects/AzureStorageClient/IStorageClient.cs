namespace AzureStorageClient
{
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IStorageClient
    {
        Task<bool> IsAccessible(CancellationToken cancellationToken = default);

        Task UpsertAsync<TStorable>(TStorable objectToUpsert, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new();

        Task<TStorable> GetAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new();

        Task<ImmutableList<TStorable>> GetListAsync<TStorable>(string prefix = null, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new();

        Task SoftDeleteAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new();

        Task RevertSoftDeleteAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new();

        Task DeleteAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new();
    }
}
