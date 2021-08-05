using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TodoApp.DbInterop;
using TodoApp.Infrastructure;
using TodoApp.Infrastructure.Extensions;
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
            
            return todoItems;
        }

        /// <summary>
        /// Returns not deleted items after the element
        /// with the specified ID - <paramref name="afterId"/> (the element can be deleted)
        /// or from the beginning, sorting newest to oldest.
        /// </summary>
        /// <param name="afterId"></param>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<TodoItem>> GetAfterAsync(
            int? afterId, 
            int count = 10,
            CancellationToken cancellationToken = default)
        {
            var orderedItems = await
                Queryable.OrderByDescending(_db.TodoItems, item => item.CreationTime)
                .ToListAsync(cancellationToken);
            
            return orderedItems
                .SkipWhile(item => afterId.HasValue && item.Id != afterId)
                .Skip(afterId.HasValue ? 1 : 0)
                .NotDeleted()
                .Take(count)
                .ToList();
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
            var deletedItemsStream = SetDeletedAsync(new[] {todoItemId}, isDeleted, cancellationToken);
            var deletedItem = await deletedItemsStream.FirstAsync(cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return deletedItem;
        }

        public async IAsyncEnumerable<TodoItem> SetDeletedAsync(IEnumerable<int> todoItemsIds, bool isDeleted,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var todoItemId in todoItemsIds.ToImmutableArray())
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
                yield return todoItem;
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<TodoItem> DeleteAsync(int todoItemId, CancellationToken cancellationToken = default)
        {
            return await SetDeletedAsync(todoItemId, true, cancellationToken);
        }
        
        public async Task<TodoItem> RestoreAsync(int todoItemId, CancellationToken cancellationToken = default)
        {
            return await SetDeletedAsync(todoItemId, false, cancellationToken);
        }

        public async IAsyncEnumerable<TodoItem> ToggleDoneAsync(IEnumerable<int> todoItemsIds,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var todoItemId in todoItemsIds.ToImmutableArray())
            {
                var todoItem = await FindAsync(todoItemId, cancellationToken);

                if (todoItem.IsDeleted)
                {
                    throw new BadHttpRequestException("Cannot edit deleted todo-item");
                }

                todoItem.IsDone = !todoItem.IsDone;
                yield return todoItem;
            }

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}