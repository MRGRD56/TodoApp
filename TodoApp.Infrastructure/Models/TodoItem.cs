using System;

namespace TodoApp.Infrastructure.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        public string Text { get; set; }

        public bool IsDone { get; set; } = false;
    }
}