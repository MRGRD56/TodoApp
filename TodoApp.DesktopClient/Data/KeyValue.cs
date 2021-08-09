using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.DesktopClient.Data
{
    public class KeyValue
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }

        public KeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
