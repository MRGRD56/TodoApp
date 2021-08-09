using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.DesktopClient.Data
{
    public sealed class LocalDbContext : DbContext
    {
        public LocalDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=data.db;Cache=Shared");
        }

        public DbSet<KeyValue> LocalStorage { get; set; }
    }
}
