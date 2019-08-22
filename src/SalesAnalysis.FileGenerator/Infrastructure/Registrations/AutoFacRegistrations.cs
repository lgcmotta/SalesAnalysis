using Autofac;
using SalesAnalysis.FileGenerator.Application.BusinessLogic;
using SalesAnalysis.FileGenerator.Core.Interfaces;
using SalesAnalysis.FileGenerator.Infrastructure.Persistence;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.FileGenerator.Infrastructure.Registrations
{
    public class AutoFacRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RabbitMqClientReceiver>()
                .As<IRabbitMqClientReceiver>()
                .InstancePerLifetimeScope();

            builder.RegisterType<OutputFileGenerator>()
                .As<IOutputFileGenerator>()
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(FileWriterDbContext));
        }
    }
}