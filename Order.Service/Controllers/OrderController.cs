using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Service.Models;
using Order.Service.Repository;
using static Order.Service.Dtos;

namespace Order.Service.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IRepository<FoodOrder> _orderRepository;

        public OrderController(IRepository<FoodOrder> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        [Route("for-restaurants")]
        public async Task<ActionResult> GetForRestaurants()
        {
            var ordersForRestaurants = await _orderRepository.GetAllAsync(o => o.Status == "Pending" || o.Status == "Executing");
            if(ordersForRestaurants.Count() == 0)
            {
                return BadRequest("List of orders for restaurants is empty!");
            }
            return Ok(ordersForRestaurants);
        }
        
        [HttpGet]
        [Route("for-deliveries")]
        public async Task<ActionResult> GetForDeliveries()
        {
            var ordersForDeliveries = await _orderRepository.GetAllAsync(o => o.Status == "Ready to deliver" || o.Status == "Executing");

            if(ordersForDeliveries.Count() == 0)
            {
                return BadRequest("List of orders for delivery is empty!");
            }
            return Ok(ordersForDeliveries);
        }

        [HttpPost]
        [Route("change-status")]
        public async Task<ActionResult> ChangeStatus(ChangeOrderStatusDto changeOrderStatusDto)
        {
            var order = await _orderRepository.GetByIdAsync(changeOrderStatusDto.Id);
            if(order is null)
            {
                return BadRequest("Order does not exists!");
            }

            order.Status = changeOrderStatusDto.OrderStatus;
            await _orderRepository.UpdateAsync(order);
            return Ok(order);
        }

    }
}
