using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TodoApp.MobileClient.Extensions;
using TodoApp.MobileClient.Models;
using TodoApp.MobileClient.Views;
using TodoApp.ServerInterop;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TodoApp.MobileClient
{
    public partial class App : Application
    {
        internal static NavigationPage GetMainPage() => (NavigationPage)Current.MainPage;

        private static readonly XamarinLocalDbContextFactory LocalDbContextFactory;
        internal static Auth Auth { get; }
        internal static Todo Todo { get; }
        internal static TodoHub TodoHub { get; }

        static App()
        {
            LocalDbContextFactory = new XamarinLocalDbContextFactory();
            Auth = new Auth(LocalDbContextFactory);
            Todo = new Todo(Auth);
            TodoHub = new TodoHub(Auth);
        }

        public App()
        {
            using var localDbContext = LocalDbContextFactory.Create();
            localDbContext.Database.EnsureCreated();

            InitializeComponent();

            MainPage = new NavigationPage();
            Initialize();
        }

        private async void Initialize()
        {
            if (await Auth.TryLoginAsync())
            {
                await GetMainPage().Navigation.PushNewAsync<MainFlyoutPage>();
            }
            else
            {
                await GetMainPage().Navigation.PushNewModalAsync<LoginPage>();
            }
        }
    }
}
