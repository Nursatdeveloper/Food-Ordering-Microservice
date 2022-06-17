using Microsoft.EntityFrameworkCore;
using Registration.Service.Models;

namespace Registration.Service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}

        public DbSet<Restaurant>? Restaurants { get; set; }
        public DbSet<Address>? Addresses { get; set; }
        public DbSet<FoodCategory>? FoodCategories { get; set; } 
        public DbSet<Food>? Foods { get; set; }
        public DbSet<OrderStreamingConnection> OrderStreamingConnections { get; set; }
    }
}