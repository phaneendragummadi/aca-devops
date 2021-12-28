using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOps.App
{
    public class Program
    {
        public static IConfiguration Configuration;
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .CreateLogger();

            try
            {
                CreateHostBuilder(args)
                    .Build()
                    .Run();

                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Configuration = CreateConfiguration(args);
            IHostBuilder webHostBuilder = CreateHostBuilder(args, Configuration);

            return webHostBuilder;
        }

        private static IConfiguration CreateConfiguration(string[] args)
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .AddEnvironmentVariables(prefix: "CODITO_")
                    .Build();

            return configuration;
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
        {
            IHostBuilder webHostBuilder =
                 Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration(configBuilder =>
                                {
                                    configBuilder.AddEnvironmentVariables(prefix: "DEVOPS_");
                                })
                    .ConfigureSecretStore((config, stores) =>
                    {
                        stores.AddEnvironmentVariables(prefix: "DEVOPS_");
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.ConfigureKestrel(kestrelServerOptions => kestrelServerOptions.AddServerHeader = false)
                             .UseStartup<Startup>();
                    });
            return webHostBuilder;
        }

    }
}
