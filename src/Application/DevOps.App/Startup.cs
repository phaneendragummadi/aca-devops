using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using DevOps.App.Interfaces;
using DevOps.App.KeyVault;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using Microsoft.OpenApi.Models;

namespace DevOps.App
{
    public class Startup
    {
        private const string ApiName = "DevOps Introduction API";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAzureClients(azureClientFactoryBuilder =>
            {
                string vaultUrl = Configuration.GetValue<string>("VaultUri");
                var vaultUri = new Uri(vaultUrl);
                azureClientFactoryBuilder.AddSecretClient(vaultUri);
            });

            services.AddSingleton<IKeyVaultManager, KeyVaultManager>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = ApiName, Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", ApiName));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
