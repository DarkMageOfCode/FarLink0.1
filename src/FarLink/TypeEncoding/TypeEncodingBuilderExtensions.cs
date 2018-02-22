using FarLink.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace FarLink.Serialization
{
    public static class TypeEncodingBuilderExtensions
    {
        public static ITypeEncodingBuilder UseMetadata(this ITypeEncodingBuilder builder)
            => builder.Add(sp => new MetadataEncoding(sp.GetService<IMetadataCache>()));

        public static ITypeEncodingBuilder UseExceptions(this ITypeEncodingBuilder builder)
            => builder.Add(_ => new ExceptionEncoding());
    }
}