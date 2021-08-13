using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;

namespace TodoApp.DesktopClient.Controls
{
    public class LoadingButtonBase : UserControl, INotifyPropertyChanged
    {
        public LoadingButtonBase()
        {
            IsEnabledChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(IsButtonEnabled));
            };
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
            "IsLoading",
            typeof(bool),
            typeof(LoadingButtonBase),
            new UIPropertyMetadata(false, (d, e) =>
            {
                var element = d as LoadingButtonBase;
                element.OnPropertyChanged(nameof(IsButtonEnabled));
            }));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(LoadingButtonBase),
            new UIPropertyMetadata(""));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(LoadingButtonBase),
            new UIPropertyMetadata(defaultValue: null));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter",
            typeof(object),
            typeof(LoadingButtonBase),
            new UIPropertyMetadata(defaultValue: null));

        public static readonly DependencyProperty ClickProperty = DependencyProperty.Register(
            "Click",
            typeof(RoutedEventHandler),
            typeof(LoadingButtonBase),
            new UIPropertyMetadata(defaultValue: null));

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set
            {
                SetValue(IsLoadingProperty, value);
                OnPropertyChanged(nameof(IsNotLoading));
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public bool IsNotLoading => !IsLoading;

        public string Text
        {
            get => (string)GetValue(TextProperty);
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

        public RoutedEventHandler Click
        {
            get => (RoutedEventHandler)GetValue(ClickProperty);
            set => SetValue(ClickProperty, value);
        }

        protected void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(sender, e);
        }

        public bool IsButtonEnabled
        {
            get
            {
                var isEnabled = (bool)GetValue(IsEnabledProperty);
                var isLoading = IsLoading;
                return !isLoading && isEnabled;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
