using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Skoruba.IdentityServer4.Shared.Helpers;

namespace Skoruba.IdentityServer4.Admin.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration(args);

            try
            {
                DockerHelpers.ApplyDockerConfiguration(configuration);

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                //Log.CloseAndFlush();
            }
        }

        private static IConfiguration GetConfiguration(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = environment == Environments.Development;

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
                //.AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
                //.AddJsonFile($"serilog.{environment}.json", optional: true, reloadOnChange: true);

            if (isDevelopment)
            {
                configurationBuilder.AddUserSecrets<Startup>();
            }

            _ = configurationBuilder.Build();

            //configuration.AddAzureKeyVaultConfiguration(configurationBuilder);

            configurationBuilder.AddCommandLine(args);
            configurationBuilder.AddEnvironmentVariables();

            return configurationBuilder.Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                     var configurationRoot = configApp.Build();

                     //configApp.AddJsonFile("serilog.json", optional: true, reloadOnChange: true);

                     var env = hostContext.HostingEnvironment;

                     //configApp.AddJsonFile($"serilog.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                     if (env.IsDevelopment())
                     {
                         configApp.AddUserSecrets<Startup>();
                     }

                     //configurationRoot.AddAzureKeyVaultConfiguration(configApp);

                     configApp.AddEnvironmentVariables();
                     configApp.AddCommandLine(args);
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSentry("https://7d49fc2605bb4f989759efb9044e8979@o361175.ingest.sentry.io/5620390");
                });
                //.UseSerilog((hostContext, loggerConfig) =>
                //{
                //    loggerConfig
                //        .ReadFrom.Configuration(hostContext.Configuration)
                //        .Enrich.WithProperty("ApplicationName", hostContext.HostingEnvironment.ApplicationName);
                //});
    }
}