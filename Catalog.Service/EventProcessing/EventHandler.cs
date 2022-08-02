using System.Text.Json;
using AutoMapper;
using Catalog.Service.Models;
using Catalog.Service.Repository;

namespace Catalog.Service.EventProcessing
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

        public async Task RestaurantPublised(string message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository<Restaurant>>();

                var restaurantPublishedDto = JsonSerializer.Deserialize<PublishRestaurantDto>(message);
                var restaurant = _mapper.Map<Restaurant>(restaurantPublishedDto);
                var createdRestaurant = await repository.CreateAsync(restaurant);
            }
        }

        public async Task RestaurantAddressPublished(string message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _addressRepository = scope.ServiceProvider.GetRequiredService<IRepository<Address>>();
                var _restaurantRepository = scope.ServiceProvider.GetRequiredService<IRepository<Restaurant>>();

                var restaurantAddressPublishedDto = JsonSerializer.Deserialize<PublishRestaurantAddressDto>(message);

                var restaurant = await _restaurantRepository.GetAsync(r => r.Name == restaurantAddressPublishedDto.RestaurantName);
                if(restaurant is null)
                {
                    Console.WriteLine("--> ERROR: Unable to add address to restaurant. Restaurant does not exist!");
                }
                else
                {
                    var restaurantAddress = _mapper.Map<Address>(restaurantAddressPublishedDto);
                    restaurantAddress.RestaurantId = restaurant.Id;
                    var createdRestaurantAddress = await _addressRepository.CreateAsync(restaurantAddress);
                }
            }
        }

        public async Task FoodCategoryPublished(string message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _foodCateoryRepository = scope.ServiceProvider.GetRequiredService<IRepository<FoodCategory>>();
                var _restaurantRepository = scope.ServiceProvider.GetRequiredService<IRepository<Restaurant>>();

                var foodCategoryPublishedDto = JsonSerializer.Deserialize<PublishFoodCategoryDto>(message);

                var restaurant = await _restaurantRepository.GetAsync(r => r.Name == foodCategoryPublishedDto.RestaurantName);

                if(restaurant is null)
                {
                    Console.WriteLine($"--> ERROR: Unable to add food category to restaurant! Restaurant does not exists!");
                }
                else
                {
                    var foodCategory = _mapper.Map<FoodCategory>(foodCategoryPublishedDto);
                    foodCategory.RestaurantId = restaurant.Id;
                    var createdFoodCategory = await _foodCateoryRepository.CreateAsync(foodCategory);
                }
            }
        }

        public async Task FoodPublished(string message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _foodRepository = scope.ServiceProvider.GetRequiredService<IRepository<Food>>();
                var _restaurantRepository = scope.ServiceProvider.GetRequiredService<IRepository<Restaurant>>();
                var _foodCateoryRepository = scope.ServiceProvider.GetRequiredService<IRepository<FoodCategory>>();

                var foodPublishedDto = JsonSerializer.Deserialize<PublishFoodDto>(message);

                var restaurant = await _restaurantRepository.GetAsync(r => r.Name == foodPublishedDto.RestaurantName);
                var foodCategory = await _foodCateoryRepository.GetAsync(c => c.CategoryName == foodPublishedDto.CategoryName);
                if (restaurant is null || foodCategory is null)
                {
                    Console.WriteLine("--> ERROR: Unable to create food. Restaurant or food category does not exist!");
                }
                else
                {
                    var food = _mapper.Map<Food>(foodPublishedDto);
                    food.CategoryId = foodCategory.Id;
                    food.RestaurantId = restaurant.Id;
                    var createdFood = await _foodRepository.CreateAsync(food);
                }

            }
        }

        public async Task RestaurantDeleted(string message)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var _restaurantRepository = scope.ServiceProvider.GetRequiredService<IRepository<Restaurant>>();

                var restaurant = JsonSerializer.Deserialize<PublishRestaurantDto>(message);

                var restaurantToDelete = await _restaurantRepository.GetAsync(r => r.Name == restaurant.Name);
                await _restaurantRepository.DeleteAsync(restaurantToDelete);
                
            }
        }
    }
}