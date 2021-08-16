using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TodoApp.Infrastructure.Models.RequestModels.Auth;
using TodoApp.DesktopClient.Models;
using TodoApp.ServerInterop;
using TodoApp.DesktopClient.Services;
using TodoApp.ServerInterop.Models.Events;
using TodoApp.ClientLocalDb;

namespace TodoApp.DesktopClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILocalDbContextFactory LocalDbContextFactory;
        internal static Auth Auth { get; }
        internal static Todo Todo { get; }
        internal static TodoHub TodoHub { get; }

        static App()
        {
            LocalDbContextFactory = new WindowsLocalDbContextFactory();
            Auth = new Auth(LocalDbContextFactory);
            Todo = new Todo(Auth);
            TodoHub = new TodoHub(Auth);
            TodoHub.ConnectionChanged += TodoHubOnConnectionChanged;
        }

        public App()
        {
            using var localDbContext = LocalDbContextFactory.Create();
            localDbContext.Database.EnsureCreated();

            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private static void TodoHubOnConnectionChanged(object sender, HubConnectionChangedEventArgs e)
        {
            if (!e.HasConnection) return;
            e.Connection.Reconnecting += ConnectionOnReconnecting;
            e.Connection.Reconnected += ConnectionOnReconnected;
        }

        private static Task ConnectionOnReconnecting(Exception arg)
        {
            return Task.Run(() =>
            {
                AppState.IsLoading = true;
            });
        }

        private static Task ConnectionOnReconnected(string arg)
        {
            return Task.Run(() =>
            {
                AppState.IsLoading = false;
            });
        }
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
#if !DEBUG
            var exception = e.Exception;
            var message = exception.Message;
            var innerException = exception.InnerException;
            while (innerException != null)
            {
                if (innerException is not TaskCanceledException)
                {
                    message += $"\n{innerException.Message}";
                }
                innerException = innerException.InnerException;
            }
            e.Handled = true;
            MessageBox.Show(message, exception.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
#endif
        }
    }
}