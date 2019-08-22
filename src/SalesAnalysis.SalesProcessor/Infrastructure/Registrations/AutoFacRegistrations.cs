using Autofac;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;
using SalesAnalysis.SalesProcessor.Application.BusinessLogic;
using SalesAnalysis.SalesProcessor.Core.Interfaces;
using SalesAnalysis.SalesProcessor.Infrastructure.Persistence;

namespace SalesAnalysis.SalesProcessor.Infrastructure.Registrations
{
    public class AutoFacRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RabbitMqClientReceiver>()
                .As<IRabbitMqClientReceiver>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RabbitMqClientPublisher>()
                .As<IRabbitMqClientPublisher>()
                .InstancePerLifetimeScope();

            builder.RegisterType<OutputDataProcessor>()
                .As<IOutputDataProcessor>()
                .InstancePerLifetimeScope();
            builder.RegisterType<SalesDataProcessor>()
                .As<ISalesDataProcessor>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SalesFileAnalyzer>()
                .As<ISalesFileAnalyser>()
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(SalesProcessorDbContext));
        }
    }
}