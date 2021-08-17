using System.ComponentModel.DataAnnotations;
using TodoApp.Infrastructure.Models.Abstractions;

namespace TodoApp.Infrastructure.Models.RequestModels.Auth
{
    //public record LoginModel(
    //    [Required(AllowEmptyStrings = false, ErrorMessage = "Specify your login")]
    //    string Login,
    //    [Required(AllowEmptyStrings = false, ErrorMessage = "Specify your password")]
    //    string Password) : ILoginModel;

    public class LoginModel : ILoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Specify your login")]
        public string Login { get; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Specify your password")]
        public string Password { get; }

        public LoginModel(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public void Deconstruct(out string login, out string password)
        {
            login = Login;
            password = Password;
        }
    }
}