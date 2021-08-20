using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TodoApp.MobileClient.Extensions;
using TodoApp.MobileClient.Views;
using TodoApp.ServerInterop.Models;
using TodoApp.ServerInterop.Models.Events;
using Xamarin.Forms;

namespace TodoApp.MobileClient.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
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

        public ICommand PushModalCommand => new Command<Type>(async type => 
        {
            await App.GetMainPage().Navigation.PushNewModalAsync(type);
        });

        public ICommand LogoutCommand => new Command(async () =>
        {
            await App.Auth.LogoutAsync();
            await App.GetMainPage().Navigation.PopAllModalsAsync();
            await App.GetMainPage().Navigation.PushNewModalAsync<LoginPage>();
            await App.GetMainPage().Navigation.PushNewAsync<EmptyPage>();
            await App.GetMainPage().Navigation.PopAllAsync();
        });

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
