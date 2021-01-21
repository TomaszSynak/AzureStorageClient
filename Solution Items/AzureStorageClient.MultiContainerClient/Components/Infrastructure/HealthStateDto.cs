namespace AzureStorageClient.MultiContainerClient.Components.Infrastructure
{
    public class HealthStateDto
    {
        public string Name { get; set; }

        public bool IsHealthy { get; set; }

        public string Environment { get; set; }

        public string ApiVersion { get; set; }
    }
}
