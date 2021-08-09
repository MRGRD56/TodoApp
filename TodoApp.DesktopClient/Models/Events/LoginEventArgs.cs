using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.DesktopClient.Models.Events
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
