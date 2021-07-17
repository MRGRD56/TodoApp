using Microsoft.Extensions.DependencyInjection;
using TodoApp.WebApp.Services.Repositories;

namespace TodoApp.WebApp.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTodoItemsRepository(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<TodoItemsRepository>();
    }
}