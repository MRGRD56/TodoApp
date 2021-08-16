using Microsoft.EntityFrameworkCore.Design;

namespace TodoApp.ServerInterop.Data
{
    public interface ILocalDbContextFactory
    {
        public LocalDbContext Create();
    }
}
