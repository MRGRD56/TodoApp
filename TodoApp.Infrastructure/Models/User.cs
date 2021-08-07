using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;
using TodoApp.Infrastructure.Extensions;
using TodoApp.Infrastructure.Models.Abstractions;

namespace TodoApp.Infrastructure.Models
{
    public class User : IDeletable
    {
        public int Id { get; set; }
        
        public string Login { get; set; }
        
        [JsonIgnore]
        public byte[] Password { get; set; }

        public List<Role> Roles { get; set; } = new();

        public List<TodoItem> TodoItems { get; set; } = new();

        public void SetPassword(string newPassword)
        {
            Password = Security.HashPassword(newPassword);
        }

        public bool VerifyPassword(string password)
        {
            return Security.VerifyPassword(Password, password);
        }

        public bool IsDeleted { get; set; }

        public User()
        {
            
        }
        
        public User(string login, string password, params Role[] roles)
        {
            Login = login;
            SetPassword(password);
            Roles.AddRange(roles);
        }
    }
}