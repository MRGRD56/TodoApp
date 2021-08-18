using System;
using TodoApp.MobileClient.Models;
using TodoApp.ServerInterop;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TodoApp.MobileClient
{
    public partial class App : Application
    {
        internal static MainPage GetMainPage() => (MainPage)Current.MainPage;

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

            MainPage = new NavigationPage(new MainPage());
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
