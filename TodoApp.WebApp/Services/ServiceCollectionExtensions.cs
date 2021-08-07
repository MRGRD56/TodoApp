using Microsoft.Extensions.DependencyInjection;
using TodoApp.WebApp.Services.Maintenance;
using TodoApp.WebApp.Services.Repositories;

namespace TodoApp.WebApp.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTodoItemsRepository(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<TodoItemsRepository>();
        
        public static IServiceCollection AddUsersRepository(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<UsersRepository>();

        public static IServiceCollection AddDatabaseMigrator(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<DatabaseMigrator>();
    }
}