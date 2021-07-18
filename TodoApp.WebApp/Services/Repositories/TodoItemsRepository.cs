using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure;
using TodoApp.Infrastructure.Models;
using TodoApp.Infrastructure.Models.Abstractions;

namespace TodoApp.WebApp.Services.Repositories
{
    public class TodoItemsRepository
    {
        private readonly AppDbContext _db;

        public TodoItemsRepository(AppDbContext db)
        {
            _db = db;
        }

        private async Task<TodoItem> FindAsync(int todoItemId, CancellationToken cancellationToken = default)
        {
            var todoItem = await _db.TodoItems.FindAsync(new object[] {todoItemId}, cancellationToken);
            if (todoItem == null)
            {
                throw new BadHttpRequestException("Todo item not found");
            }

            return todoItem;
        }

        public async Task<List<TodoItem>> GetAsync(
            int pageIndex,
            int pageItems = 10,
            CancellationToken cancellationToken = default)
        {
            var todoItems = await _db.TodoItems
                .NotDeleted()
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
            var todoItem = await FindAsync(todoItemId, cancellationToken);

            if (todoItem.IsDeleted)
            {
                throw new BadHttpRequestException("Cannot edit deleted todo");
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

        public async Task<TodoItem> SetDeletedAsync(int todoItemId, bool isDeleted,
            CancellationToken cancellationToken = default)
        {
            var todoItem = await FindAsync(todoItemId, cancellationToken);

            if (todoItem.IsDeleted == isDeleted)
            {
                throw new BadHttpRequestException(
                    todoItem.IsDeleted
                        ? "Todo item is already deleted"
                        : "Cannot restore not deleted todo");
            }

            todoItem.IsDeleted = isDeleted;
            await _db.SaveChangesAsync(cancellationToken);
            return todoItem;
        }

        public async Task<TodoItem> DeleteAsync(int todoItemId, CancellationToken cancellationToken = default)
        {
            return await SetDeletedAsync(todoItemId, true, cancellationToken);
        }
        
        public async Task<TodoItem> RestoreAsync(int todoItemId, CancellationToken cancellationToken = default)
        {
            return await SetDeletedAsync(todoItemId, false, cancellationToken);
        }
    }
}