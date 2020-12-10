namespace AzureStorageClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class CollectionExtensions
    {
        public static IEnumerable<IEnumerable<T>> GetBatchesOf<T>(this IEnumerable<T> enumerableToBatch, int batchSize)
        {
            return enumerableToBatch
                .Select((x, i) => new { index = i, value = x })
                .GroupBy(x => x.index / batchSize)
                .Select(group => group.Select(v => v.value));
        }

        public static IEnumerable<IEnumerable<T>> GetBatchesOf<T>(this IReadOnlyList<T> collectionToBatch, int batchSize)
        {
            var numberOfBatches = (int)Math.Ceiling(collectionToBatch.Count / (decimal)batchSize);
            for (var i = 0; i < numberOfBatches; ++i)
            {
                yield return collectionToBatch.Skip(i * batchSize).Take(batchSize);
            }
        }
    }
}
