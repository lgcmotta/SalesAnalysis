using System;
using Microsoft.Extensions.Configuration;

namespace SalesAnalysis.ServicesConfiguration.Configurations
{
    public class ConfigurationFactory
    {
        public static IConfiguration GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? string.Empty;

            var appSettings = environment.Equals("Development") ? "appsettings.Development.json" : "appsettings.json";

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(appSettings, false, true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}