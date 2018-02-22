using FarLink.Logging;
using FarLink.Metadata;
using FarLink.Serialization;
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

        public static IServiceCollection UseMetadata(this IServiceCollection registrations)
        {
            registrations.TryAddSingleton<IMetadataCache>(new MetadataCache());
            return registrations;
        }

        public static IServiceCollection AddSerializer(this IServiceCollection registrations, ISerializationService serializationService)
        {
            registrations.AddSingleton(serializationService);
            return registrations;
        }
    }
}