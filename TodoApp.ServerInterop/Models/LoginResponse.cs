using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.ServerInterop.Models
{
    public record LoginResponse(
        int Id,
        string Login,
        string AccessToken);
}
