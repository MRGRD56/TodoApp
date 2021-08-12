using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using TodoApp.Infrastructure.Models;

namespace TodoApp.DesktopClient.Services.ServerInterop
{
    public static class TodoHub
    {
        public static HubConnection Connection { get; }

        public static async Task EnsureConnectedAsync()
        {
            if (Connection.State == HubConnectionState.Disconnected)
            {
                await Connection.StartAsync();
            }
        }

        public static async Task Add(TodoItem todoItem) => await Connection.InvokeAsync("Add", todoItem);
        public static async Task Delete(int[] todoItemsId) => await Connection.InvokeAsync("Delete", todoItemsId);
        public static async Task ToggleDone(TodoItem[] todoItems) => await Connection.InvokeAsync("ToggleDone", todoItems);
        public static async Task Edit(TodoItem todoItem) => await Connection.InvokeAsync("Edit", todoItem);

        public static event Action<TodoItem> Added;
        public static event Action<int[]> Deleted;
        public static event Action<TodoItem[]> ToggledDone;
        public static event Action<TodoItem> Edited;

        static TodoHub()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl($"{ApiSettings.BaseUrl}hubs/todo")
                .WithAutomaticReconnect()
                .Build();

            Connection.Reconnecting += ConnectionOnReconnecting;
            Connection.Reconnected += ConnectionOnReconnected;

            Connection.On<TodoItem>("Add", todoItem => Added?.Invoke(todoItem));
            Connection.On<int[]>("Delete", ids => Deleted?.Invoke(ids));
            Connection.On<TodoItem[]>("ToggleDone", todoItems => ToggledDone?.Invoke(todoItems));
            Connection.On<TodoItem>("Edit", todoItem => Edited?.Invoke(todoItem));
        }

        private static Task ConnectionOnReconnected(string arg)
        {
            return Task.Run(() =>
            {
                AppState.IsLoading = false;
            });
        }

        private static Task ConnectionOnReconnecting(Exception arg)
        {
            return Task.Run(() =>
            {
                AppState.IsLoading = true;
            });
        }
    }
}
