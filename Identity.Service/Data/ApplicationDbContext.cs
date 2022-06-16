using Identity.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

    }
}
