using System.Text.Json;
using AutoMapper;

namespace Catalog.Service.EventProcessing
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
                case EventType.RestaurantPublished:
                    await handler.RestaurantPublised(message);
                    break;
                case EventType.RestaurantAddressPublished:
                    await handler.RestaurantAddressPublished(message);
                    break;
                case EventType.FoodCategoryPublished:
                    await handler.FoodCategoryPublished(message);
                    break;
                case EventType.FoodPublished:
                    await handler.FoodPublished(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string message)
        {
            var eventType = JsonSerializer.Deserialize<EventDto>(message);

            switch(eventType.Event)
            {
                case "Restaurant_Published":
                    return EventType.RestaurantPublished;
                case "Restaurant_Address_Published":
                    return EventType.RestaurantAddressPublished;
                case "Food_Published":
                    return EventType.FoodPublished;
                case "Food_Category_Published":
                    return EventType.FoodCategoryPublished;
                default:
                    return EventType.Undefined;
            }
        }
    }
}