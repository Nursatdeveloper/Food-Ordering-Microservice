using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Registration.Service.AsyncDataServices;
using Registration.Service.Models;
using Registration.Service.PublishItems;
using Registration.Service.Repository;

namespace Registration.Service.Controllers
{
    [ApiController]
    [Route("api/v1/foods")]
    public class FoodController : ControllerBase
    {
        private readonly IRepository<Food> _foodRepository;
        private readonly IRepository<FoodCategory> _foodCategoryRepository;
        private readonly IRepository<Restaurant> _restaurantRepository;
        private readonly IMessageBusClient _messageBusClient;
        private readonly IMapper _mapper;

        public FoodController(IRepository<Food> foodRepository, 
                IRepository<FoodCategory> foodCategoryRepository,
                IRepository<Restaurant> restaurantRepository,
                IMessageBusClient messageBusClient,
                IMapper mapper)
        {
            _foodRepository = foodRepository;
            _foodCategoryRepository = foodCategoryRepository;
            _restaurantRepository = restaurantRepository;
            _messageBusClient = messageBusClient;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FoodViewDto>> Get(int id)
        {
            var food = await _foodRepository.GetByIdAsync(id);
            if(food is null)
            {
                return BadRequest($"Food with id: {id} does not exists!");
            }
            var foodVeiwDto = _mapper.Map<FoodViewDto>(food);
            return Ok(foodVeiwDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreateFoodDto createFoodDto)
        {
            var food = _mapper.Map<Food>(createFoodDto);

            var restaurantExists = await _restaurantRepository.Contains(r => r.Id == createFoodDto.RestaurantId);
            var categoryExists = await _foodCategoryRepository.Contains(c => c.Id == createFoodDto.CategoryId);

            if(restaurantExists && categoryExists)
            {
                var createdFood = await _foodRepository.CreateAsync(food);

                if(createdFood is null)
                {
                    return StatusCode(500, "Could not create food");
                }
                var restaurant = await _restaurantRepository.GetByIdAsync(createFoodDto.RestaurantId);
                var category = await _foodCategoryRepository.GetByIdAsync(createFoodDto.CategoryId);

                var publishFood = _mapper.Map<PublishFood>(createdFood);
                publishFood.CategoryName = category.CategoryName;
                publishFood.RestaurantName = restaurant.Name;
                publishFood.Event = "Food_Published";
                _messageBusClient.PublishFood(publishFood);

                return CreatedAtAction(nameof(Get), new { id = createdFood.Id }, createdFood);
            }
            return BadRequest($"Restaurant (id: {createFoodDto.RestaurantId}) or Food Category (id: {createFoodDto.CategoryId}) does not exists");

        }

        [HttpGet]
        [Route("category/{id}")]
        public async Task<ActionResult<FoodCategoryViewDto>> GetFoodCategory(int id)
        {
            var foodCategory = await _foodCategoryRepository.GetByIdAsync(id);
            if(foodCategory is null)
            {
                return BadRequest($"Could not find food category with id: {id}");
            }
            var foodCategoryViewDto = _mapper.Map<FoodCategoryViewDto>(foodCategory);
            return Ok(foodCategoryViewDto);
        }

        [HttpPost]
        [Route("category")]
        public async Task<ActionResult> Post(CreateFoodCategoryDto createFoodCategoryDto)
        {
            var foodCategoryExists = await _foodCategoryRepository.Contains(c => 
                c.CategoryName == createFoodCategoryDto.CategoryName &&
                c.RestaurantId == createFoodCategoryDto.RestaurantId);
            if(foodCategoryExists)
            {
                return BadRequest($"Food Category ({createFoodCategoryDto.CategoryName}) already exists!");
            }

            var foodCategory = _mapper.Map<FoodCategory>(createFoodCategoryDto);
            var restaurantExists = await _restaurantRepository.Contains(r => r.Id == createFoodCategoryDto.RestaurantId);

            if(restaurantExists)
            {
                var createdFoodCategory = await _foodCategoryRepository.CreateAsync(foodCategory);

                if(createdFoodCategory is null)
                {
                    return StatusCode(500, "Could not create food category");
                }

                var restaurant = await _restaurantRepository.GetByIdAsync(createFoodCategoryDto.RestaurantId);

                var publishFoodCategory = _mapper.Map<PublishFoodCategory>(createdFoodCategory);
                publishFoodCategory.Event = "Food_Category_Published";
                publishFoodCategory.RestaurantName = restaurant.Name;
                _messageBusClient.PublishFoodCategory(publishFoodCategory);
                
                return CreatedAtAction(nameof(GetFoodCategory), new { id = createdFoodCategory.Id }, createdFoodCategory);
            }
            return BadRequest($"Restaurant with Id: {createFoodCategoryDto.RestaurantId} does not exists");

        }
    }
}