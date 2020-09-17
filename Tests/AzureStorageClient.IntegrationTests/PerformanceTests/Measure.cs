namespace AzureStorageClient.IntegrationTests.PerformanceTests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Measure
    {
        public static async Task<TimeSpan> GetExecutionTime(Func<CancellationToken, Task> functionToMeasure, CancellationToken cancellationToken = default)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Reset();
            stopWatch.Start();

            await functionToMeasure.Invoke(cancellationToken);

            stopWatch.Stop();
            return stopWatch.Elapsed;
        }
    }
}
