using TodoApp.ClientLocalDb;

namespace TodoApp.DesktopClient.Models
{
    public class WindowsLocalDbContextFactory : ILocalDbContextFactory
    {
        public LocalDbContext Create() => new WindowsLocalDbContext();
    }
}
