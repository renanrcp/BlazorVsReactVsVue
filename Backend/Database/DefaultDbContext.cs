using Backend.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database
{
    public class DefaultDbContext : DbContext
    {
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}