using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure;

namespace TodoApp.WebApp.Services.Maintenance
{
    public class DatabaseMigrator
    {
        private static bool _ensuredCreated = false;
        
        public DatabaseMigrator(AppDbContext appDbContext)
        {
            if (_ensuredCreated) return;
            
            appDbContext.Database.EnsureCreated();
            _ensuredCreated = true;
        }
    }
}