using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace FileWatcherService1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logsFilePath = ConfigurationManager.AppSettings["logsFile"];

            Log.Logger = new LoggerConfiguration()
         .MinimumLevel.Information()
         .WriteTo.File(logsFilePath,
             rollOnFileSizeLimit: true)
         .CreateLogger();

            try
            {
                Log.Information("starting up the service");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There Where Froblam starting the service");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                }).UseSerilog();
        }
    }
    
}
