using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Infrastructure.Models.Abstractions
{
    public interface ILoginModel
    {
        string Login { get; init; }
        string Password { get; init; }
    }
}
