using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Models.RequestModels.Todo
{
    public record TodosPutModel(
        [Required]
        IEnumerable<int> Id);
}