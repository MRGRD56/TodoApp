using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Models.RequestModels.Todo
{
    public class TodoPostModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Specify text")]
        public string Text { get; }

        public TodoPostModel(string text)
        {
            Text = text;
        }
    }
}