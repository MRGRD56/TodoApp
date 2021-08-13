using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TodoApp.DesktopClient.Data;

namespace TodoApp.DesktopClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            using var localDbContext = new LocalDbContext();
            localDbContext.Database.EnsureCreated();

            DispatcherUnhandledException += OnDispatcherUnhandledException;
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