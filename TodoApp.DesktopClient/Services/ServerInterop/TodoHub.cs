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
        public static HubConnection Connection { get; private set; }

        private static string CurrentUserId => Auth.CurrentUser.Id.ToString();

        public static async Task Add(TodoItem todoItem) => await Connection.InvokeAsync("Add", todoItem, CurrentUserId);
        public static async Task Delete(int[] todoItemsId) => await Connection.InvokeAsync("Delete", todoItemsId, CurrentUserId);
        public static async Task ToggleDone(TodoItem[] todoItems) => await Connection.InvokeAsync("ToggleDone", todoItems, CurrentUserId);
        public static async Task Edit(TodoItem todoItem) => await Connection.InvokeAsync("Edit", todoItem, CurrentUserId);

        public static event Action<TodoItem> Added;
        public static event Action<int[]> Deleted;
        public static event Action<TodoItem[]> ToggledDone;
        public static event Action<TodoItem> Edited;

        static TodoHub()
        {
            //Auth.LoggedIn += async (_, _) =>
            //{
            //    AppState.IsConnecting = true;
            //    await StartNewConnectionAsync();
            //    AppState.IsConnecting = false;
            //};

            //Auth.LoggedOut += async (_, _) =>
            //{
            //    await RemoveConnectionAsync();
            //};
        }

        public static async Task RemoveConnectionAsync()
        {
            if (Connection == null) return;

            Connection.Reconnecting -= ConnectionOnReconnecting;
            Connection.Reconnected -= ConnectionOnReconnected;

            await Connection.StopAsync();
            Connection = null;
        }

        public static async Task StartNewConnectionAsync(bool recreate = false)
        {
            if (Connection != null && !recreate) return;

            if (!Auth.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Failed to connect to the todo hub because the user is unauthorised");
            }

            Connection = new HubConnectionBuilder()
                .WithUrl($"{ApiSettings.BaseUrl}hubs/todo", options =>
                {
                    options.UseDefaultCredentials = true;
                    options.AccessTokenProvider = async () => Auth.AccessToken;
                })
                .WithAutomaticReconnect()
                .Build();

            Connection.Reconnecting += ConnectionOnReconnecting;
            Connection.Reconnected += ConnectionOnReconnected;

            Connection.On<TodoItem>("Add", todoItem => Added?.Invoke(todoItem));
            Connection.On<int[]>("Delete", ids => Deleted?.Invoke(ids));
            Connection.On<TodoItem[]>("ToggleDone", todoItems => ToggledDone?.Invoke(todoItems));
            Connection.On<TodoItem>("Edit", todoItem => Edited?.Invoke(todoItem));

            await Connection.StartAsync();
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
