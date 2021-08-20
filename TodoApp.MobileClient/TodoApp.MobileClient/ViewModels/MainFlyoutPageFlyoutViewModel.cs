using System;
using System.Collections.Generic;
using System.Text;
using TodoApp.MobileClient.Models;

namespace TodoApp.MobileClient.ViewModels
{
    public class MainFlyoutPageFlyoutViewModel : ViewModel
    {
        private ListItem _selectedItem;

        public ListItem[] MenuItems => new[]
        {
            new ListItem("Log out", LogoutCommand)
        };

        public ListItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnSelectedItemChanged();
            }
        }

        private void OnSelectedItemChanged()
        {
            if (SelectedItem == null) return;

            if (SelectedItem.Command.CanExecute(SelectedItem.CommandParameter))
            {
                SelectedItem.Command.Execute(SelectedItem.CommandParameter);
            }
            SelectedItem = null;
        }
    }
}
