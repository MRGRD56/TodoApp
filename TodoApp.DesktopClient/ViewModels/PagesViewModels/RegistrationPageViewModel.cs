using MgMvvmTools;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TodoApp.DesktopClient.Extensions;
using TodoApp.DesktopClient.Models.Exceptions;
using TodoApp.DesktopClient.Services;
using TodoApp.DesktopClient.Services.ServerInterop;
using TodoApp.DesktopClient.Views.Pages;
using TodoApp.Infrastructure.Models.RequestModels.Auth;

namespace TodoApp.DesktopClient.ViewModels.PagesViewModels
{
    public class RegistrationPageViewModel : ViewModel
    {
        private string _login = "";
        private string _error;
        private SecureString _password;
        private SecureString _passwordRepeat;
        private bool _isRegistering;

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

        public SecureString PasswordRepeat
        {
            get => _passwordRepeat;
            set
            {
                _passwordRepeat = value;
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

        public bool IsRegistering
        {
            get => _isRegistering;
            set
            {
                _isRegistering = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand => new Command(async () =>
        {
            Error = null;
            var login = Login.Trim() ?? "";
            var password = Password.GetString() ?? "";
            var passwordRepeat = PasswordRepeat.GetString() ?? "";

            var isLoginNull = string.IsNullOrWhiteSpace(login);
            var isPasswordNull = string.IsNullOrWhiteSpace(password);
            var isPasswordRepeatNull = string.IsNullOrWhiteSpace(passwordRepeat);
            if (isLoginNull || isPasswordNull)
            {
                Error = "Specify the " +
                        (isLoginNull && isPasswordNull
                            ? "login and password"
                            : isLoginNull
                                ? "login"
                                : "password");
                return;
            }
            if (isPasswordRepeatNull)
            {
                Error = "Repeat the password";
                return;
            }
            if (password != passwordRepeat)
            {
                Error = "The passwords must match";
                return;
            }

            //FIXME: Not working - isValid is always true
            //var registrationModel = new RegistrationModel(login, password);
            //
            //var validationContext = new ValidationContext(registrationModel, null, null);
            //var validationResults = new List<ValidationResult>();
            //var isValid = Validator.TryValidateObject(registrationModel, validationContext, validationResults, true);
            //if (!isValid)
            //{
            //    foreach (var validationResult in validationResults)
            //    {
            //        Error += validationResult.ErrorMessage;
            //    }
            //    return;
            //}

            if (!Regex.IsMatch(login, @"^(?=.*[A-Za-z0-9]$)[A-Za-z][A-Za-z\d.-_]{0,19}$")
                || login.Length < 3)
            {
                Error = "Invalid login. The login must be at least 3 characters long.";
                return;
            }
            if (!Regex.IsMatch(password, @"^(?=.*[A-Za-zА-Яа-яЁё])(?=.*\d).{6,}$"))
            {
                Error = "The password must be at least 6 characters long and contain at least one number and one letter.";
                return;
            }

            try
            {
                var registrationModel = new RegistrationModel(login, password);
                IsRegistering = true;
                var response = await Auth.RegisterAsync(registrationModel);
                MainWindowNavigation.NavigateNew<HomePage>(clearHistory: true);
            }
            catch (HttpException exception)
            {
                var error = await Validation.ParseServerErrorsStringAsync(exception.Response.Content);
                Error = error;
            }
            finally
            {
                IsRegistering = false;
            }
        });
    }
}
