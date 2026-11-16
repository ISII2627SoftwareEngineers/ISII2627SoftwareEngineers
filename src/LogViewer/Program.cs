using Microsoft.Extensions.Configuration;
using LogViewer;

namespace LogViewer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 1. Cargar el appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            // 2. Arrancar el suscriptor
            var subscriber = new Subscriber(config);
            await subscriber.StartAsync();
        }
    }
}