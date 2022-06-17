using Microsoft.EntityFrameworkCore;
using Order.Service.Models;

namespace Order.Service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}

        public DbSet<FoodOrder> FoodOrders { get; set; }
        public DbSet<OrderStreamingConnection> OrderStreamingConnections { get; set; }

    }
}