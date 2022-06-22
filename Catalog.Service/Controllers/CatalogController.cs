using Catalog.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Catalog.Service.Repository;
using Grpc.Net.Client;
using Image.Grpc.Service;

namespace Catalog.Service.Controllers
{
    [ApiController]
    [Route("api/v1/catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly IRepository<Restaurant> _restaurantRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<FoodCategory> _foodCategoryRepository;
        private readonly IRepository<Food> _foodRepository;

        public CatalogController(IRepository<Restaurant> restaurantRepository,
                IRepository<Address> addressRepository,
                IRepository<FoodCategory> foodCategoryRepository,
                IRepository<Food> foodRepository)
        {
            _restaurantRepository = restaurantRepository;
            _addressRepository = addressRepository;
            _foodCategoryRepository = foodCategoryRepository;
            _foodRepository = foodRepository;
        }

        [HttpGet]
        [Route("restaurants")]
        public async Task<ActionResult> Get()
        {
            var restaurants = await _restaurantRepository.GetAllAsync();
            if(restaurants.Count() == 0)
            {
                return NotFound();
            }

            var restaurantWithImageList = new List<RestaurantWithImageDto>();
            foreach (var restaurant in restaurants)
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5061");
                var grpcClient = new Images.ImagesClient(channel);
                var restaurantImageReply = await grpcClient.GetRestaurantImageAsync(new GetRestaurantImageRequest 
                { 
                    Restaurant = restaurant.Name 
                });
                if(restaurantImageReply is null)
                {
                    var restaurantWithImage = new RestaurantWithImageDto(restaurant.Id, restaurant.Name, null);
                    restaurantWithImageList.Add(restaurantWithImage);
                }
                else
                {
                    var restaurantWithImage = new RestaurantWithImageDto(restaurant.Id, restaurant.Name,
                    restaurantImageReply.Image.ToArray());
                    restaurantWithImageList.Add(restaurantWithImage);
                }          
            }
            return Ok(restaurantWithImageList);
        }

        [HttpGet]
        [Route("restaurants/{restaurantId}/addresses")]
        public async Task<ActionResult<RestaurantWithAddressDto>> GetRestaurantAddresses(int restaurantId, string city)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);
            if(restaurant is null)
            {
                return NotFound($"Restaurant with id: {restaurantId} does not exists!");
            }

            var addresses = await _addressRepository.GetAllAsync(a => a.RestaurantId == restaurantId && a.City == city);
            if(addresses.Count() == 0)
            {
                return NotFound("Addresses with specified parameters do not exist!");
            }

            var restaurantWithAddressDto = new RestaurantWithAddressDto(restaurant.Name, city, addresses.ToList());
            return Ok(restaurantWithAddressDto);
        }

        [HttpGet]
        [Route("restaurants/{restaurantId}/food-categories")]
        public async Task<ActionResult<RestaurantWithFoodCategoryDto>> GetRestaurantFoodCategory(int restaurantId)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);
            if (restaurant is null)
            {
                return NotFound($"Restaurant with id: {restaurantId} does not exists!");
            }

            var foodCategories = await _foodCategoryRepository.GetAllAsync(c => c.RestaurantId == restaurantId);
            if(foodCategories.Count() == 0)
            {
                return NotFound("Restaurant does not have any food categories!");
            }

            var foodCategoriesWithImageList = new List<FoodCategoriesWithImageDto>();
            foreach(var category in foodCategories)
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5061");
                var grpcClient = new Images.ImagesClient(channel);
                var foodCategoryImageReply = await grpcClient.GetFoodCategoryImageAsync(new GetFoodCategoryImageRequest
                {
                    Restaurant = restaurant.Name,
                    Category = category.CategoryName
                });
                if(foodCategoryImageReply is null)
                {
                    var foodCategoryWithImage = new FoodCategoriesWithImageDto(
                        category.Id, category.CategoryName, null);
                    foodCategoriesWithImageList.Add(foodCategoryWithImage);
                }
                else
                {
                    var foodCategoryWithImage = new FoodCategoriesWithImageDto(
                        category.Id, category.CategoryName, foodCategoryImageReply.Image.ToArray());
                    foodCategoriesWithImageList.Add(foodCategoryWithImage);
                }
            }
            var restaurantWithFoodCategories = new RestaurantWithFoodCategoryDto(restaurant.Name, foodCategoriesWithImageList);
            return Ok(restaurantWithFoodCategories);
        }

        [HttpGet]
        [Route("restaurants/{restaurantId}/food-categories/{categoryId}/foods")]
        public async Task<ActionResult<FoodsViewDto>> GetFoods(int restaurantId, int categoryId)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);
            if (restaurant is null)
            {
                return NotFound($"Restaurant with id: {restaurantId} does not exists!");
            }

            var foodCategory = await _foodCategoryRepository.GetByIdAsync(categoryId);
            if(foodCategory is null)
            {
                return NotFound($"Food category with id: {categoryId} does not exists!");
            }

            var foods = await _foodRepository.GetAllAsync(f => f.RestaurantId == restaurantId && f.CategoryId == categoryId);
            if(foods.Count() == 0)
            {
                return NotFound($"Category '{foodCategory.CategoryName}' does not have any foods!");
            }
            List<FoodWithImageDto> foodWithImageList = new List<FoodWithImageDto>();

            foreach(var food in foods)
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5061");
                var grpcClient = new Images.ImagesClient(channel);
                var result = await grpcClient.GetFoodImageAsync(new GetFoodImageRequest
                {
                    Restaurant = restaurant.Name,
                    Food = food.FoodName
                });

                var foodWithImageDto = new FoodWithImageDto(food.Id, food.FoodName, food.Ingredients,
                    food.Price, food.PreparationTime, result.Image.ToArray());

                foodWithImageList.Add(foodWithImageDto);
            }

            var foodViewDto = new FoodsViewDto(restaurant.Name, foodCategory.CategoryName, foodWithImageList);
            return Ok(foodViewDto);
        }

    }
}