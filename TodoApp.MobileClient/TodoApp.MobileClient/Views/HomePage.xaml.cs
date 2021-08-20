using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CheckLib;
using TodoApp.Infrastructure.Models;
using TodoApp.MobileClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TodoApp.MobileClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        private HomePageViewModel ViewModel => (HomePageViewModel)BindingContext;

        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomePageViewModel(this);
        }

        private void TodoItemsOnScrolled(object sender, ScrolledEventArgs e)
        {
            var scrollView = (ScrollView)sender;
            var offset = e.ScrollY;
            var height = scrollView.ContentSize.Height - scrollView.Height;
            ViewModel.OnTodoItemsScroll(offset, height);
        }
    }
}