using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Order.Service.Hubs;
using Order.Service.Models;
using Order.Service.Repository;
using System.Text.Json;
using static Order.Service.Dtos;

namespace Order.Service.EventProcessing
{
    public class EventHandler
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
        private readonly IHubContext<OrderHub> _hubContext;

        public EventHandler(IServiceScopeFactory scopeFactory, IMapper mapper, IHubContext<OrderHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        public async Task OrderPublished(string message)
        {
            var orderPublishedDto = JsonSerializer.Deserialize<OrderPublishedDto>(message);
            var order = _mapper.Map<FoodOrder>(orderPublishedDto);
            order.Status = "Pending";
            order.IsAcceptedByDelivery = false;

            var scope = _scopeFactory.CreateScope();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IRepository<FoodOrder>>();

            var createdOrder = await orderRepository.CreateAsync(order);

            if(createdOrder is null)
            {
                Console.WriteLine("Unable to create order");
            }
            else
            {
                string room = $"{createdOrder.RestaurantName} {createdOrder.RestaurantAddress}";
                await _hubContext.Clients.Group(room).SendAsync("NewOrder", createdOrder);  
                Console.WriteLine("Send new order to Restaurants"); 
            }
        }

        public async Task OrderStreamingConnectionPublished(string message)
        {
            var orderStreamingConnection = JsonSerializer.Deserialize<OrderStreamingConnection>(message);
            var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRepository<OrderStreamingConnection>>();

            var createdOrderStreamingConnection = await repository.CreateAsync(orderStreamingConnection);
            if(createdOrderStreamingConnection is null)
            {
                Console.WriteLine("Unable to create order streaming connection");
            }
        }
    }
}
