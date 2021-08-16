using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using TodoApp.DesktopClient.Views.Windows;

namespace TodoApp.DesktopClient.Services
{
    public static class MainWindowNavigation
    {
        public static Frame NavigationFrame => WindowsManager.GetMainWindow().NavigationFrame;

        private static readonly Random Random = new();

        public static bool Navigate(object content, bool clearHistory = false)
        {
            var extraData = clearHistory
                ? new
                {
                    NavigationTime = DateTime.UtcNow.Ticks,
                    RandomId = Random.Next(int.MinValue, int.MaxValue)
                }
                : null;

            void NavigationFrameNavigated(object sender, NavigationEventArgs e)
            {
                try
                {
                    dynamic extra = e.ExtraData;
                    if (extra != null
                        && extra.NavigationTime == extraData.NavigationTime
                        && extra.RandomId == extraData.RandomId)
                    {
                        ClearBackStack();
                    }
                }
                catch (RuntimeBinderException) { }
                finally
                {
                    NavigationFrame.Navigated -= NavigationFrameNavigated;
                }
            }

            if (clearHistory)
            {
                NavigationFrame.Navigated += NavigationFrameNavigated;
            }
            var navigationResult = NavigationFrame.Navigate(content, extraData);
            return navigationResult;
        }

        public static bool NavigateNew<T>(bool clearHistory = false) where T : new()
        {
            return Navigate(new T(), clearHistory);
        }

        public static bool NavigateNew(Type type, bool clearHistory = false)
        {
            var obj = Activator.CreateInstance(type);
            return Navigate(obj, clearHistory);
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

        public static void ClearBackStack()
        {
            if (!NavigationFrame.CanGoBack && !NavigationFrame.CanGoForward)
            {
                return;
            }

            var entry = NavigationFrame.RemoveBackEntry();
            while (entry != null)
            {
                entry = NavigationFrame.RemoveBackEntry();
            }

            //NavigationFrame.Navigate(new PageFunction<string>() { RemoveFromJournal = true });
        }
    }
}
