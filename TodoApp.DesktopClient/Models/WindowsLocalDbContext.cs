using System;
using System.IO;
using TodoApp.ClientLocalDb;

namespace TodoApp.DesktopClient.Models
{
    public class WindowsLocalDbContext : LocalDbContext
    {
        private static readonly string DbFileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"TodoApp\");

        static WindowsLocalDbContext()
        {
            if (!Directory.Exists(DbFileDirectory))
            {
                Directory.CreateDirectory(DbFileDirectory);
            }
        }

        public WindowsLocalDbContext() : base(DbFileDirectory)
        {

        }
    }
}
