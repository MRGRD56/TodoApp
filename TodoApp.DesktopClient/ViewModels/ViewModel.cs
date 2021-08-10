using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MgMvvmTools;
using TodoApp.DesktopClient.Models;
using TodoApp.DesktopClient.Models.Events;
using TodoApp.DesktopClient.Services;
using TodoApp.DesktopClient.Services.ServerInterop;

namespace TodoApp.DesktopClient.ViewModels
{
    public abstract class ViewModel : NotifyPropertyChanged
    {
        public AccountInfo CurrentUser => Auth.CurrentUser;
        public bool IsAuthenticated => CurrentUser != null;

        public ViewModel()
        {
            Auth.LoggedIn += AuthOnUserChanged;
            Auth.LoggedOut += AuthOnUserChanged;
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
