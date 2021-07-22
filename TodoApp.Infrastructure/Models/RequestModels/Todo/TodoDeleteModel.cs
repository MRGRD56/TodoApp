using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Models.RequestModels.Todo
{
    public record TodoDeleteModel(
        [Required]
        IEnumerable<int> Id,
        bool Restore);
}