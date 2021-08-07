using System;
using TodoApp.Infrastructure.Models.Abstractions;

namespace TodoApp.Infrastructure.Models
{
    public class TodoItem : IDeletable
    {
        public int Id { get; set; }
        
        public User User { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        public string Text { get; set; }

        public bool IsDone { get; set; }
        
        public bool IsDeleted { get; set; }

        public TodoItem()
        {
            
        }

        public TodoItem(string text, bool isDone = false)
        {
            Text = text;
            IsDone = isDone;
        }
    }
}