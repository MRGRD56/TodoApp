using System;
using System.Windows.Input;
using MgMvvmTools;
using TodoApp.DesktopClient.Services;
using TodoApp.ServerInterop.Models;
using TodoApp.ServerInterop.Models.Events;

namespace TodoApp.DesktopClient.ViewModels
{
    public abstract class ViewModel : NotifyPropertyChanged
    {
        public AccountInfo CurrentUser => App.Auth.CurrentUser;
        public bool IsAuthenticated => CurrentUser != null;

        public ViewModel()
        {
            App.Auth.LoggedIn += AuthOnUserChanged;
            App.Auth.LoggedOut += AuthOnUserChanged;
        }

        private void AuthOnUserChanged(object sender, LoginEventArgs e)
        {
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(IsAuthenticated));
        }

        public ICommand NavigateCommand => new Command<Type>(type =>
        {
            MainWindowNavigation.NavigateNew(type);
        });
    }
}
