namespace AzureStorageClient.IntegrationTests.PerformanceTests
{
    using System;
    using System.Reflection;
    using System.Threading;
    using Infrastructure;
    using Xunit.Sdk;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class SetUpAzureBlobAttribute : BeforeAfterTestAttribute
    {
        private readonly AzureBlobManager _blobStorageManager;

        public SetUpAzureBlobAttribute() => _blobStorageManager = new AzureBlobManager();

        public override void Before(MethodInfo methodUnderTest)
        {
            _blobStorageManager.SetUp(CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
