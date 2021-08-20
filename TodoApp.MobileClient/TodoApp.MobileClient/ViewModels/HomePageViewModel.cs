using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CheckLib;
using TodoApp.Infrastructure.Models;
using TodoApp.Infrastructure.Models.RequestModels.Todo;
using TodoApp.MobileClient.Extensions;
using TodoApp.MobileClient.Views;
using Xamarin.Forms;

namespace TodoApp.MobileClient.ViewModels
{
    public class HomePageViewModel : ViewModel
    {
        private readonly HomePage _homePage;
        private ObservableCollection<ToolbarItem> ToolbarItems { get; } = new ObservableCollection<ToolbarItem>();

        private IEnumerable<ToolbarItem> AllToolbarItems => new[]
            {
                (ImageSource: "baseline_edit_24.xml", Command: EditCommand),
                (ImageSource: "baseline_done_24.xml", Command: ToggleDoneCommand),
                (ImageSource: "baseline_delete_24.xml", Command: DeleteCommand),
                (ImageSource: "baseline_clear_24.xml", Command: UnselectAllCommand)
            }
            .Select(x => new ToolbarItem
            {
                IconImageSource = ImageSource.FromFile(x.ImageSource),
                Command = x.Command
            })
            .ToArray();

        public ObservableCollection<Checkable<TodoItem>> TodoItems { get; } = new ObservableCollection<Checkable<TodoItem>>();

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
                OnPropertyChanged(nameof(IsNotItemsLoading));
            }
        }

        public bool IsNotItemsLoading => !IsItemsLoading;

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

        public HomePageViewModel(HomePage homePage)
        {
            _homePage = homePage;
            if (!App.Auth.IsAuthenticated)
            {
                App.GetMainPage().Navigation.PushNewModalAsync<LoginPage>();
                return;
            }
            Initialize();
            ToolbarItems.CollectionChanged += ToolbarItemsOnCollectionChanged;
        }

        private void ToolbarItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    _homePage.ToolbarItems.Clear();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var oldItem in e.OldItems)
                    {
                        _homePage.ToolbarItems.Remove((ToolbarItem)oldItem);
                    }

                    break;
                case NotifyCollectionChangedAction.Add:
                    foreach (var oldItem in e.NewItems)
                    {
                        _homePage.ToolbarItems.Add((ToolbarItem)oldItem);
                    }

                    break;
            }
        }

        public bool IsActionButtonsShown => IsAddMode && HasSelectedItems;

        private async void Initialize()
        {
            await FetchItemsAsync();

            App.TodoHub.Added += TodoHubOnAdded;
            App.TodoHub.Deleted += TodoHubOnDeleted;
            App.TodoHub.ToggledDone += TodoHubOnToggledDone;
            App.TodoHub.Edited += TodoHubOnEdited;
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
                OnPropertyChanged(nameof(IsNotAllItemsLoaded));
            }
        }

        public bool IsNotAllItemsLoaded => !IsAllItemsLoaded;

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

                var todoItems = await App.Todo.GetAfter(_lastItemId);

                if (!todoItems.Any())
                {
                    IsAllItemsLoaded = true;
                    IsItemsLoading = false;
                    return;
                }

                var syncContext = SynchronizationContext.Current;

                await Task.Run(() =>
                {
                    syncContext.Send(_ =>
                    {
                        foreach (var todoItem in todoItems)
                        {

                            TodoItems.Add(new Checkable<TodoItem>(todoItem, onChecked: OnTodoItemChecked));
                        }
                    }, null);
                });

                _lastItemId = todoItems.Last().Id;
            }
            finally
            {
                _ = Task.Delay(100).ContinueWith(_ =>
                {
                    IsItemsLoading = false;
                });
            }
        }

        private void OnTodoItemChecked(object sender, CheckedEventArgs e)
        {
            OnPropertyChanged(nameof(HasSelectedItems));
            OnPropertyChanged(nameof(SelectedItemsCount));
            OnPropertyChanged(nameof(HasSingleSelectedItem));
            OnPropertyChanged(nameof(SelectedItemsCountString));
            OnPropertyChanged(nameof(IsActionButtonsShown));

            ToolbarItems.Clear();
            if (SelectedItemsCount > 0)
            {
                foreach (var toolbarItem in AllToolbarItems.Skip(SelectedItemsCount == 1 ? 0 : 1))
                {
                    ToolbarItems.Add(toolbarItem);
                }
            }
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
                    var editedTodo = await App.Todo.Edit(EditingTodoItem.Item.Id, new TodoPutModel(NewTodoItemText, null));
                    await App.TodoHub.Edit(editedTodo);
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
                var addedTodo = await App.Todo.Add(new TodoPostModel(NewTodoItemText));
                await App.TodoHub.Add(addedTodo);
                NewTodoItemText = "";
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        public ICommand ToggleCheckedCommand => new Command<Checkable<TodoItem>>(todoItem =>
        {
            todoItem.Check();
        });

        public ICommand ToggleDoneCommand => new Command(async () =>
        {
            if (!HasSelectedItems) return;

            IsTogglingDone = true;
            var body = new TodosPutModel(SelectedTodoItems.Select(x => x.Item.Id));
            var editedTodoItems = await App.Todo.ToggleDone(body); //TODO add loading
            await App.TodoHub.ToggleDone(editedTodoItems);
            IsTogglingDone = false;
        });

        public ICommand DeleteCommand => new Command(async () =>
        {
            if (!HasSelectedItems) return;

            var alertResult = await _homePage.DisplayAlert(
                "Delete", 
                $"Are you sure you want to delete {SelectedItemsCountString}?", 
                "OK", "Cancel");
            if (!alertResult) return;

            IsDeleting = true;
            var body = new TodosDeleteModel(SelectedTodoItems.Select(x => x.Item.Id), false);
            var deletedTodoItems = await App.Todo.DeleteMany(body); //TODO add loading
            await App.TodoHub.Delete(deletedTodoItems.Select(x => x.Id).ToArray());
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
