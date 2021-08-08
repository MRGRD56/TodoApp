using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        private readonly UsersRepository _usersRepository;

        public TodoItemsRepository(AppDbContext db, UsersRepository usersRepository)
        {
            _db = db;
            _usersRepository = usersRepository;
        }

        private async Task<TodoItem> FindAsync(int todoItemId, CancellationToken cancellationToken = default)
        {
            await _db.Users.LoadAsync(cancellationToken);
            var todoItem = await _db.TodoItems.FindAsync(new object[] {todoItemId}, cancellationToken);
            if (todoItem == null)
            {
                throw new BadHttpRequestException("Todo item not found");
            }

            return todoItem;
        }

        public async Task<List<TodoItem>> GetAsync(
            int userId,
            int pageIndex,
            int pageItems = 10,
            CancellationToken cancellationToken = default)
        {
            await _db.Users.LoadAsync(cancellationToken);
            var todoItems = await _db.TodoItems
                .NotDeleted()
                .Where(t => t.UserId == userId)
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
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<TodoItem>> GetAfterAsync(
            int userId, 
            int? afterId,
            int count = 10,
            CancellationToken cancellationToken = default)
        {
            await _db.Users.LoadAsync(cancellationToken);
            var orderedItems = await
                Queryable.OrderByDescending(_db.TodoItems, item => item.CreationTime)
                    .ToListAsync(cancellationToken);
            
            return orderedItems
                .SkipWhile(item => afterId.HasValue && item.Id != afterId)
                .Skip(afterId.HasValue ? 1 : 0)
                .NotDeleted()
                .Where(t => t.UserId == userId)
                .Take(count)
                .ToList();
        }

        public async Task<TodoItem> AddAsync(int userId, string text, CancellationToken cancellationToken = default)
        {
            var todoItem = new TodoItem(text, userId);
            await _db.AddAsync(todoItem, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return todoItem;
        }

        public async Task<TodoItem> EditAsync(
            int userId,
            int todoItemId,
            string newText = null,
            bool? isDone = null,
            CancellationToken cancellationToken = default)
        {
            var todoItem = await FindAsync(todoItemId, cancellationToken);

            var user = await _usersRepository.GetByIdAsync(userId, cancellationToken);
            if (!user.Roles.Contains(Role.Admin) && todoItem.UserId != userId)
            {
                throw new HttpRequestException("Not enough rights for this action", null, HttpStatusCode.Forbidden);
            }
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

        public async Task<TodoItem> SetDeletedAsync(int userId, int todoItemId, bool isDeleted,
            CancellationToken cancellationToken = default)
        {
            var deletedItemsStream = SetDeletedAsync(userId, new[] {todoItemId}, isDeleted, cancellationToken);
            var deletedItem = await deletedItemsStream.FirstAsync(cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return deletedItem;
        }

        public async IAsyncEnumerable<TodoItem> SetDeletedAsync(int userId, IEnumerable<int> todoItemsIds, bool isDeleted,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var user = await _usersRepository.GetByIdAsync(userId, cancellationToken);
            
            foreach (var todoItemId in todoItemsIds.ToImmutableArray())
            {
                var todoItem = await FindAsync(todoItemId, cancellationToken);
                
                if (!user.Roles.Contains(Role.Admin) && todoItem.UserId != userId)
                {
                    throw new HttpRequestException("Not enough rights for this action", null, HttpStatusCode.Forbidden);
                }
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

        public async Task<TodoItem> DeleteAsync(int userId, int todoItemId, CancellationToken cancellationToken = default)
        {
            return await SetDeletedAsync(userId, todoItemId, true, cancellationToken);
        }
        
        public async Task<TodoItem> RestoreAsync(int userId, int todoItemId, CancellationToken cancellationToken = default)
        {
            return await SetDeletedAsync(userId, todoItemId, false, cancellationToken);
        }

        public async IAsyncEnumerable<TodoItem> ToggleDoneAsync(int userId, IEnumerable<int> todoItemsIds,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var user = await _usersRepository.GetByIdAsync(userId, cancellationToken);
            
            foreach (var todoItemId in todoItemsIds.ToImmutableArray())
            {
                var todoItem = await FindAsync(todoItemId, cancellationToken);
                
                if (!user.Roles.Contains(Role.Admin) && todoItem.UserId != userId)
                {
                    throw new HttpRequestException("Not enough rights for this action", null, HttpStatusCode.Forbidden);
                }
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