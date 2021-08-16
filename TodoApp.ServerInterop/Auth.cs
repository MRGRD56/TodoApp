using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TodoApp.Infrastructure.Models.Abstractions;
using TodoApp.Infrastructure.Models.RequestModels.Auth;
using TodoApp.ServerInterop.Models;
using TodoApp.ServerInterop.Models.Events;
using TodoApp.ServerInterop.Extensions;
using TodoApp.ClientLocalDb;
using TodoApp.ClientLocalDb.Models;

namespace TodoApp.ServerInterop
{
    public class Auth
    {
        private readonly ILocalDbContextFactory _dbFactory;

        public Auth(ILocalDbContextFactory localDbContextFactory)
        {
            _dbFactory = localDbContextFactory;
        }

        private AccountInfo _currentUser;
        private const string AccessTokenKey = "API_ACCESS_TOKEN";

        public HttpClient ApiHttpClient
        {
            get
            {
                var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(15)
                };
                if (AccessToken != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                }

                return httpClient;
            }
        }

        public AccountInfo CurrentUser
        {
            get => _currentUser;
            private set
            {
                _currentUser = value;
                if (_currentUser != null)
                {
                    LoggedIn?.Invoke(this, new LoginEventArgs(_currentUser));
                }
                else
                {
                    LoggedOut?.Invoke(this, new LoginEventArgs(_currentUser));
                }
            }
        }

        public string AccessToken { get; private set; }

        public bool IsAuthenticated => AccessToken != null;

        private async Task<string> GetAccessTokenAsync()
        {
            await using var db = _dbFactory.Create();
            var entry = await db.LocalStorage.FindAsync(AccessTokenKey);
            return entry?.Value;
        }

        private async Task SetAccessTokenAsync(string value)
        {
            AccessToken = value;
            await using var db = _dbFactory.Create();
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

        private async Task<LoginResponse> FetchLoginResponseAsync(ILoginModel loginModel, string url)
        {
            var response = await ApiHttpClient.PostAsync<LoginResponse>(url, loginModel);
            await SetAccessTokenAsync(response.AccessToken);
            CurrentUser = await GetProfileAsync();
            return response;
        }

        public async Task<LoginResponse> LoginAsync(LoginModel loginModel) =>
            await FetchLoginResponseAsync(loginModel, $"{ApiSettings.BaseUrl}api/auth/login");

        public async Task<LoginResponse> RegisterAsync(RegistrationModel registrationModel) =>
            await FetchLoginResponseAsync(registrationModel, $"{ApiSettings.BaseUrl}api/auth/register");

        public async Task LogoutAsync()
        {
            CurrentUser = null;
            await SetAccessTokenAsync(null);
        }

        public event AuthEventHandler LoggedIn;
        public event AuthEventHandler LoggedOut;

        public async Task<bool> TryLoginAsync()
        {
            AccessToken = await GetAccessTokenAsync();
            if (AccessToken != null)
            {
                CurrentUser = await GetProfileAsync();
                return true;
            }

            return false;
        }

        private async Task<AccountInfo> GetProfileAsync()
        {
            if (!IsAuthenticated)
            {
                throw new Exception("You are not logged in!");
            }

            return await ApiHttpClient.GetAsync<AccountInfo>($"{ApiSettings.BaseUrl}api/profile");
        }
    }

    public delegate void AuthEventHandler(object sender, LoginEventArgs e);
}
