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
        

        public RabbitConfig Config => _link.Config;
        public ILog Logger => _link.Logger;
    }
}