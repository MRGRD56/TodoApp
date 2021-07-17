using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Models.RequestModels.Todo
{
    public record TodoPutModel(
        [Required(ErrorMessage = "Specify ID")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid ID")]
        int? Id,
        string Text,
        bool? IsDone);
}