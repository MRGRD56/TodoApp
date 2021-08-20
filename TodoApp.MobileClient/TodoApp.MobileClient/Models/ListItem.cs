using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace TodoApp.MobileClient.Models
{
    public class ListItem
    {
        public string Title { get; }

        public ICommand Command { get; }

        public object CommandParameter { get; }

        public ListItem(string title, ICommand command = null, object commandParameter = null)
        {
            Title = title;
            Command = command;
            CommandParameter = commandParameter;
        }
    }
}
