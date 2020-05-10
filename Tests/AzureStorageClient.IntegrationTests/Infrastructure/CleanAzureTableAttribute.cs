namespace AzureStorageClient.IntegrationTests.Infrastructure
{
    using System;
    using System.Reflection;
    using System.Threading;
    using Xunit.Sdk;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class CleanAzureTableAttribute : BeforeAfterTestAttribute
    {
        private readonly AzureTableManager _azureTableManager;

        private readonly string _tableName;

        public CleanAzureTableAttribute(string tableName)
        {
            _azureTableManager = new AzureTableManager();
            _tableName = tableName;
        }

        // public override void Before(MethodInfo methodUnderTest) => _azureTableManager.SetUp(_tableName, CancellationToken.None).GetAwaiter().GetResult();
        public override void After(MethodInfo methodUnderTest) => _azureTableManager.CleanUp(_tableName, CancellationToken.None).GetAwaiter().GetResult();
    }
}
