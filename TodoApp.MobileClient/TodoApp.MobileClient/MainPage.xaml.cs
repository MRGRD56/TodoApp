using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.MobileClient.Extensions;
using TodoApp.MobileClient.Views;
using Xamarin.Forms;

namespace TodoApp.MobileClient
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            Initialize();
        }

        private async void Initialize()
        {
            if (await App.Auth.TryLoginAsync())
            {
                await Navigation.PushNewAsync<HomePage>();
            }
            else
            {
                await Navigation.PushNewModalAsync<LoginPage>();
            }
        }
    }
}
