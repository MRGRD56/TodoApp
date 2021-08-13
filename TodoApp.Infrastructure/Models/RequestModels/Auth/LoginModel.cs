using System.ComponentModel.DataAnnotations;
using TodoApp.Infrastructure.Models.Abstractions;

namespace TodoApp.Infrastructure.Models.RequestModels.Auth
{
    public record LoginModel(
        [Required(AllowEmptyStrings = false, ErrorMessage = "Specify your login")]
        string Login,
        [Required(AllowEmptyStrings = false, ErrorMessage = "Specify your password")]
        string Password) : ILoginModel;
}