using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;
using MgMvvmTools;
using Newtonsoft.Json.Linq;
using TodoApp.DesktopClient.Extensions;
using TodoApp.DesktopClient.Models.Exceptions;
using TodoApp.DesktopClient.Services;
using TodoApp.DesktopClient.Services.ServerInterop;
using TodoApp.DesktopClient.Views.Pages;
using TodoApp.Infrastructure.Models.RequestModels.Auth;

namespace TodoApp.DesktopClient.ViewModels.PagesViewModels
{
    public class LoginPageViewModel : ViewModel
    {
        private string _login = "";
        private string _error;
        private SecureString _password;
        private bool _isLoggingIn;

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
                Error = null;
            }
        }

        public SecureString Password
        {
            get => _password;
            set
            {
                _password = value;
                Error = null;
            }
        }

        public string Error
        {
            get => _error;
            set
            {
                if (_error == value) return;
                _error = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrWhiteSpace(Error);

        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set
            {
                _isLoggingIn = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand => new Command(async () =>
        {
            Error = null;
            var login = Login.Trim() ?? "";
            var password = Password.GetString() ?? "";

            var isLoginNull = string.IsNullOrWhiteSpace(login);
            var isPasswordNull = string.IsNullOrWhiteSpace(password);
            if (isLoginNull || isPasswordNull)
            {
                Error = "Provide your " +
                        (isLoginNull && isPasswordNull
                            ? "credentials"
                            : isLoginNull
                                ? "login"
                                : "password");
                return;
            }

            try
            {
                IsLoggingIn = true;
                var response = await Auth.LoginAsync(new LoginModel(login, password));
                MainWindowNavigation.NavigateNew<HomePage>();
            }
            catch (HttpException exception)
            {
                var error = await Validation.ParseServerErrorsStringAsync(exception.Response.Content);
                Error = error;
            }
            finally
            {
                IsLoggingIn = false;
            }
        });
    }
}
