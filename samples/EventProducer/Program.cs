using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FarLink;
using FarLink.Logging;
using FarLink.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace EventProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}:{SourceContext}] {Message:lj} {NewLine}{Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            var sc = new ServiceCollection()
                .AddLogging(builder => builder.AddSerilog())
                .AddFarLinkLogging()
                .AddRabbitFarLink();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(sc);
            using (var container = containerBuilder.Build())
            {

                var rootProvider = new AutofacServiceProvider(container);
                var logger = rootProvider.GetService<ILog<Program>>();
                logger.With("@Value", new { Name ="Test", Id = 5 }).Info("Service {SourceContext} started!");
                var rfl = rootProvider.GetService<IRabbitFarLink>();
                Console.ReadKey();

            }

            
        }
    }
}