using System;
using System.Threading.Tasks;
using TodoApp.DesktopClient.Extensions;
using TodoApp.Infrastructure.Models;
using TodoApp.Infrastructure.Models.RequestModels.Todo;

namespace TodoApp.DesktopClient.Services.ServerInterop
{
    public static class Todo
    {
        [Obsolete("Use GetAfter instead")]
        public static async Task<TodoItem[]> Get(int page)
        {
            return await Auth.ApiHttpClient.GetAsync<TodoItem[]>(
                $"{ApiSettings.BaseUrl}api/todo?page={page}");
        }

        public static async Task<TodoItem[]> GetAfter(int afterId)
        {
            return await Auth.ApiHttpClient.GetAsync<TodoItem[]>(
                $"{ApiSettings.BaseUrl}api/todo/get_after?afterId={afterId}");
        }

        public static async Task<TodoItem> Add(TodoPostModel body)
        {
            return await Auth.ApiHttpClient.PostAsync<TodoItem>(
                $"{ApiSettings.BaseUrl}api/todo", body);
        }

        public static async Task<TodoItem> Edit(int todoItemId, TodoPutModel body)
        {
            return await Auth.ApiHttpClient.PutAsync<TodoItem>(
                $"{ApiSettings.BaseUrl}api/todo/{todoItemId}", body);
        }

        public static async Task<TodoItem[]> ToggleDone(TodosPutModel body)
        {
            return await Auth.ApiHttpClient.PutAsync<TodoItem[]>(
                $"{ApiSettings.BaseUrl}api/todo/toggle_done", body);
        }

        public static async Task<TodoItem> Delete(int todoItemId, bool restore = false)
        {
            return await Auth.ApiHttpClient.DeleteAsync<TodoItem>(
                $"{ApiSettings.BaseUrl}api/todo/{todoItemId}?restore={restore}");
        }

        public static async Task<TodoItem[]> DeleteMany(TodosDeleteModel body)
        {
            return await Auth.ApiHttpClient.DeleteAsync<TodoItem[]>(
                $"{ApiSettings.BaseUrl}api/todo", body);
        }
    }
}
