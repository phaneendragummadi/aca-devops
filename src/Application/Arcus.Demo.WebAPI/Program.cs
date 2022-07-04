using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text.Json.Serialization;
using Arcus.Security.Core.Caching.Configuration;
using Arcus.WebApi.Logging.Core.Correlation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Microsoft.OpenApi.Models;
using Arcus.Demo.WebAPI.ExampleProviders;
using Swashbuckle.AspNetCore.Filters;

namespace Arcus.Demo.WebAPI
{
    public class Program
    {
        #warning Make sure that the appsettings.json is updated with your Azure Application Insights instrumentation key.
        private const string ApplicationInsightsInstrumentationKeyName = "APPINSIGHTS_INSTRUMENTATIONKEY";
        
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            
            try
            {
                CreateWebApplication(args)
                    .Run();
                
                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Host terminated unexpectedly");
                return 1;
                //if error 
                // check excluded ports: netsh interface ipv4 show excludedportrange protocol=tcp
                //https://ardalis.com/attempt-made-to-access-socket/
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        
        public static WebApplication CreateWebApplication(string[] args)
        {
            IConfiguration configuration = CreateConfiguration(args);
            WebApplicationBuilder builder = CreateWebApplicationBuilder(args, configuration);
            
            WebApplication app = builder.Build();
            ConfigureApp(app);
            
            return app;
        }
        
        private static IConfiguration CreateConfiguration(string[] args)
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
            
            return configuration;
        }
        
        private static WebApplicationBuilder CreateWebApplicationBuilder(string[] args, IConfiguration configuration)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            
            builder.Configuration.AddConfiguration(configuration);
            ConfigureServices(builder, configuration);
            ConfigureHost(builder, configuration);
            
            return builder;
        }
        
        private static void ConfigureServices(WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });
            builder.Services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.RespectBrowserAcceptHeader = true;
                options.OnlyAllowJsonFormatting();
                options.ConfigureJsonFormatting(json =>
                {
                    json.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    json.Converters.Add(new JsonStringEnumConverter());
                });

            });
            builder.Services.AddHealthChecks();
            builder.Services.AddHttpCorrelation((HttpCorrelationInfoOptions options) => { });
            
            ConfigureOpenApi(builder);
        }
        
        private static void ConfigureOpenApi(WebApplicationBuilder builder)
        {
            #warning Be careful of exposing sensitive information with the OpenAPI document, only expose what's necessary and hide everything else.
            var openApiInformation = new OpenApiInfo
            {
                Title = "Arcus.Demo.WebAPI",
                Version = "v1"
            };
            
            builder.Services.AddSwaggerGen(swaggerGenerationOptions =>
            {
                swaggerGenerationOptions.SwaggerDoc("v1", openApiInformation);
                swaggerGenerationOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Arcus.Demo.WebAPI.Open-Api.xml"));

                swaggerGenerationOptions.ExampleFilters();
                swaggerGenerationOptions.OperationFilter<AddHeaderOperationFilter>("X-Transaction-Id", "Transaction ID is used to correlate multiple operation calls. A new transaction ID will be generated if not specified.", false);
                swaggerGenerationOptions.OperationFilter<AddResponseHeadersFilter>();
            });
            
            builder.Services.AddSwaggerExamplesFromAssemblyOf<HealthReportResponseExampleProvider>();
        }
        
        private static void ConfigureHost(WebApplicationBuilder builder, IConfiguration configuration)
        {
            string arcusport = configuration["ARCUS_HTTP_PORT"];
            string httpEndpointUrl = "http://+:" + arcusport;
            builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
            //  .UseUrls(httpEndpointUrl);

            builder.Host.ConfigureSecretStore((context, config, stores) =>
            {
#if DEBUG
            stores.AddConfiguration(config);
#endif

            //#error Please provide a valid secret provider, for example Azure Key Vault: https://security.arcus-azure.net/features/secret-store/provider/key-vault
                string vaulturi = configuration["VaultUri"];
                stores.AddAzureKeyVaultWithManagedIdentity(vaulturi, CacheConfiguration.Default);
            });
            builder.Host.UseSerilog(ConfigureLoggerConfiguration);
        }

        
        private static void ConfigureLoggerConfiguration(
            HostBuilderContext context, 
            IServiceProvider serviceProvider, 
            LoggerConfiguration config)
        {
            config.ReadFrom.Configuration(context.Configuration)
                  .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                  .Enrich.FromLogContext()
                  .Enrich.WithVersion()
                  .Enrich.WithComponentName("API")
                  //.Enrich.WithHttpCorrelationInfo(serviceProvider)
                   .WriteTo.Console();
            
            var instrumentationKey = context.Configuration.GetValue<string>(ApplicationInsightsInstrumentationKeyName);
            if (!string.IsNullOrWhiteSpace(instrumentationKey))
            {
                config.WriteTo.AzureApplicationInsights(instrumentationKey);
            }
        }
        
        private static void ConfigureApp(IApplicationBuilder app)
        {
          //  app.UseHttpCorrelation();
            app.UseRouting();
            app.UseRequestTracking(options => options.OmittedRoutes.Add("/"));
            app.UseExceptionHandling();
            
            #warning Please configure application with authentication mechanism: https://webapi.arcus-azure.net/features/security/auth/shared-access-key

            app.UseSwagger(swaggerOptions =>
            {
                swaggerOptions.RouteTemplate = "api/{documentName}/docs.json";
            });
#if DEBUG
            app.UseSwaggerUI(swaggerUiOptions =>
            {
                swaggerUiOptions.SwaggerEndpoint("/api/v1/docs.json", "Arcus.Demo.WebAPI");
                swaggerUiOptions.RoutePrefix = "api/docs";
                swaggerUiOptions.DocumentTitle = "Arcus.Demo.WebAPI";
            });
#endif
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
