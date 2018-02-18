using FarLink.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace FarLink
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFarLinkLogging(this IServiceCollection registrations)
        {
            registrations.TryAddTransient<ILogFactory>(sp => new LogFactory(sp.GetService<ILoggerFactory>()));
            registrations.TryAddTransient(typeof(ILog<>), typeof(Log<>));
            return registrations;
        }
    }
}