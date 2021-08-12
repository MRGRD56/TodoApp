using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CheckLib;
using MgMvvmTools;
using TodoApp.DesktopClient.Services;
using TodoApp.DesktopClient.Services.ServerInterop;
using TodoApp.DesktopClient.Views.Pages;
using TodoApp.Infrastructure.Models;
using TodoApp.Infrastructure.Models.RequestModels.Todo;

namespace TodoApp.DesktopClient.ViewModels.PagesViewModels
{
    public class HomePageViewModel : ViewModel
    {
        public ObservableCollection<Checkable<TodoItem>> TodoItems { get; } = new();

        private IEnumerable<Checkable<TodoItem>> SelectedTodoItems => TodoItems.GetChecked().ToList();

        public bool HasSelectedItems => SelectedTodoItems.Any();

        public int SelectedItemsCount => SelectedTodoItems.Count();

        public bool HasSingleSelectedItem => SelectedItemsCount == 1;

        public string SelectedItemsCountString => $"{SelectedItemsCount} {(SelectedItemsCount == 1 ? "item" : "items")}";

        public bool IsItemsLoading { get; private set; }

        public string NewTodoItemText
        {
            get => _newTodoItemText;
            set
            {
                _newTodoItemText = value;
                OnPropertyChanged();
            }
        }

        public bool IsSubmitting
        {
            get => _isSubmitting;
            set
            {
                _isSubmitting = value;
                OnPropertyChanged();
            }
        }

        public HomePageViewModel()
        {
            if (!Auth.IsAuthenticated)
            {
                MainWindowNavigation.NavigateNew<LoginPage>();
            }
            Initialize();
        }

        private async void Initialize()
        {
            await FetchItemsAsync();

            TodoHub.Added += TodoHubOnAdded;
            TodoHub.Deleted += TodoHubOnDeleted;
            TodoHub.ToggledDone += TodoHubOnToggledDone;
            TodoHub.Edited += TodoHubOnEdited;
        }

        private void TodoHubOnEdited(TodoItem todoItem)
        {
            var itemToEdit = TodoItems.FirstOrDefault(ti => ti.Item.Id == todoItem.Id);
            if (itemToEdit != null)
            {
                itemToEdit.Item.Text = todoItem.Text;
            }
        }

        private void TodoHubOnToggledDone(TodoItem[] todoItems)
        {
            foreach (var editedItem in todoItems)
            {
                var todoItem = TodoItems.FirstOrDefault(ti => ti.Item.Id == editedItem.Id);
                if (todoItem == null) continue;
                todoItem.Item.IsDone = editedItem.IsDone;
            }
        }

        private void TodoHubOnDeleted(int[] ids)
        {
            var itemsToRemove = TodoItems.Where(ti => ids.Contains(ti.Item.Id)).ToArray();
            foreach (var item in itemsToRemove)
            {
                TodoItems.Remove(item);
            }
        }

        private void TodoHubOnAdded(TodoItem todoItem)
        {
            TodoItems.Insert(0, new Checkable<TodoItem>(todoItem, onChecked: OnTodoItemChecked));
        }

        private int _lastItemId = 0;
        private bool _isAllItemsLoaded = false;
        private string _newTodoItemText;
        private bool _isSubmitting;

        private async Task FetchItemsAsync()
        {
            if (_isAllItemsLoaded) return;
            IsItemsLoading = true;

            var todoItems = await Todo.GetAfter(_lastItemId);

            if (!todoItems.Any())
            {
                _isAllItemsLoaded = true;
                IsItemsLoading = false;
                return;
            }

            foreach (var todoItem in todoItems)
            {
                TodoItems.Add(new Checkable<TodoItem>(todoItem, onChecked: OnTodoItemChecked));
            }

            _lastItemId = todoItems.Last().Id;

            IsItemsLoading = false;
        }

        private void OnTodoItemChecked(object sender, CheckedEventArgs e)
        {
            OnPropertyChanged(nameof(HasSelectedItems));
            OnPropertyChanged(nameof(SelectedItemsCount));
            OnPropertyChanged(nameof(HasSingleSelectedItem));
            OnPropertyChanged(nameof(SelectedItemsCountString));
        }

        public void OnTodoItemClick(Checkable<TodoItem> todoItem)
        {
            todoItem.Check();
        }

        public ICommand AddCommand => new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(NewTodoItemText)) return;
            try
            {
                IsSubmitting = true;
                var addedTodo = await Todo.Add(new TodoPostModel(NewTodoItemText));
                await TodoHub.Add(addedTodo);
                NewTodoItemText = "";
            }
            finally
            {
                IsSubmitting = false;
            }
        });
    }
}
