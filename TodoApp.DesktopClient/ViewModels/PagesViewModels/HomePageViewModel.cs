using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Infrastructure.Models;

namespace TodoApp.DesktopClient.ViewModels.PagesViewModels
{
    public class HomePageViewModel : ViewModel
    {
        public ObservableCollection<TodoItem> TodoItems { get; } = new();

        public HomePageViewModel()
        {
            Initialize();
        }

        private async void Initialize()
        {
            await LoadTodoItemsAsync();
        }

        private async Task LoadTodoItemsAsync()
        {
            
        }
    }
}
