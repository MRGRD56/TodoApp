using System.Collections.Generic;
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
        
        public IList<Role> Roles { get; set; }

        public void SetPassword(string newPassword)
        {
            Password = Security.HashPassword(newPassword);
        }

        public bool VerifyPassword(string password)
        {
            return Security.VerifyPassword(Password, password);
        }

        public bool IsDeleted { get; set; }
    }
}