using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Models.RequestModels.Todo
{
    public class TodosDeleteModel
    {
        [Required]
        public IEnumerable<int> Id { get; }
        public bool Restore { get; }

        public TodosDeleteModel(IEnumerable<int> id, bool restore)
        {
            Id = id;
            Restore = restore;
        }

        public void Deconstruct(out IEnumerable<int> id, out bool restore)
        {
            id = Id;
            restore = Restore;
        }
    }
}