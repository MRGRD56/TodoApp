using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TodoApp.DesktopClient.Services;
using TodoApp.DesktopClient.Services.ServerInterop;
using TodoApp.DesktopClient.Views.Pages;

namespace TodoApp.DesktopClient.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //LoadingBrowser.NavigateToStream(EmbeddedResources.GetStream("TodoApp.DesktopClient.Assets.global-loading.html"));
            MainWindowNavigation.NavigationFrame.Navigated += NavigationFrameOnNavigated;

            Initialize();
        }

        private void NavigationFrameOnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page)
            {
                Title = page.Title;
            }
        }

        private async void Initialize()
        {
            await TodoHub.EnsureConnectedAsync();
            await TryLoginAsync();

            AppState.IsLoading = false;
        }

        private async Task TryLoginAsync()
        {
            var isAuthenticated = await Auth.TryLoginAsync();
            if (isAuthenticated)
            {
                MainWindowNavigation.NavigateNew<HomePage>();
            }
            else
            {
                MainWindowNavigation.NavigateNew<LoginPage>();
            }
        }
    }
}