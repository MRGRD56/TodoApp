using System;

namespace TodoApp.ServerInterop.Models.Events
{
    public class LoginEventArgs : EventArgs
    {
        public AccountInfo User { get; }

        public LoginEventArgs(AccountInfo user)
        {
            User = user;
        }
    }
}
