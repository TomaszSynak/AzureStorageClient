namespace AzureStorageClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    internal class AzureBlobMetadata
    {
        private const string IsDeletedKey = "IsDeleted";

        private readonly IDictionary<string, string> _metadata;

        public AzureBlobMetadata(IDictionary<string, string> metadata) => _metadata = metadata;

        public AzureBlobMetadata() => _metadata = new Dictionary<string, string>();

        public IDictionary<string, string> Metadata => _metadata ?? new Dictionary<string, string>();

        public void SetIsDeleted(bool isDeleted = false) => Set(IsDeletedKey, isDeleted);

        public bool IsDeleted() => Get<bool>(IsDeletedKey);

        public bool IsNotDeleted() => !IsDeleted();

        internal T Get<T>(string key)
        {
            if (_metadata.ContainsKey(key))
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(_metadata[key]);
            }

            throw new Exception($"Metadata {key} is missing.");
        }

        internal void Set<T>(string key, T value)
        {
            if (_metadata.ContainsKey(key))
            {
                _metadata[key] = value.ToString();
            }
            else
            {
                _metadata.Add(key, value.ToString());
            }
        }
    }
}
