using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TodoApp.DesktopClient.AttachedProperties
{
    [Obsolete("Not working", true)]
    public static class View
    {
        #region IsVisible

        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
            "IsVisible",
            typeof(bool),
            typeof(View),
            new FrameworkPropertyMetadata(false, CreatePropertyChangedCallback(Visibility.Visible, Visibility.Collapsed)));

        public static bool GetIsVisible(UIElement element)
        {
            return (bool) element.GetValue(IsVisibleProperty);
        }

        public static void SetIsVisible(UIElement element, bool value)
        {
            element.SetValue(IsVisibleProperty, value);
        }

        #endregion

        #region IsVisibleRe

        public static readonly DependencyProperty IsVisibleReProperty = DependencyProperty.RegisterAttached(
            "IsVisibleRe",
            typeof(bool),
            typeof(View),
            new FrameworkPropertyMetadata(false, CreatePropertyChangedCallback(Visibility.Collapsed, Visibility.Visible)));

        public static bool GetIsVisibleRe(UIElement element)
        {
            return (bool) element.GetValue(IsVisibleReProperty);
        }

        public static void SetIsVisibleRe(UIElement element, bool value)
        {
            element.SetValue(IsVisibleReProperty, value);
        }

        #endregion

        #region IsHidden

        public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.RegisterAttached(
            "IsHidden",
            typeof(bool),
            typeof(View),
            new FrameworkPropertyMetadata(false, CreatePropertyChangedCallback(Visibility.Hidden, Visibility.Visible)));

        public static bool GetIsHidden(UIElement element)
        {
            return (bool) element.GetValue(IsHiddenProperty);
        }

        public static void SetIsHidden(UIElement element, bool value)
        {
            element.SetValue(IsHiddenProperty, value);
        }

        #endregion

        #region IsHiddenRe

        public static readonly DependencyProperty IsHiddenReProperty = DependencyProperty.RegisterAttached(
            "IsHiddenRe",
            typeof(bool),
            typeof(View),
            new FrameworkPropertyMetadata(false, CreatePropertyChangedCallback(Visibility.Visible, Visibility.Hidden)));

        public static bool GetIsHiddenRe(UIElement element)
        {
            return (bool) element.GetValue(IsHiddenReProperty);
        }

        public static void SetIsHiddenRe(UIElement element, bool value)
        {
            element.SetValue(IsHiddenReProperty, value);
        }

        #endregion

        private static PropertyChangedCallback CreatePropertyChangedCallback(
            Visibility trueValue, Visibility falseValue)
        {
            return (d, e) =>
            {
                var value = (bool) e.NewValue;
                if (d is UIElement element)
                {
                    element.Visibility = value ? trueValue : falseValue;
                }
            };
        }
    }
}
