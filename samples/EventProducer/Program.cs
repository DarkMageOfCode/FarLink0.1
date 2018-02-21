using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FarLink;
using FarLink.Eventing;
using FarLink.Json;
using FarLink.Logging;
using FarLink.Markup;
using FarLink.Markup.RabbitMq;
using FarLink.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace EventProducer
{
    [ContentType("application/json")]
    [Exchange("test.myevent", ExchangeKind.Fanout)]
    public class MyEvent : IEvent
    {
        public MyEvent(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}:{SourceContext}] {Message:lj} {NewLine}{Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            var sc = new FarLink.FarLink()
                .AddLogging(builder => builder.AddSerilog())
                .UseSerializer(bld => bld.AddJson())
                .Prepare()
                .AddRabbitFarLink(cfg => cfg.Uri("amqp://youdo:youdo@localhost"))
                .AddEvent<MyEvent>();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(sc);
            using (var container = containerBuilder.Build())
            {

                var rootProvider = new AutofacServiceProvider(container);
                var logger = rootProvider.GetService<ILog<Program>>();
                logger.With("@Value", new { Name ="Test", Id = 5 }).Info("Service {SourceContext} started!");
                var rfl = rootProvider.GetService<IRabbitFarLink>();
                var publisher = rootProvider.GetService<IEventPublisher<MyEvent>>();
                publisher.PublishAsync(new MyEvent("Hellow, World!")).GetAwaiter().GetResult();
                Console.ReadKey();

            }

            
        }
    }
}