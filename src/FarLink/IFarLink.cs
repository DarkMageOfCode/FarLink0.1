using System;
using FarLink.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FarLink
{
    public interface IFarLink
    {
        IFarLink UseTypeEncoding(Action<TypeEncodingBuilder> configure);
        IFarLink UseSerializer(Action<SerializationBuilder> configure);
        IFarLink AddLogging(Action<ILoggingBuilder> builder);
        IServiceCollection Prepare(IServiceCollection collection = null);
    }
}