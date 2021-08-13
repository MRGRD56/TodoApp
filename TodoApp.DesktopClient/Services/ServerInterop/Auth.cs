using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TodoApp.DesktopClient.Data;
using TodoApp.DesktopClient.Extensions;
using TodoApp.DesktopClient.Models;
using TodoApp.DesktopClient.Models.Events;
using TodoApp.Infrastructure.Models.Abstractions;
using TodoApp.Infrastructure.Models.RequestModels.Auth;

namespace TodoApp.DesktopClient.Services.ServerInterop
{
    public static class Auth
    {
        private static AccountInfo _currentUser;
        private const string AccessTokenKey = "API_ACCESS_TOKEN";

        public static HttpClient ApiHttpClient
        {
            get
            {
                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(15);
                if (AccessToken != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                }

                return httpClient;
            }
        }

        public static AccountInfo CurrentUser
        {
            get => _currentUser;
            private set
            {
                _currentUser = value;
                if (_currentUser != null)
                {
                    LoggedIn?.Invoke(null, new LoginEventArgs(_currentUser));
                }
                else
                {
                    LoggedOut?.Invoke(null, new LoginEventArgs(_currentUser));
                }
            }
        }

        public static string AccessToken { get; private set; }

        public static bool IsAuthenticated => AccessToken != null;

        private static async Task<string> GetAccessTokenAsync()
        {
            await using var db = new LocalDbContext();
            var entry = await db.LocalStorage.FindAsync(AccessTokenKey);
            return entry?.Value;
        }

        private static async Task SetAccessTokenAsync(string value)
        {
            AccessToken = value;
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

        private static async Task<LoginResponse> FetchLoginResponseAsync(ILoginModel loginModel, string url)
        {
            var response = await ApiHttpClient.PostAsync<LoginResponse>(url, loginModel);
            await SetAccessTokenAsync(response.AccessToken);
            CurrentUser = await Profile.GetAsync();
            await TodoHub.StartNewConnectionAsync();
            return response;
        }

        public static async Task<LoginResponse> LoginAsync(LoginModel loginModel) =>
            await FetchLoginResponseAsync(loginModel, $"{ApiSettings.BaseUrl}api/auth/login");

        public static async Task<LoginResponse> RegisterAsync(RegistrationModel registrationModel) =>
            await FetchLoginResponseAsync(registrationModel, $"{ApiSettings.BaseUrl}api/auth/register");

        public static async Task LogoutAsync()
        {
            CurrentUser = null;
            await SetAccessTokenAsync(null);
            await TodoHub.RemoveConnectionAsync();
        }

        public static event AuthEventHandler LoggedIn;
        public static event AuthEventHandler LoggedOut;

        public static async Task<bool> TryLoginAsync()
        {
            AccessToken = await GetAccessTokenAsync();
            if (AccessToken != null)
            {
                CurrentUser = await Profile.GetAsync();
                await TodoHub.StartNewConnectionAsync();
                return true;
            }

            return false;
        }
    }

    public delegate void AuthEventHandler(object sender, LoginEventArgs e);
}
