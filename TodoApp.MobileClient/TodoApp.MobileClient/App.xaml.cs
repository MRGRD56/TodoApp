using System;
using TodoApp.MobileClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TodoApp.MobileClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var dbFilePath = DependencyService.Get<ILocalDbFileDirectoryProvider>().GetLocalDbFileDirectory();


            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
