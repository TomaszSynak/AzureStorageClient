namespace AzureStorageClient
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    public interface ITableStorable : ITableEntity
    {
        Guid AzureTableRowId { get; }

        bool IsDeleted { get; set; }
    }
}
