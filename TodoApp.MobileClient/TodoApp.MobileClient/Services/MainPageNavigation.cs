using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TodoApp.MobileClient.Services
{
    public static class MainPageNavigation
    {
        public static INavigation Navigation => Application.Current.MainPage.Navigation;

        private static Page CreatePage(Type pageType)
        {
            var instance = Activator.CreateInstance(pageType);
            if (!(instance is Page page))
            {
                throw new TypeAccessException($"A page created from the pageType is not a Page, its type is {instance.GetType().Name}");
            }
            return page;
        }

        public static async Task PushNewAsync(Type pageType)
        {
            await Navigation.PushAsync(CreatePage(pageType));
        }

        public static async Task PushNewModalAsync(Type pageType)
        {
            await Navigation.PushModalAsync(CreatePage(pageType));
        }

        public static async Task PushNewAsync<TPage>() where TPage : Page, new()
        {
            await Navigation.PushAsync(new TPage());
        }

        public static async Task PushNewModalAsync<TPage>() where TPage : Page, new()
        {
            await Navigation.PushModalAsync(new TPage());
        }
    }
}
