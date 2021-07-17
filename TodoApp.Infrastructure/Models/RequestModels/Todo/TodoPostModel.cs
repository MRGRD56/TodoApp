using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Models.RequestModels.Todo
{
    public record TodoPostModel(
        [Required(AllowEmptyStrings = false, ErrorMessage = "Specify text")]
        string Text);
}