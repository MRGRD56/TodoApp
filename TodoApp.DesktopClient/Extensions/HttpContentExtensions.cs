using System.Net.Http;
using System.Threading.Tasks;

namespace TodoApp.DesktopClient.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ParseJsonAsync<T>(this HttpContent httpContent)
        {
            var stringContent = await httpContent.ReadAsStringAsync();
            return stringContent.ParseJson<T>();
        }
    }
}
