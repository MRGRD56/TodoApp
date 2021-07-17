using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure;
using TodoApp.Infrastructure.Models;
using TodoApp.Infrastructure.Models.RequestModels.Todo;

namespace TodoApp.WebApp.Services.Repositories
{
    public class TodoItemsRepository
    {
        private readonly AppDbContext _db;

        public TodoItemsRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<TodoItem>> GetAsync(
            int pageIndex,
            int pageItems = 10, 
            CancellationToken cancellationToken = default)
        {
            var todoItems = await _db.TodoItems
                .OrderByDescending(todoItem => todoItem.CreationTime)
                .Skip(pageIndex * pageItems)
                .Take(pageItems)
                .ToListAsync(cancellationToken);

            // return todoItems.Any()
            //     ? todoItems
            //     : throw new HttpRequestException(
            //         "The specified page contains no elements", 
            //         null, 
            //         HttpStatusCode.NotFound);

            return todoItems;
        }

        public async Task<TodoItem> AddAsync(string text, CancellationToken cancellationToken = default)
        {
            var todoItem = new TodoItem(text);
            await _db.AddAsync(todoItem, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return todoItem;
        }

        public async Task<TodoItem> EditAsync(
            int todoItemId,
            string newText = null, 
            bool? isDone = null,
            CancellationToken cancellationToken = default)
        {
            var todoItem = await _db.TodoItems.FindAsync(new object[] { todoItemId }, cancellationToken);
            if (todoItem == null)
            {
                throw new HttpRequestException("TODO-item not found", null, HttpStatusCode.BadRequest);  
            }

            if (newText != null)
            {
                todoItem.Text = newText;
            }

            if (isDone != null)
            {
                todoItem.IsDone = isDone.Value;
            }

            await _db.SaveChangesAsync(cancellationToken);
            return todoItem;
        }
    }
}