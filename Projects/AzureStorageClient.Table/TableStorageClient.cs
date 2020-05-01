namespace AzureStorageClient
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    internal class TableStorageClient : ITableStorageClient
    {
        public TableStorageClient(IOptions<TableStorageClientSettings> options)
        {
        }

        public Task<bool> IsAccessible(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task UpsertAsync<TStorable>(TStorable objectToUpsert, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
            => throw new NotImplementedException();

        public Task<TStorable> GetAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
            => throw new NotImplementedException();

        public Task<ImmutableList<TStorable>> GetListAsync<TStorable>(string prefix = null, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
            => throw new NotImplementedException();

        public Task SoftDeleteAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
            => throw new NotImplementedException();

        public Task RevertSoftDeleteAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
            => throw new NotImplementedException();

        public Task DeleteAsync<TStorable>(string blobId, CancellationToken cancellationToken = default)
            where TStorable : class, IStorable, new()
            => throw new NotImplementedException();
    }
}
