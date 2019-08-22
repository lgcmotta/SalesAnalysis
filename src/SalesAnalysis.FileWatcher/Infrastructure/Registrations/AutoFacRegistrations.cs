using Autofac;
using SalesAnalysis.FileWatcher.Application.BusinessLogic;
using SalesAnalysis.FileWatcher.Core.Interfaces;
using SalesAnalysis.FileWatcher.Infrastructure.Persitence;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.FileWatcher.Infrastructure.Registrations
{
    public class AutoFacRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FolderScanner>()
                .As<IFolderScanner>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RabbitMqClientPublisher>()
                .As<IRabbitMqClientPublisher>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RabbitMqClientReceiver>()
                .As<IRabbitMqClientReceiver>()
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(FileWatcherDbContext));
        }

    }
}