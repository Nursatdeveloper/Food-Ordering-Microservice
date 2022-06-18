using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Order.Service.Data;
using Order.Service.Migrations;
using Order.Service.Models;
using Order.Service.Repository;
using Order.Service.SignalR;
using System.Linq.Expressions;

namespace Order.Service.Hubs
{
    public class OrderHub : Hub
    {
        private readonly IDictionary<string, ConnectionParameters> _connections;
        private readonly ApplicationDbContext _context;
        public OrderHub(IDictionary<string, ConnectionParameters> connections,
            ApplicationDbContext context)
        {
            _connections = connections;
            _context = context;
        }

        public async Task JoinOrderStreaming(ConnectionParameters connection)
        {
            var orderStreamingConnection = await _context.OrderStreamingConnections.Where(o =>
                o.RestaurantName == connection.RestaurantName &&
                o.Address == connection.Address &&
                o.City == connection.City).FirstOrDefaultAsync();
            if(orderStreamingConnection is null)
            {
                Console.WriteLine("Order streaming connection does not exists!");
            }
            else
            {
                string room = $"{connection.RestaurantName} {connection.City},  {connection.Address}";
                Console.WriteLine(room);
                if(BCrypt.Net.BCrypt.Verify(connection.Password, orderStreamingConnection.ConnectionPassword))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, room);
                    _connections[Context.ConnectionId] = connection;

                    await SendAvailableOrders(connection, room); 
                }
                else
                {
                    await Clients.Group(room).SendAsync("IncorrectPassword", "Password is incorrect");
                }
            }
        }

        public Task SendAvailableOrders(ConnectionParameters parameters, string room)
        {
            string formattedAddress = $"{parameters.City},  {parameters.Address}";
            var orders =  _context.FoodOrders.Where(o => 
                o.RestaurantAddress == formattedAddress &&
                o.RestaurantName == parameters.RestaurantName).ToList();
            var ordersForRestaurants = orders.Where(o => o.Status == "Pending" || o.Status == "Executing").ToList();

            return Clients.Group(room).SendAsync("AvailableOrders", ordersForRestaurants);
        }

        public async Task ChangeStatus(ConnectionParameters connection, int id, string status)
        {
            string room = $"{connection.RestaurantName} {connection.City},  {connection.Address}";
            var order = await _context.FoodOrders.FindAsync(id);
            order.Status = status;
            var updatedOrder = _context.FoodOrders.Update(order);
            await _context.SaveChangesAsync();

            await Clients.Group(room).SendAsync("StatusUpdated", updatedOrder.Entity);
        }
    }
}
