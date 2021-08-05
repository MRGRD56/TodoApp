using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure.Models;

namespace TodoApp.DbInterop
{
    public class AppDbContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}