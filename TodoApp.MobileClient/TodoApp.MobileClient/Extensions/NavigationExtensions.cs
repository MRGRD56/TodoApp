using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Xamarin.Forms;

namespace TodoApp.MobileClient.Extensions
{
    public static class NavigationExtensions
    {
        private static Page CreatePage(Type pageType)
        {
            var instance = Activator.CreateInstance(pageType);
            if (!(instance is Page page))
            {
                throw new TypeAccessException($"A page created from the pageType is not a Page, its type is {instance.GetType().Name}");
            }
            return page;
        }

        public static async Task PushNewAsync(this INavigation navigation, Type pageType)
        {
            await navigation.PushAsync(CreatePage(pageType));
        }

        public static async Task PushNewModalAsync(this INavigation navigation, Type pageType)
        {
            await navigation.PushModalAsync(CreatePage(pageType));
        }

        public static async Task PushNewAsync<TPage>(this INavigation navigation) where TPage : Page, new()
        {
            await navigation.PushAsync(new TPage());
        }

        public static async Task PushNewModalAsync<TPage>(this INavigation navigation) where TPage : Page, new()
        {
            await navigation.PushModalAsync(new TPage());
        }
    }
}
