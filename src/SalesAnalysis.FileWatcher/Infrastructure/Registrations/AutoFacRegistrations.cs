using Autofac;
using Microsoft.Extensions.Configuration;
using SalesAnalysis.FileWatcher.Infrastructure.Modules;

namespace SalesAnalysis.FileWatcher.Infrastructure.Registrations
{
    public static class AutoFacRegistrations
    {
        public static void AddAutoFacModules(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterModule(new ApplicationModules());
        }
    }
}