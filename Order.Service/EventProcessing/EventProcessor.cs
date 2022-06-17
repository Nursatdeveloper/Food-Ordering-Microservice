using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Order.Service.Hubs;
using System.Text.Json;
using static Order.Service.Dtos;

namespace Order.Service.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
        private readonly IHubContext<OrderHub> _hubContext;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper, IHubContext<OrderHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
            _hubContext = hubContext;
        }
        public async Task ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            EventHandler handler = new(_scopeFactory, _mapper, _hubContext);

            switch(eventType)
            {
                case EventType.Order_Published:
                    await handler.OrderPublished(message);
                    break;
                case EventType.OrderStreamingConnection_Published:
                    await handler.OrderStreamingConnectionPublished(message);
                    break;
                default:
                    break;
            }
        }

        public EventType DetermineEvent(string message)
        {
            var eventType = JsonSerializer.Deserialize<EventDto>(message);

            switch(eventType.Event)
            {
                case "Order_Published":
                    return EventType.Order_Published;
                case "OrderStreamingConnection_Published":
                    return EventType.OrderStreamingConnection_Published;
                default:
                    return EventType.Undefined;
            }
        }
    }
}
