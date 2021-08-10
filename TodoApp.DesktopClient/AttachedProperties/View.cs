using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TodoApp.DesktopClient.AttachedProperties
{
    public static class View
    {
        #region IsVisible

        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
            "IsVisible",
            typeof(bool),
            typeof(View),
            new FrameworkPropertyMetadata(null));

        public static bool GetIsVisible(UIElement element)
        {
            CheckIfElementNull(element);

            return (bool) element.GetValue(IsVisibleProperty);
        }

        public static void SetIsVisible(UIElement element, bool value)
        {
            CheckIfElementNull(element);

            element.SetValue(IsVisibleProperty, value);

            element.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region IsVisibleRe

        public static readonly DependencyProperty IsVisibleReProperty = DependencyProperty.RegisterAttached(
            "IsVisibleRe",
            typeof(bool),
            typeof(View),
            new FrameworkPropertyMetadata(null));

        public static bool GetIsVisibleRe(UIElement element)
        {
            CheckIfElementNull(element);

            return (bool) element.GetValue(IsVisibleReProperty);
        }

        public static void SetIsVisibleRe(UIElement element, bool value)
        {
            CheckIfElementNull(element);

            element.SetValue(IsVisibleReProperty, value);

            element.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #region IsHidden

        public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.RegisterAttached(
            "IsHidden",
            typeof(bool),
            typeof(View),
            new FrameworkPropertyMetadata(null));

        public static bool GetIsHidden(UIElement element)
        {
            CheckIfElementNull(element);

            return (bool) element.GetValue(IsHiddenProperty);
        }

        public static void SetIsHidden(UIElement element, bool value)
        {
            CheckIfElementNull(element);

            element.SetValue(IsHiddenProperty, value);

            element.Visibility = value ? Visibility.Hidden : Visibility.Visible;
        }

        #endregion

        #region IsHiddenRe

        public static readonly DependencyProperty IsHiddenReProperty = DependencyProperty.RegisterAttached(
            "IsHiddenRe",
            typeof(bool),
            typeof(View),
            new FrameworkPropertyMetadata(null));

        public static bool GetIsHiddenRe(UIElement element)
        {
            CheckIfElementNull(element);

            return (bool) element.GetValue(IsHiddenReProperty);
        }

        public static void SetIsHiddenRe(UIElement element, bool value)
        {
            CheckIfElementNull(element);

            element.SetValue(IsHiddenReProperty, value);

            element.Visibility = value ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion


        private static void CheckIfElementNull(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
        }
    }
}
