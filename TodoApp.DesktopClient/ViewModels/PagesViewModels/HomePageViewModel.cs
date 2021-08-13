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

        public bool IsItemsLoading
        {
            get => _isItemsLoading;
            private set
            {
                _isItemsLoading = value;
                OnPropertyChanged();
            }
        }

        public Checkable<TodoItem> EditingTodoItem
        {
            get => _editingTodoItem;
            set
            {
                _editingTodoItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(IsAddMode));
                OnPropertyChanged(nameof(IsActionButtonsShown));
            }
        }

        public bool IsEditMode => EditingTodoItem != null;

        public bool IsAddMode => !IsEditMode;

        private string _reservedNewTodoItemText = "";

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

        public bool IsTogglingDone
        {
            get => _isTogglingDone;
            set
            {
                if (value == _isTogglingDone) return;
                _isTogglingDone = value;
                OnPropertyChanged();
            }
        }

        public bool IsDeleting
        {
            get => _isDeleting;
            set
            {
                if (value == _isDeleting) return;
                _isDeleting = value;
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

        public bool IsActionButtonsShown => IsAddMode && HasSelectedItems;

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
                item.IsChecked = false;
                TodoItems.Remove(item);
            }
        }

        private void TodoHubOnAdded(TodoItem todoItem)
        {
            TodoItems.Insert(0, new Checkable<TodoItem>(todoItem, onChecked: OnTodoItemChecked));
        }

        public bool IsAllItemsLoaded
        {
            get => _isAllItemsLoaded;
            set
            {
                _isAllItemsLoaded = value;
                OnPropertyChanged();
            }
        }

        private int _lastItemId = 0;
        private string _newTodoItemText;
        private bool _isSubmitting;
        private bool _isTogglingDone;
        private bool _isDeleting;
        private Checkable<TodoItem> _editingTodoItem;
        private bool _isAllItemsLoaded;
        private bool _isItemsLoading;

        private async Task FetchItemsAsync()
        {
            if (IsAllItemsLoaded || IsItemsLoading) return;

            try
            {
                IsItemsLoading = true;

                var todoItems = await Todo.GetAfter(_lastItemId);

                if (!todoItems.Any())
                {
                    IsAllItemsLoaded = true;
                    IsItemsLoading = false;
                    return;
                }

                foreach (var todoItem in todoItems)
                {
                    TodoItems.Add(new Checkable<TodoItem>(todoItem, onChecked: OnTodoItemChecked));
                }

                _lastItemId = todoItems.Last().Id;
            }
            finally
            {
                IsItemsLoading = false;
            }
        }

        private void OnTodoItemChecked(object sender, CheckedEventArgs e)
        {
            OnPropertyChanged(nameof(HasSelectedItems));
            OnPropertyChanged(nameof(SelectedItemsCount));
            OnPropertyChanged(nameof(HasSingleSelectedItem));
            OnPropertyChanged(nameof(SelectedItemsCountString));
            OnPropertyChanged(nameof(IsActionButtonsShown));
        }

        public ICommand SubmitCommand => new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(NewTodoItemText)) return;

            if (IsEditMode)
            {
                await EditAsync();
            }
            else
            {
                await AddAsync();
            }
        });

        private async Task EditAsync()
        {
            try
            {
                IsSubmitting = true;
                if (EditingTodoItem.Item.Text != NewTodoItemText)
                {
                    var editedTodo = await Todo.Edit(EditingTodoItem.Item.Id, new TodoPutModel(NewTodoItemText, null));
                    await TodoHub.Edit(editedTodo);
                }
                EndEditing();
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        private async Task AddAsync()
        {
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
        }

        public ICommand ToggleDoneCommand => new Command(async () =>
        {
            if (!HasSelectedItems) return;

            IsTogglingDone = true;
            var body = new TodosPutModel(SelectedTodoItems.Select(x => x.Item.Id));
            var editedTodoItems = await Todo.ToggleDone(body); //TODO add loading
            await TodoHub.ToggleDone(editedTodoItems);
            IsTogglingDone = false;
        });

        public ICommand DeleteCommand => new Command(async () =>
        {
            if (!HasSelectedItems) return;

            IsDeleting = true;
            var body = new TodosDeleteModel(SelectedTodoItems.Select(x => x.Item.Id), false);
            var deletedTodoItems = await Todo.DeleteMany(body); //TODO add loading
            await TodoHub.Delete(deletedTodoItems.Select(x => x.Id).ToArray());
            IsDeleting = false;
        });

        public ICommand UnselectAllCommand => new Command(() =>
        {
            foreach (var item in SelectedTodoItems)
            {
                item.IsChecked = false;
            }
        });

        public ICommand CancelEditCommand => new Command(() =>
        {
            EndEditing();
        });

        private void EndEditing()
        {
            EditingTodoItem = null;
            NewTodoItemText = _reservedNewTodoItemText;
        }

        public ICommand EditCommand => new Command(() =>
        {
            EditingTodoItem = SelectedTodoItems.First();
            _reservedNewTodoItemText = NewTodoItemText;
            NewTodoItemText = EditingTodoItem.Item.Text;
        }, () => HasSingleSelectedItem);

        internal void OnTodoItemClick(Checkable<TodoItem> todoItem)
        {
            if (IsAddMode)
            {
                todoItem.Check();
            }
        }

        internal async void OnTodoItemsScroll(double offset, double height)
        {
            if (!IsItemsLoading && height - offset < 10)
            {
                await FetchItemsAsync();
            }
        }

        public ICommand LoadMoreItemsCommand => new Command(async () =>
        {
            await FetchItemsAsync();
        });
    }
}
