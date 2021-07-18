using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Models.RequestModels.Todo
{
    public record TodoPutModel(
        string Text,
        bool? IsDone);
}