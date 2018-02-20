using FarLink.Logging;
using FarLink.RabbitMq.Configuration;

namespace FarLink.RabbitMq.Utilites
{
    internal class RabbitFarLinkAdapter : IRabbitFarLink
    {
        private readonly RabbitFarLink _link;

        internal RabbitFarLinkAdapter(RabbitFarLink link)
        {
            _link = link;
        }
        

        public ILog Logger => _link.Logger;

        public string AppId => _link.AppId;
    }
}