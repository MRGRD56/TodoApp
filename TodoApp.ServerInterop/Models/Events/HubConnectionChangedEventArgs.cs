using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace TodoApp.ServerInterop.Models.Events
{
    public class HubConnectionChangedEventArgs : EventArgs
    {
        public bool HasConnection => Connection != null;

        public HubConnection Connection { get; }

        public HubConnectionChangedEventArgs(HubConnection connection)
        {
            Connection = connection;
        }
    }
}
