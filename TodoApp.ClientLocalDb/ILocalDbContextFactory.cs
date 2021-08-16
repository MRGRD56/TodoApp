namespace TodoApp.ClientLocalDb
{
    public interface ILocalDbContextFactory
    {
        public LocalDbContext Create();
    }
}
