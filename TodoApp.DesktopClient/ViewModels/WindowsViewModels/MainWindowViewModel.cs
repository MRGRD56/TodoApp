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

namespace TodoApp.DesktopClient.ViewModels.WindowsViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel()
        {
            
        }

        public ICommand LogoutCommand => new Command(async () =>
        {
            await Auth.LogoutAsync();
        });
    }
}
