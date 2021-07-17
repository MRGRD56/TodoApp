using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure.Models;

namespace TodoApp.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }

        protected AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}