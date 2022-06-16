using Catalog.Service.PublishItems;

namespace Catalog.Service.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishOrder(PublishOrder publishOrder);
    }
}
