using AutoMapper;
using System.Text.Json;
using static Order.Service.Dtos;

namespace Order.Service.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public async Task ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            EventHandler handler = new(_scopeFactory, _mapper);

            switch(eventType)
            {
                case EventType.Order_Published:
                    await handler.OrderPublished(message);
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
                default:
                    return EventType.Undefined;
            }
        }
    }
}
