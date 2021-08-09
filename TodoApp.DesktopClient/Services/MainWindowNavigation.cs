using System;
using System.Windows.Controls;
using TodoApp.DesktopClient.Views.Windows;

namespace TodoApp.DesktopClient.Services
{
    public static class MainWindowNavigation
    {
        public static Frame NavigationFrame => WindowsManager.GetMainWindow().NavigationFrame;

        public static bool Navigate(object content)
        {
            return NavigationFrame.Navigate(content);
        }

        public static bool NavigateNew<T>() where T : new()
        {
            return Navigate(new T());
        }

        public static bool GoBack()
        {
            if (!CanGoBack) return false;

            NavigationFrame.GoBack();
            return true;

        }

        public static bool GoForward()
        {
            if (!CanGoForward) return false;

            NavigationFrame.GoForward();
            return true;

        }

        public static bool CanGoBack => NavigationFrame.CanGoBack;
        public static bool CanGoForward => NavigationFrame.CanGoForward;
    }
}
