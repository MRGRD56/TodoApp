using System.ComponentModel.DataAnnotations;
using TodoApp.Infrastructure.Models.Abstractions;

namespace TodoApp.Infrastructure.Models.RequestModels.Auth
{
    public record RegistrationModel(
        [Required(AllowEmptyStrings = false, ErrorMessage = "Specify the login")]
        [RegularExpression(@"^(?=.*[A-Za-z0-9]$)[A-Za-z][A-Za-z\d.-_]{0,19}$", 
            ErrorMessage = "Invalid login")]
        [MinLength(3, ErrorMessage = "The login must be at least 3 characters long")]
        string Login,
        [Required(AllowEmptyStrings = false, ErrorMessage = "Specify the password")]
        [RegularExpression(@"^(?=.*[A-Za-zА-Яа-яЁё])(?=.*\d).{6,}$", 
            ErrorMessage = "The password must be at least 6 characters long and contain at least one number and one letter")]
        string Password) : ILoginModel;
}