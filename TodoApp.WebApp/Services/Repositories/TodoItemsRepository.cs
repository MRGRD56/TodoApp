using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure;
using TodoApp.Infrastructure.Models;

namespace TodoApp.WebApp.Services.Repositories
{
    public class TodoItemsRepository
    {
        private readonly AppDbContext _db;

        public TodoItemsRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<TodoItem>> GetAsync(int pageIndex, int pageItems = 10)
        {
            return await _db.TodoItems
                .OrderByDescending(todoItem => todoItem.CreationTime)
                .Skip(pageIndex * pageItems)
                .Take(pageItems)
                .ToListAsync();
        }
    }
}