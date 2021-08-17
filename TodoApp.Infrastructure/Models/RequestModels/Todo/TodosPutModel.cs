using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Models.RequestModels.Todo
{
    public class TodosPutModel
    {
        [Required]
        public IEnumerable<int> Id { get; }

        public TodosPutModel(IEnumerable<int> id)
        {
            Id = id;
        }

        public void Deconstruct(out IEnumerable<int> id)
        {
            id = Id;
        }
    }
}