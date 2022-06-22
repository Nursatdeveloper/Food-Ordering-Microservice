using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Order.Service.Hubs;
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
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderController(IRepository<FoodOrder> orderRepository,
            IHubContext<OrderHub> hubContext)
        {
            _orderRepository = orderRepository;
            _hubContext = hubContext;
        }

        [HttpPost]
        [Route("accept-for-delivery")]
        public async Task<ActionResult> AcceptForDelivery(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            order.DeliveryCode = GenerateDeliveryCode();
            await _orderRepository.UpdateAsync(order);

            string room = $"{order.RestaurantName} {order.RestaurantAddress}";
            await _hubContext.Clients.Group(room).SendAsync("AcceptedByDelivery", order);
            return Ok(order);
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

        private string GenerateDeliveryCode()
        {
            string deliveryCode = "";
            char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            Random random = new Random();
            for(int i = 0; i < 6; i++)
            {
                int index = random.Next(0, chars.Length - 1);
                deliveryCode += chars[index];
            }
            return deliveryCode;
        }

    }
}
