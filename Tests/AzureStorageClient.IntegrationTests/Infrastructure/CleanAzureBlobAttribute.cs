namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using System;
    using System.Reflection;
    using System.Threading;
    using Xunit.Sdk;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class CleanAzureBlobAttribute : BeforeAfterTestAttribute
    {
        private readonly AzureBlobManager _blobStorageManager;

        public CleanAzureBlobAttribute() => _blobStorageManager = new AzureBlobManager();

        public override void Before(MethodInfo methodUnderTest) => _blobStorageManager.SetUp(CancellationToken.None).GetAwaiter().GetResult();

        public override void After(MethodInfo methodUnderTest) => _blobStorageManager.CleanUp(CancellationToken.None).GetAwaiter().GetResult();
    }
}
