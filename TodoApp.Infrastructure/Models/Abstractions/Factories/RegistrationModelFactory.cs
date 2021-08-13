using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Infrastructure.Models.RequestModels.Auth;

namespace TodoApp.Infrastructure.Models.Abstractions.Factories
{
    public class RegistrationModelFactory : ILoginModelFactory
    {
        public ILoginModel CreateLoginModel(string login, string password)
        {
            return new RegistrationModel(login, password);
        }
    }
}
