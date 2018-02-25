using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FarLink;
using FarLink.Eventing;
using FarLink.Json;
using FarLink.Markup;
using FarLink.Markup.RabbitMq;
using FarLink.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                .AddRabbitMq(cfg => cfg.Uri("amqp://localhost"), builder => { builder.AddPublisher<MyEvent>(); })
                .Prepare();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(sc);
            using (var container = containerBuilder.Build())
            {

                var rootProvider = new AutofacServiceProvider(container);
                var logger = rootProvider.GetService<ILogger<Program>>();
                using(logger.BeginScope("{@Value}", new { Name ="Test", Id = 5 })) 
                    logger.LogInformation("Service {SourceContext} started!");
                var rfl = rootProvider.GetService<IRabbitFarLink>();
                var publisher = rootProvider.GetService<IEventPublisher<MyEvent>>();
                publisher.PublishAsync(new MyEvent("Hellow, World!")).GetAwaiter().GetResult();
                Console.ReadKey();

            }

            
        }
    }
}