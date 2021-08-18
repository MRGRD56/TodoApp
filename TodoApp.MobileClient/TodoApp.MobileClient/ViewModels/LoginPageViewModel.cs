using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using TodoApp.Infrastructure.Models.RequestModels.Auth;
using TodoApp.MobileClient.Extensions;
using TodoApp.MobileClient.Views;
using Xamarin.Forms;

namespace TodoApp.MobileClient.ViewModels
{
    public class LoginPageViewModel : ViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand => new Command(async () =>
        {
            var login = Login?.Trim();
            var password = Password?.Trim();

            if (string.IsNullOrWhiteSpace(login)
                || string.IsNullOrWhiteSpace(password))
            {
                await App.GetMainPage().DisplayAlert("Sign in", "Provide your credentials", "OK");
                return;
            }

            var loginResponse = await App.Auth.LoginAsync(new LoginModel(login, password));
            await App.GetMainPage().DisplayAlert("Logged In", loginResponse.AccessToken, "OK");
            await App.GetMainPage().Navigation.PushNewAsync<HomePage>();
        });
    }
}
