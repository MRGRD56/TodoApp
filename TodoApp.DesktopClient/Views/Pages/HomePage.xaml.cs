using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CheckLib;
using TodoApp.DesktopClient.ViewModels.PagesViewModels;
using TodoApp.Infrastructure.Models;

namespace TodoApp.DesktopClient.Views.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private HomePageViewModel ViewModel => (HomePageViewModel) DataContext;

        public HomePage()
        {
            InitializeComponent();
        }

        private void OnTodoItemClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            var element = (FrameworkElement) sender;
            element.Focus();
            var todoItem = (Checkable<TodoItem>) element.DataContext;
            ViewModel.OnTodoItemClick(todoItem);
        }

        private void OnTodoItemKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space) return;

            var todoItem = (Checkable<TodoItem>) ((FrameworkElement) sender).DataContext;
            ViewModel.OnTodoItemClick(todoItem);
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            var offset = e.VerticalOffset;
            var height = scrollViewer.ScrollableHeight;
            ViewModel.OnTodoItemsScroll(offset, height);
        }
    }
}
