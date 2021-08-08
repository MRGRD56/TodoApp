using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure.Models;

namespace TodoApp.DbInterop
{
    public class AppDbContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<User> Users { get; set; }
        
        public AppDbContext() { }

        public AppDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .Property(u => u.Roles)
                .HasConversion(
                    r => ConvertRolesToString(r),
                    r => ConvertRolesFromString(r));
            modelBuilder.Entity<User>().HasData(new User("admin", "admin", Role.User, Role.Admin) { Id = 1 });
        }

        private static string ConvertRolesToString(IEnumerable<Role> roles) =>
            string.Join(',', roles.Select(role => (int) role));

        private static List<Role> ConvertRolesFromString(string rolesString)
        {
            return rolesString
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(roleString => (Role) int.Parse(roleString))
                .ToList();
        }
    }
}