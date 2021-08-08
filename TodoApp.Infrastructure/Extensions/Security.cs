using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TodoApp.Infrastructure.Extensions
{
    public static class Security
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string correctPasswordHash, string verifiablePassword)
        {
            var isValid = BCrypt.Net.BCrypt.Verify(verifiablePassword, correctPasswordHash);
            return isValid;
        }
    }
}