using FarLink.Metadata;
using FarLink.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FarLink
{
    public static class ServiceCollectionExtensions
    {
        

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