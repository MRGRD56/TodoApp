using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.DesktopClient.Data;
using TodoApp.DesktopClient.Services;
using TodoApp.DesktopClient.ViewModels.WindowsViewModels;
using TodoApp.DesktopClient.Views.Windows;

namespace TodoApp.DesktopClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            using var localDbContext = new LocalDbContext();
            localDbContext.Database.EnsureCreated();
        }
    }
}