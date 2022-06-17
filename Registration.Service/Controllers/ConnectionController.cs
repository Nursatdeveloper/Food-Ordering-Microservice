using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Registration.Service.AsyncDataServices;
using Registration.Service.Models;
using Registration.Service.PublishItems;
using Registration.Service.Repository;
using System.IdentityModel.Tokens.Jwt;

namespace Registration.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionController : ControllerBase
    {
        private readonly IRepository<OrderStreamingConnection> _repository;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public ConnectionController(IRepository<OrderStreamingConnection> repository, 
            IMapper mapper,
            IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        [Route("order-streaming")]
        public async Task<ActionResult> GetOrderStreamingConnections(string company, string city)
        {
            var jwt = GetToken();
            var companyObj = jwt.Claims.Where(c => c.Type == "Company").FirstOrDefault()?.Value;
            if (companyObj is null || companyObj != company)
            {
                return Unauthorized();
            }
            var connections = await _repository.GetAllAsync(c => c.Company == company && c.City == city);
            if(connections.Count() == 0)
            {
                return NotFound("Could not find any order streaming connection!");
            }
            return Ok(connections);
        }

        [HttpPost]
        [Route("order-streaming")]
        public async Task<ActionResult> Post(CreateOrderStreamingConnection createOrderStreamingConnection)
        {
            var jwt = GetToken();
            var company = jwt.Claims.Where(c => c.Type == "Company").FirstOrDefault()?.Value;
            if (company is null || company != createOrderStreamingConnection.Company)
            {
                return Unauthorized();
            }
            bool connectionExists = await _repository.Contains(c => c.Company == createOrderStreamingConnection.Company
                                                                && c.RestaurantName == createOrderStreamingConnection.RestaurantName
                                                                && c.City == createOrderStreamingConnection.City
                                                                && c.Address == createOrderStreamingConnection.Address);
            if(connectionExists)
            {
                return BadRequest("Order streaming connection already exists");
            }
                 
            var connection = _mapper.Map<OrderStreamingConnection>(createOrderStreamingConnection);
            connection.ConnectionPassword = BCrypt.Net.BCrypt.HashPassword(connection.ConnectionPassword);
            var createdConnection = await _repository.CreateAsync(connection);
            if(createdConnection is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not create!");
            }

            var publishOrderStreamingConnection = _mapper.Map<PublishOrderStreamingConnection>(createdConnection);
            publishOrderStreamingConnection.Event = "OrderStreamingConnection_Published";
            _messageBusClient.PublishOrderStreamingConnection(publishOrderStreamingConnection);

            return CreatedAtAction(nameof(GetOrderStreamingConnections), createdConnection);

        }
        [HttpDelete]
        [Route("order-streaming/{id}")]
        public async Task<ActionResult> DeleteOrderStreamingConnection(int id)
        {
            var orderStreamingConnection = await _repository.GetByIdAsync(id);
            await _repository.DeleteAsync(orderStreamingConnection);
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
