using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Models.RequestModels.Todo
{
    //public record TodoPutModel(
    //    string Text,
    //    bool? IsDone);

    public class TodoPutModel
    {
        public string Text { get; }
        public bool? IsDone { get; }

        public TodoPutModel(string text, bool? isDone)
        {
            Text = text;
            IsDone = isDone;
        }

        public void Deconstruct(out string text, out bool? isDone)
        {
            text = Text;
            isDone = IsDone;
        }
    }
}