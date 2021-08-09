using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TodoApp.DesktopClient.Views.Windows;

namespace TodoApp.DesktopClient.Services
{
    public static class WindowsManager
    {
        private static Application CurrentApplication => Application.Current;

        public static IEnumerable<TWindow> Get<TWindow>() where TWindow : Window
        {
            foreach (var window in CurrentApplication.Windows.Cast<Window>().ToArray())
            {
                if (window is TWindow windowT)
                {
                    yield return windowT;
                }
            }
        }

        public static TWindow GetFirstOrDefault<TWindow>() where TWindow : Window => 
            Get<TWindow>().FirstOrDefault();

        public static MainWindow GetMainWindow() => 
            (MainWindow) CurrentApplication.MainWindow;
    }
}
