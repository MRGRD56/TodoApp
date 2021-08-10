using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MgMvvmTools;
using TodoApp.DesktopClient.Services;
using TodoApp.DesktopClient.Services.ServerInterop;
using TodoApp.DesktopClient.Views.Pages;
using TodoApp.Infrastructure.Models.RequestModels.Auth;

namespace TodoApp.DesktopClient.ViewModels.PagesViewModels
{
    public class LoginPageViewModel : ViewModel
    {
        private string _login;

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand => new Command<PasswordBox>(async passwordBox =>
        {
            var login = Login.Trim();
            var password = passwordBox.Password;

            var response = await Auth.LoginAsync(new LoginModel(login, password));
            if (response != null)
            {
                MainWindowNavigation.NavigateNew<HomePage>();
            }
        });
    }
}
