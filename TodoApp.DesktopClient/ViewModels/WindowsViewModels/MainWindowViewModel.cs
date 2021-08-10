using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MgMvvmTools;
using TodoApp.DesktopClient.Models;
using TodoApp.DesktopClient.Services;
using TodoApp.DesktopClient.Services.ServerInterop;
using TodoApp.DesktopClient.Views.Pages;

namespace TodoApp.DesktopClient.ViewModels.WindowsViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel()
        {
            AppState.IsLoadingChanged += AppStateOnIsLoadingChanged;
        }

        public bool IsLoading => AppState.IsLoading;

        private void AppStateOnIsLoadingChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsLoading));
        }

        public ICommand LogoutCommand => new Command(async () =>
        {
            await Auth.LogoutAsync();
            MainWindowNavigation.NavigateNew<LoginPage>();
        });
    }
}
