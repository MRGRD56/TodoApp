using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp.Infrastructure.Models.Abstractions
{
    public interface IDeletable
    {
        public bool IsDeleted { get; set; }
    }
}