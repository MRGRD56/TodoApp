using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TodoApp.MobileClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TodoApp.MobileClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainFlyoutPageFlyout : ContentPage
    {
        public MainFlyoutPageFlyout()
        {
            InitializeComponent();
        }

        //private void ListViewOnItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    var listView = (ListView)sender;
        //    var item = (ListItem)e.SelectedItem;
        //    if (item == null) return;

        //    if (item.Command.CanExecute(item.CommandParameter))
        //    {
        //        item.Command.Execute(item.CommandParameter);
        //    }
        //    listView.SelectedItem = null;
        //}
    }
}