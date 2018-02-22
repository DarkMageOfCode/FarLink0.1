using System;
using FarLink.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FarLink
{
    public interface IFarLink
    {
        IFarLink UseTypeEncoding(Action<ITypeEncodingBuilder> configure);
        IFarLink UseSerializer(Action<ISerializationBuilder> configure);
        IFarLink AddLogging(Action<ILoggingBuilder> builder);
        IServiceCollection Prepare(IServiceCollection collection = null);
        IFarLink ConfigureServices(Action<IServiceCollection> configurer);
    }
}