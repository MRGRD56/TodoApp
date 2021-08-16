using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.ServerInterop.Data;

namespace TodoApp.DesktopClient.Models
{
    public class WindowsLocalDbContextFactory : ILocalDbContextFactory
    {
        public LocalDbContext Create() => new WindowsLocalDbContext();
    }
}
