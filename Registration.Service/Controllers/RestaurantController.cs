using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.Service.AsyncDataServices;
using Registration.Service.Models;
using Registration.Service.PublishItems;
using Registration.Service.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Registration.Service.Controllers
{
    [ApiController]
    [Route("api/v1/restaurants")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRepository<Restaurant> _restaurantRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public RestaurantController(
                    IRepository<Restaurant> restaurantRepository, 
                    IRepository<Address> addressRepository, 
                    IMessageBusClient messageBusClient,
                    IMapper mapper)
        {
            _restaurantRepository = restaurantRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> Get(int id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if(restaurant is null)
            {
                return BadRequest($"Restaurant with id: {id} does not exists!");
            }
            return Ok(restaurant);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> Get()
        {
            var restaurants = await _restaurantRepository.GetAllAsync();
            if(restaurants.Count() == 0)
            {
                return BadRequest("List of Restaurants is empty!");
            }
            return Ok(restaurants);
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreateRestaurantDto createRestaurantDto)
        {
            var jwt = GetToken();
            var company = jwt.Claims.Where(c => c.Type == "Company").FirstOrDefault()?.Value;
            if(company is null || company != createRestaurantDto.Company)
            {
                return Unauthorized();
            }

            var restaurantExists = await _restaurantRepository.Contains(r => r.Name == createRestaurantDto.Name);
            if(restaurantExists)
            {
                return BadRequest($"Restaurant: {createRestaurantDto.Name} already exists!");
            }
            var restaurant = _mapper.Map<Restaurant>(createRestaurantDto);

            var createdModel = await _restaurantRepository.CreateAsync(restaurant);

            if(createdModel is null)
            {
                return StatusCode(500, "Could not create restaurant!");
            }
            var publishRestaurant = new PublishRestaurant()
            {
                Name = createdModel.Name,
                Event = "Restaurant_Published"
            };
            _messageBusClient.PublishRestaurant(publishRestaurant);

            return CreatedAtAction(nameof(Get), new { id = createdModel.Id}, createdModel);
        }

        [HttpGet]
        [Route("{id}/addresses")]
        public async Task<ActionResult<IEnumerable<RestaurantWithAddressesDto>>> GetAddresses(int id)
        {
            var addresses = await _addressRepository.GetAllAsync(address => address.RestaurantId == id);
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if(addresses.Count() == 0)
            {
                return Ok($"Restaurant {restaurant.Name} does not have any addresses!");
            }

            var restaurantWithAddressesDto = new RestaurantWithAddressesDto(restaurant.Name!, addresses.ToList());
            return Ok(restaurantWithAddressesDto);
        }

        [HttpGet]
        [Route("{rid}/addresses/{aid}")]
        public async Task<ActionResult<IEnumerable<RestaurantWithAddressesDto>>> GetAddress(int rid, int aid)
        {
            var address = await _addressRepository.GetByIdAsync(aid);
            var restaurant = await _restaurantRepository.GetByIdAsync(rid);

            var restaurantWithAddressDto = new RestaurantWithAddressDto(restaurant.Name!, address);
            return Ok(restaurantWithAddressDto);
        }


        [HttpPost]
        [Route("address")]
        public async Task<ActionResult> AddAddress(CreateAddressDto createAddressDto)
        {
            bool restaurantExists = await _restaurantRepository.Contains(r => r.Id == createAddressDto.RestaurantId);
            if(!restaurantExists)
            {
                return BadRequest($"Restaurant with id: {createAddressDto.RestaurantId} does not exists!");
            }

            var address = _mapper.Map<Address>(createAddressDto);

            var createdAddress = await _addressRepository.CreateAsync(address);
            
            if(createdAddress is null)
            {
                return StatusCode(500, "Could not create restaurant!");
            }

            var restaurant = await _restaurantRepository.GetByIdAsync(createAddressDto.RestaurantId);
            var publishRestaurantAddress = _mapper.Map<PublishRestaurantAddress>(createdAddress);
            publishRestaurantAddress.Event = "Restaurant_Address_Published";
            publishRestaurantAddress.RestaurantName = restaurant.Name;

            _messageBusClient.PublishRestaurantAddress(publishRestaurantAddress);

            return CreatedAtAction(nameof(GetAddress), new {rid = createdAddress.RestaurantId, aid = createdAddress.Id}, createdAddress);
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if(restaurant is null)
            {
                return BadRequest($"Restaurant with id: {id} does not exists!");
            }

            await _restaurantRepository.DeleteAsync(restaurant);
            return Ok();
        }

        private JwtSecurityToken GetToken()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            string token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt;
        }

    }
}