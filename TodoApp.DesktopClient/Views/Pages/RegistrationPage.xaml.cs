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
    /// Interaction logic for RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        private RegistrationPageViewModel ViewModel => (RegistrationPageViewModel)DataContext;

        public RegistrationPage()
        {
            InitializeComponent();
            LoginTextBox.Focus();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.Password = ((PasswordBox)sender).SecurePassword;
        }

        private void OnPasswordRepeatChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.PasswordRepeat = ((PasswordBox)sender).SecurePassword;
        }
    }
}
