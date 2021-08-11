using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TodoApp.DesktopClient.ViewModels.PagesViewModels;

namespace TodoApp.DesktopClient.Views.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private LoginPageViewModel ViewModel => (LoginPageViewModel) DataContext;

        public LoginPage()
        {
            InitializeComponent();
            LoginTextBox.Focus();
        }

        private void PasswordBoxOnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.Password = ((PasswordBox)sender).SecurePassword;
        }
    }
}
