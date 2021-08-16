using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TodoApp.Infrastructure.Models.Auth
{
    public class AuthOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public int TokenLifetime { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
    }
}