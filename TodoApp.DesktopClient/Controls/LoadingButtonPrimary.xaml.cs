using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using JetBrains.Annotations;

namespace TodoApp.DesktopClient.Controls
{
    /// <summary>
    /// Interaction logic for LoadingButtonPrimary.xaml
    /// </summary>
    public partial class LoadingButtonPrimary : UserControl, INotifyPropertyChanged
    {
        public LoadingButtonPrimary()
        {
            InitializeComponent();
            RootElement.DataContext = this;
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
            "IsLoading", 
            typeof(bool), 
            typeof(LoadingButtonPrimary),
            new UIPropertyMetadata(false));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(LoadingButtonPrimary),
            new UIPropertyMetadata(""));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(LoadingButtonPrimary),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter",
            typeof(object),
            typeof(LoadingButtonPrimary),
            new UIPropertyMetadata(null));

        public bool IsLoading
        {
            get => (bool) GetValue(IsLoadingProperty);
            set
            {
                SetValue(IsLoadingProperty, value);
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        public bool IsNotLoading => !IsLoading;

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
