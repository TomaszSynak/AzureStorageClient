namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using System;
    using System.Reflection;
    using Xunit.Sdk;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class CleanBlobStorageAttribute : BeforeAfterTestAttribute
    {
        private readonly BlobStorageManager _blobStorageManager;

        public CleanBlobStorageAttribute() => _blobStorageManager = new BlobStorageManager();

        public override void Before(MethodInfo methodUnderTest) => _blobStorageManager.SetUp().GetAwaiter().GetResult();

        public override void After(MethodInfo methodUnderTest) => _blobStorageManager.CleanUp().GetAwaiter().GetResult();
    }
}
