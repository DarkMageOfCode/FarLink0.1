using System;
using FarLink.Logging;
using FarLink.RabbitMq.Configuration;

namespace FarLink.RabbitMq
{
    public interface IRabbitFarLink 
    {
        string AppId { get; }
        ILog Logger { get; }
    }
}