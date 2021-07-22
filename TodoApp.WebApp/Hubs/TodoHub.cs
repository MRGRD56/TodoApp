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
    }
}