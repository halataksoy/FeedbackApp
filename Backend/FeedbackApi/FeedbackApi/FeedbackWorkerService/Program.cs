using Microsoft.Extensions.Hosting;

namespace FeedbackWorkerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}
