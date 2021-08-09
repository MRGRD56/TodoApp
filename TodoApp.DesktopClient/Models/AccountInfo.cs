using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.DesktopClient.Models
{
    public class AccountInfo
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string[] Roles { get; set; }
    }
}
