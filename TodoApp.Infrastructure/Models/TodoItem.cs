using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using TodoApp.Infrastructure.Annotations;
using TodoApp.Infrastructure.Models.Abstractions;

namespace TodoApp.Infrastructure.Models
{
    public class TodoItem : IDeletable, INotifyPropertyChanged
    {
        private string _text;
        private bool _isDone;
        private bool _isDeleted;
        public int Id { get; set; }
        
        [JsonIgnore]
        public User User { get; set; }
        public int UserId { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public DateTime CreationLocalTime => CreationTime.ToLocalTime();

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public bool IsDone
        {
            get => _isDone;
            set
            {
                if (value == _isDone) return;
                _isDone = value;
                OnPropertyChanged();
            }
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set
            {
                if (value == _isDeleted) return;
                _isDeleted = value;
                OnPropertyChanged();
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}