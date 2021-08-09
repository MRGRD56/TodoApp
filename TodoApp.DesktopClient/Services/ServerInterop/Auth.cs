using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TodoApp.DesktopClient.Data;
using TodoApp.DesktopClient.Models;

namespace TodoApp.DesktopClient.Services.ServerInterop
{
    public static class Auth
    {
        private const string AccessTokenKey = "API_ACCESS_TOKEN";

        private static HttpClient ApiHttpClient
        {
            get
            {
                var httpClient = new HttpClient();
                if (AccessToken != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                }

                return httpClient;
            }
        }

        public static AccountInfo CurrentUser = null;
        public static string AccessToken { get; private set; }

        private static async Task<string> GetAccessTokenAsync()
        {
            await using var db = new LocalDbContext();
            var entry = await db.LocalStorage.FindAsync(AccessTokenKey);
            return entry?.Value;
        }

        private static async Task SetAccessTokenAsync(string value)
        {
            await using var db = new LocalDbContext();
            var entry = await db.LocalStorage.FindAsync(AccessTokenKey);
            if (entry != null)
            {
                entry.Value = value;
            }
            else
            {
                entry = new KeyValue(AccessTokenKey, value);
                await db.LocalStorage.AddAsync(entry);
            }
            await db.SaveChangesAsync();
        }
    }
}
