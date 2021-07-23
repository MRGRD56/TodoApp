using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TodoApp.Infrastructure.Models;

namespace TodoApp.WebApp.Hubs
{
    public class TodoHub : Hub
    {
        public async Task Add(TodoItem todoItem)
        {
            await Clients.All.SendAsync(nameof(Add), todoItem);
        }

        public async Task Delete(int[] todoItemsId)
        {
            await Clients.All.SendAsync(nameof(Delete), todoItemsId);
        }

        public async Task ToggleDone(TodoItem[] todoItems)
        {
            await Clients.All.SendAsync(nameof(ToggleDone), todoItems);
        }

        public async Task Edit(TodoItem todoItem)
        {
            await Clients.All.SendAsync(nameof(Edit), todoItem);
        }
    }
}