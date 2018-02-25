using System;
using FarLink.RabbitMq.Configuration;
using Microsoft.Extensions.Logging;

namespace FarLink.RabbitMq
{
    public interface IRabbitFarLink 
    {
        string AppId { get; }
        ILogger Logger { get; }
    }
}