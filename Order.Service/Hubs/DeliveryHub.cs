using Microsoft.AspNetCore.SignalR;
using Order.Service.Models;
using Order.Service.Repository;
using Order.Service.SignalR;

namespace Order.Service.Hubs
{
    //<summary>
    // 
    //  This SignalR hub is responsible for real time update and processing of orders for delivery
    //
    //</summary>
    public class DeliveryHub : Hub
    {
        private readonly IDictionary<string, DeliveryConnection> _connections;
        private readonly IRepository<FoodOrder> _foodOrderRepository;

        public DeliveryHub(IDictionary<string, DeliveryConnection> connections, IRepository<FoodOrder> foodOrderRepository)
        {
            _connections = connections;
            _foodOrderRepository = foodOrderRepository;
        }

        public async Task JoinDeliveryStreaming(DeliveryConnection connection)
        {
            string room = $"Delivery_{connection.City}";
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            _connections[Context.ConnectionId] = connection;

            var deliveries = await _foodOrderRepository.GetAllAsync(x => x.Status == "Executing" || x.Status == "Ready to deliver");
            if(deliveries.Count() == 0)
            {
                await Clients.Group(room).SendAsync("NoDelivery", "Currently there are not any delivery options");
            }
            await Clients.Group(room).SendAsync("AvailableDeliveries", deliveries.ToList());
        }

    }
}
