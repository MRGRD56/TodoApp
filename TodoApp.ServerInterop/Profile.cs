using System;
using System.Threading.Tasks;
using TodoApp.ServerInterop.Extensions;
using TodoApp.ServerInterop.Models;

namespace TodoApp.ServerInterop
{
    public class Profile
    {
        private readonly Auth _auth;

        public Profile(Auth auth)
        {
            _auth = auth;
        }

        public async Task<AccountInfo> GetAsync()
        {
            if (!_auth.IsAuthenticated)
            {
                throw new Exception("You are not logged in!");
            }

            return await _auth.ApiHttpClient.GetAsync<AccountInfo>($"{ApiSettings.BaseUrl}api/profile");
        }
    }
}
