using System;
using FarLink.Logging;
using FarLink.RabbitMq.Configuration;

namespace FarLink.RabbitMq
{
    public interface IRabbitFarLink 
    {
        RabbitConfig Config { get; }
        ILog Logger { get; }
    }
}