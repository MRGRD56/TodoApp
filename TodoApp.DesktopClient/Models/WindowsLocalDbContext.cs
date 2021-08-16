using System;
using System.IO;
using TodoApp.ClientLocalDb;

namespace TodoApp.DesktopClient.Models
{
    public class WindowsLocalDbContext : LocalDbContext
    {
        public WindowsLocalDbContext() 
            : base(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"TodoApp\"))
        {

        }
    }
}
