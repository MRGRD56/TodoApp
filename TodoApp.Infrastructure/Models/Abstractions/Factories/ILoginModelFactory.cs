using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Infrastructure.Models.Abstractions.Factories
{
    public interface ILoginModelFactory
    {
        ILoginModel CreateLoginModel(string login, string password);
    }
}
