using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using TodoApp.Infrastructure.Models;

namespace TodoApp.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }
        
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}