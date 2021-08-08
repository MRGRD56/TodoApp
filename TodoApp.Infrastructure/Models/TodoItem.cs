using System;
using Newtonsoft.Json;
using TodoApp.Infrastructure.Models.Abstractions;

namespace TodoApp.Infrastructure.Models
{
    public class TodoItem : IDeletable
    {
        public int Id { get; set; }
        
        [JsonIgnore]
        public User User { get; set; }
        public int UserId { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        public string Text { get; set; }

        public bool IsDone { get; set; }
        
        public bool IsDeleted { get; set; }

        public TodoItem()
        {
            
        }

        public TodoItem(string text, User user, bool isDone = false)
        {
            Text = text;
            User = user;
            IsDone = isDone;
        }
        
        public TodoItem(string text, int userId, bool isDone = false)
        {
            Text = text;
            UserId = userId;
            IsDone = isDone;
        }
    }
}