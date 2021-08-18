using TodoApp.ClientLocalDb;
using Xamarin.Forms;

namespace TodoApp.MobileClient.Models
{
    public class XamarinLocalDbContext : LocalDbContext
    {
        private static string FileDirectory => 
            DependencyService.Get<ILocalDbFileDirectoryProvider>().GetLocalDbFileDirectory();

        public XamarinLocalDbContext() : base(FileDirectory)
        {

        }
    }
}
