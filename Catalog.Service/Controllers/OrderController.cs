using AutoMapper;
using Catalog.Service.AsyncDataServices;
using Catalog.Service.Models;
using Catalog.Service.PublishItems;
using Catalog.Service.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Service.Controllers
{
    [Route("api/v1/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IRepository<Food> _foodRepository;
        private readonly IRepository<Restaurant> _restaurantRepository;
        private readonly IRepository<FoodCategory> _foodCategoryRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IMessageBusClient _messageBusClient;
        private readonly IMapper _mapper;

        public OrderController(IRepository<Food> foodRepository,
                IRepository<Restaurant> restaurantRepository,
                IRepository<FoodCategory> foodCategoryRepository,
                IRepository<Address> addressRepository,
                IMessageBusClient messageBusClient,
                IMapper mapper)
        {
            _foodRepository = foodRepository;
            _restaurantRepository = restaurantRepository;
            _foodCategoryRepository = foodCategoryRepository;
            _addressRepository = addressRepository;
            _messageBusClient = messageBusClient;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult> Post(CreateOrderDto createOrderDto)
        {
            var food = await _foodRepository.GetByIdAsync(createOrderDto.foodId);
            if(food is null)
            {
                return BadRequest($"Food with id: {createOrderDto.foodId} does not exists!");
            }

            var restaurant = await _restaurantRepository.GetByIdAsync(food.RestaurantId);
            var foodCategory = await _foodCategoryRepository.GetByIdAsync(food.CategoryId);
            var address = await _addressRepository.GetAsync(a => a.RestaurantId == food.RestaurantId);
            string addressFormatted = $"{address.City},  {address.Street} {address.BuildingNumber}";

            var publishOrder = _mapper.Map<PublishOrder>(createOrderDto);
            publishOrder.Date = DateTime.UtcNow;
            publishOrder.RestaurantName = restaurant.Name;
            publishOrder.RestaurantAddress = addressFormatted; // <-- This actually should give the address of the nearest restaurant!
            publishOrder.FoodCategory = foodCategory.CategoryName;
            publishOrder.FoodName = food.FoodName;
            publishOrder.Event = "Order_Published";

            _messageBusClient.PublishOrder(publishOrder);

            return Ok();

        }
    }
}
