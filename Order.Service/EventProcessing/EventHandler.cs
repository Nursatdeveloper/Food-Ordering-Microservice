using AutoMapper;
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

        public EventHandler(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
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

        }
    }
}
