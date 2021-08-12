using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TodoApp.Infrastructure.Models;

namespace TodoApp.WebApp.Hubs
{
    [Authorize]
    public class TodoHub : Hub
    {
        public async Task Add(TodoItem todoItem, string userId)
        {
            await Clients.User(userId).SendAsync(nameof(Add), todoItem);
        }
        
        public async Task Delete(int[] todoItemsId, string userId)
        {
            await Clients.User(userId).SendAsync(nameof(Delete), todoItemsId);
        }
        
        public async Task ToggleDone(TodoItem[] todoItems, string userId)
        {
            await Clients.User(userId).SendAsync(nameof(ToggleDone), todoItems);
        }
        
        public async Task Edit(TodoItem todoItem, string userId)
        {
            await Clients.User(userId).SendAsync(nameof(Edit), todoItem);
        }
    }
}