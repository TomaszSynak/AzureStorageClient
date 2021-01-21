namespace AzureStorageClient.MultiContainerClient
{
    using System.Security.Authentication;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.ConfigureKestrel((context, serverOptions) =>
                        {
                            serverOptions.ConfigureHttpsDefaults(configureOptions => configureOptions.SslProtocols = SslProtocols.Tls12);
                        })
                        .UseStartup<Startup>()
                        .CaptureStartupErrors(true);
                });
    }
}
