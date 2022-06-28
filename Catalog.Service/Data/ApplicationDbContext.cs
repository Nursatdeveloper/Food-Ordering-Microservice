using Microsoft.EntityFrameworkCore;
using Catalog.Service.Models;

namespace Catalog.Service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<FoodCategory> FoodCategories { get; set; } 
        public DbSet<Food> Foods { get; set; }
    }
}