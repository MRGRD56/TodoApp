using System;
using System.Threading.Tasks;
using TodoApp.ServerInterop.Extensions;
using TodoApp.Infrastructure.Models;
using TodoApp.Infrastructure.Models.RequestModels.Todo;

namespace TodoApp.ServerInterop
{
    public class Todo
    {
        private readonly Auth _auth;

        public Todo(Auth auth)
        {
            _auth = auth;
        }

        [Obsolete("Use GetAfter instead")]
        public async Task<TodoItem[]> Get(int page)
        {
            return await _auth.ApiHttpClient.GetAsync<TodoItem[]>(
                $"{ApiSettings.BaseUrl}api/todo?page={page}");
        }

        public async Task<TodoItem[]> GetAfter(int afterId)
        {
            return await _auth.ApiHttpClient.GetAsync<TodoItem[]>(
                $"{ApiSettings.BaseUrl}api/todo/get_after?afterId={afterId}");
        }

        public async Task<TodoItem> Add(TodoPostModel body)
        {
            return await _auth.ApiHttpClient.PostAsync<TodoItem>(
                $"{ApiSettings.BaseUrl}api/todo", body);
        }

        public async Task<TodoItem> Edit(int todoItemId, TodoPutModel body)
        {
            return await _auth.ApiHttpClient.PutAsync<TodoItem>(
                $"{ApiSettings.BaseUrl}api/todo/{todoItemId}", body);
        }

        public async Task<TodoItem[]> ToggleDone(TodosPutModel body)
        {
            return await _auth.ApiHttpClient.PutAsync<TodoItem[]>(
                $"{ApiSettings.BaseUrl}api/todo/toggle_done", body);
        }

        public async Task<TodoItem> Delete(int todoItemId, bool restore = false)
        {
            return await _auth.ApiHttpClient.DeleteAsync<TodoItem>(
                $"{ApiSettings.BaseUrl}api/todo/{todoItemId}?restore={restore}");
        }

        public async Task<TodoItem[]> DeleteMany(TodosDeleteModel body)
        {
            return await _auth.ApiHttpClient.DeleteAsync<TodoItem[]>(
                $"{ApiSettings.BaseUrl}api/todo", body);
        }
    }
}
