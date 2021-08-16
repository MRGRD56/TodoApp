using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using TodoApp.Infrastructure.Models;
using TodoApp.ServerInterop.Models.Events;

namespace TodoApp.ServerInterop
{
    public class TodoHub
    {
        private readonly Auth _auth;
        private HubConnection _connection;

        public TodoHub(Auth auth)
        {
            _auth = auth;

            _auth.LoggedIn += async (_, _) =>
            {
                await StartNewConnectionAsync();
            };

            _auth.LoggedOut += async (_, _) =>
            {
                await RemoveConnectionAsync();
            };
        }

        public HubConnection Connection
        {
            get => _connection;
            private set
            {
                _connection = value;
                ConnectionChanged?.Invoke(this, new HubConnectionChangedEventArgs(Connection));
            }
        }

        private string CurrentUserId => _auth.CurrentUser.Id.ToString(CultureInfo.InvariantCulture);

        public async Task Add(TodoItem todoItem) => await Connection.InvokeAsync("Add", todoItem, CurrentUserId);
        public async Task Delete(int[] todoItemsId) => await Connection.InvokeAsync("Delete", todoItemsId, CurrentUserId);
        public async Task ToggleDone(TodoItem[] todoItems) => await Connection.InvokeAsync("ToggleDone", todoItems, CurrentUserId);
        public async Task Edit(TodoItem todoItem) => await Connection.InvokeAsync("Edit", todoItem, CurrentUserId);

        public event Action<TodoItem> Added;
        public event Action<int[]> Deleted;
        public event Action<TodoItem[]> ToggledDone;
        public event Action<TodoItem> Edited;

        public async Task RemoveConnectionAsync()
        {
            if (Connection == null) return;

            //TODO!!!
            //Connection.Reconnecting -= ConnectionOnReconnecting;
            //Connection.Reconnected -= ConnectionOnReconnected;

            await Connection.StopAsync();
            Connection = null;
        }

        public async Task StartNewConnectionAsync(bool recreate = false)
        {
            if (Connection != null && !recreate) return;

            if (!_auth.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Failed to connect to the todo hub because the user is unauthorised");
            }

            Connection = new HubConnectionBuilder()
                .WithUrl($"{ApiSettings.BaseUrl}hubs/todo", options =>
                {
                    options.UseDefaultCredentials = true;
                    options.AccessTokenProvider = async () => _auth.AccessToken;
                })
                .WithAutomaticReconnect()
                .Build();

            //TODO!!!
            //Connection.Reconnecting += ConnectionOnReconnecting;
            //Connection.Reconnected += ConnectionOnReconnected;

            Connection.On<TodoItem>("Add", todoItem => Added?.Invoke(todoItem));
            Connection.On<int[]>("Delete", ids => Deleted?.Invoke(ids));
            Connection.On<TodoItem[]>("ToggleDone", todoItems => ToggledDone?.Invoke(todoItems));
            Connection.On<TodoItem>("Edit", todoItem => Edited?.Invoke(todoItem));

            await Connection.StartAsync();
        }

        public event EventHandler<HubConnectionChangedEventArgs> ConnectionChanged;

        //TODO!!!
        //private Task ConnectionOnReconnected(string arg)
        //{
        //    return Task.Run(() =>
        //    {
        //        AppState.IsLoading = false;
        //    });
        //}

        //private Task ConnectionOnReconnecting(Exception arg)
        //{
        //    return Task.Run(() =>
        //    {
        //        AppState.IsLoading = true;
        //    });
        //}
    }
}
