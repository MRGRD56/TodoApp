using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TodoApp.DesktopClient.Data;
using TodoApp.DesktopClient.Extensions;
using TodoApp.DesktopClient.Models;
using TodoApp.Infrastructure.Models.RequestModels.Auth;

namespace TodoApp.DesktopClient.Services.ServerInterop
{
    public static class Auth
    {
        private const string AccessTokenKey = "API_ACCESS_TOKEN";

        public static HttpClient ApiHttpClient
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

        public static AccountInfo CurrentUser { get; private set; } = null;
        public static string AccessToken { get; private set; }

        public static bool IsAuthenticated => AccessToken != null;

        private static async Task<string> LoadAccessTokenAsync()
        {
            await using var db = new LocalDbContext();
            var entry = await db.LocalStorage.FindAsync(AccessTokenKey);
            return entry?.Value;
        }

        private static async Task SaveAccessTokenAsync(string value)
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

        private static async Task<LoginResponse> FetchLoginResponseAsync(LoginModel loginModel, string url)
        {
            var response = await ApiHttpClient.PostAsync<LoginResponse>(url, loginModel);
            await SaveAccessTokenAsync(response.AccessToken);
            return response;
        }

        public static async Task<LoginResponse> LoginAsync(LoginModel loginModel) =>
            await FetchLoginResponseAsync(loginModel, $"{ApiSettings.BaseUrl}api/auth/login");

        public static async Task<LoginResponse> RegisterAsync(LoginModel registrationModel) =>
            await FetchLoginResponseAsync(registrationModel, $"{ApiSettings.BaseUrl}api/auth/register");

        public static async Task LogoutAsync()
    }
}
