using Autofac;
using SalesAnalysis.FileWatcher.Application.BusinessLogic;
using SalesAnalysis.FileWatcher.Core.Interfaces;
using SalesAnalysis.FileWatcher.Infrastructure.Persitence;

namespace SalesAnalysis.FileWatcher.Infrastructure.Modules
{
    public class ApplicationModules : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FolderScanner>()
                .As<IFolderScanner>()
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(FileWatcherDbContext));
        }

    }
}