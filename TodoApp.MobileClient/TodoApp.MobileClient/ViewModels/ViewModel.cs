using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TodoApp.MobileClient.Services;
using Xamarin.Forms;

namespace TodoApp.MobileClient.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public ICommand PushModalCommand => new Command<Type>(async type => 
        {
            await MainPageNavigation.PushNewModalAsync(type);
        });

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
