using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.ServerInterop.Data
{
    public abstract class LocalDbContext : DbContext
    {
        private readonly string _dbFileDirectory;
        private const string DbFileName = "data.db";

        protected LocalDbContext(string dbFileDirectory)
        {
            _dbFileDirectory = dbFileDirectory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var fullFilePath = Path.Combine(_dbFileDirectory, DbFileName);
            optionsBuilder.UseSqlite($"Filename={fullFilePath}");
        }

        public DbSet<KeyValue> LocalStorage { get; set; }
    }
}
