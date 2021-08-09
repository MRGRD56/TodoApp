using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.DesktopClient.Extensions;
using TodoApp.DesktopClient.Models;

namespace TodoApp.DesktopClient.Services.ServerInterop
{
    public static class Profile
    {
        public static async Task<AccountInfo> GetAsync()
        {
            if (!Auth.IsAuthenticated)
            {
                throw new Exception("You are not logged in!");
            }

            return await Auth.ApiHttpClient.GetAsync<AccountInfo>($"{ApiSettings.BaseUrl}api/profile");
        }
    }
}
