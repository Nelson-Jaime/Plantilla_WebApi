using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Plantilla.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    webBuilder.ConfigureLogging(logging =>
                    {
                        Log.Logger = new LoggerConfiguration()
                                            .WriteTo.File("log.txt", Serilog.Events.LogEventLevel.Debug)
                                            .WriteTo.Console()
                                            .CreateLogger();
                    });
                });
    }
}
