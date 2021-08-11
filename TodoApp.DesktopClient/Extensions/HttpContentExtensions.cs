using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TodoApp.DesktopClient.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ParseJsonAsync<T>(this HttpContent httpContent)
        {
            var stringContent = await httpContent.ReadAsStringAsync();
            return stringContent.ParseJson<T>();
        }

        public static async Task<JObject> ParseJsonAsync(this HttpContent httpContent)
        {
            return await httpContent.ParseJsonAsync<JObject>();
        }
    }
}
