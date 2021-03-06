﻿namespace AzureStorageClient
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAzureTableClient
    {
        Task<bool> IsAccessible<TStorable>(CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new();

        Task UpsertAsync<TStorable>(TStorable objectToUpsert, Guid? azureTablePartitionKey = null, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new();

        Task<TStorable> GetAsync<TStorable>(Guid azureTableRowId, Guid? azureTablePartitionId = null, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new();

        Task<ImmutableList<TStorable>> GetListAsync<TStorable>(Guid? azureTablePartitionId = null, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new();

        Task SoftDeleteAsync<TStorable>(Guid azureTableRowId, Guid? azureTablePartitionId = null, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new();

        Task RevertSoftDeleteAsync<TStorable>(Guid azureTableRowId, Guid? azureTablePartitionId = null, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new();

        Task DeleteAsync<TStorable>(Guid azureTableRowId, Guid? azureTablePartitionId = null, CancellationToken cancellationToken = default)
            where TStorable : class, ITableStorable, new();
    }
}
