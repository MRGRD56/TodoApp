using System;
using System.Collections.Generic;
using System.Text;
using TodoApp.ClientLocalDb;

namespace TodoApp.MobileClient.Models
{
    public class XamarinLocalDbContextFactory : ILocalDbContextFactory
    {
        public LocalDbContext Create() => new XamarinLocalDbContext();
    }
}
