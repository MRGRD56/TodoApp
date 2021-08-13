using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Infrastructure.Models.RequestModels.Auth;

namespace TodoApp.Infrastructure.Models.Abstractions.Factories
{
    public class LoginModelFactory : ILoginModelFactory
    {
        public ILoginModel CreateLoginModel(string login, string password)
        {
            return new LoginModel(login, password);
        }
    }
}
