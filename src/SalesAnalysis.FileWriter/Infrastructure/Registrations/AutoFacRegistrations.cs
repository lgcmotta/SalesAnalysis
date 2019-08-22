using Autofac;
using SalesAnalysis.FileWriter.Application.BusinessLogic;
using SalesAnalysis.FileWriter.Core.Interfaces;
using SalesAnalysis.FileWriter.Infrastructure.Persistence;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.FileWriter.Infrastructure.Registrations
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