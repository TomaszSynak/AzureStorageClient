namespace AzureStorageClient
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    public interface ITableStorable : ITableEntity
    {
        Guid AzureTableId { get; }

        bool IsDeleted { get; set; }
    }
}
