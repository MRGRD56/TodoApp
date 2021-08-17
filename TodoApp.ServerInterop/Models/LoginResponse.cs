using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.ServerInterop.Models
{
    //public record LoginResponse(
    //    int Id,
    //    string Login,
    //    string AccessToken);

    public class LoginResponse
    {
        public int Id { get; }
        public string Login { get; }
        public string AccessToken { get; }

        public LoginResponse(int id, string login, string accessToken)
        {
            Id = id;
            Login = login;
            AccessToken = accessToken;
        }
    }
}
