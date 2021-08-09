using System.Windows;
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
        }
    }
}