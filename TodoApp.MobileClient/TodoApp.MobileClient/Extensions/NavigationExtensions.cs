using System;
using System.Collections.Generic;
using System.Linq;
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

        private static void ClearStack(this INavigation navigation, Func<INavigation, IReadOnlyList<Page>> stackGetter)
        {
            var stackList = stackGetter.Invoke(navigation).ToList();
            stackList.ForEach(navigation.RemovePage);
        }

        public static void ClearNavigationStack(this INavigation navigation)
        {
            navigation.NavigationStack
                .ToList()
                .ForEach(navigation.RemovePage);
        }

        public static async Task PopAllAsync(this INavigation navigation)
        {
            foreach (var _ in navigation.NavigationStack.ToList())
            {
                await navigation.PopAsync(false);
            }
        }

        public static async Task PopAllModalsAsync(this INavigation navigation)
        {
            foreach (var _ in navigation.ModalStack.ToList())
            {
                await navigation.PopModalAsync(false);
            }
        }
    }
}
