using FarLink.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace FarLink.Serialization
{
    public static class TypeEncodingBuilderExtensions
    {
        public static TypeEncodingBuilder UseMetadata(this TypeEncodingBuilder builder)
            => builder.Add(sp => new MetadataEncoding(sp.GetService<IMetaInfoCache>()));

        public static TypeEncodingBuilder UseExceptions(this TypeEncodingBuilder builder)
            => builder.Add(_ => new ExceptionEncoding());
    }
}