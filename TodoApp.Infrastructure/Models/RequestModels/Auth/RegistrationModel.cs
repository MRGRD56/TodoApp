using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Models.RequestModels.Auth
{
    public record RegistrationModel(
        [Required(ErrorMessage = "Specify the login")]
        [RegularExpression(@"^(?=.*[A-Za-z0-9]$)[A-Za-z][A-Za-z\d.-_]{0,19}$", 
            ErrorMessage = "Invalid login")]
        string Login,
        [Required(ErrorMessage = "Specify the password")]
        [RegularExpression(@"^(?=.*[A-Za-zА-Яа-яЁё])(?=.*\d).{6,}$", 
            ErrorMessage = "The password must be at least 6 characters long and contain at least one number and one letter")]
        string Password,
        [Required]
        string PasswordRepeat);
}