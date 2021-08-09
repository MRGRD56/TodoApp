using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MgMvvmTools;
using TodoApp.DesktopClient.Models;
using TodoApp.DesktopClient.Models.Events;
using TodoApp.DesktopClient.Services.ServerInterop;

namespace TodoApp.DesktopClient.ViewModels
{
    public abstract class BaseViewModel : NotifyPropertyChanged
    {
        private AccountInfo _currentUser;

        public AccountInfo CurrentUser
        {
            get => _currentUser;
            private set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        static BaseViewModel()
        {
            Auth.LoggedIn += AuthOnUserChanged;
            Auth.LoggedOut += AuthOnUserChanged;
        }

        private static void AuthOnUserChanged(object sender, LoginEventArgs e)
        {
        }
    }
}
